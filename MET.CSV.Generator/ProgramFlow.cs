using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using MET.Data.Models;
using MET.Data.Models.Profits;
using MET.Domain.Logic;
using MET.Domain.Logic.Comparers;
using MET.Domain.Logic.Models;
using MET.Proxy.Configuration;
using MET.Proxy.ProductProvider;
using METCSV.Common;
using METCSV.Common.Formatters;
using METCSV.WPF.ProductProvider;

namespace MET.CSV.Generator
{
    public class ProgramFlow
    {
        List<Product> finalList;
        IReadOnlyCollection<ProductGroup> allProducts;

        private readonly ISettings settings;
        readonly Products products;
        private readonly int maximumPriceDifference;


        readonly IObjectFormatterConstructor<object> objectFormatterSource;

        private readonly CancellationToken token;

        public IReadOnlyCollection<CategoryProfit> CategoryProfits { get; set; }
        public IReadOnlyCollection<CustomProfit> CustomProfits { get; set; }

        public IReadOnlyDictionary<string, string> RenameManufacturerDictionary { get; set; }


        public IReadOnlyList<Product> FinalList => finalList;
        public IReadOnlyCollection<ProductGroup> AllProducts => allProducts;

        public ProgramFlow(ISettings settings, Products products, int maximumPriceDifference, CancellationToken token, IObjectFormatterConstructor<object> objectFormatter = null)
        {
            this.settings = settings;
            this.products = products;
            this.maximumPriceDifference = maximumPriceDifference;
            this.token = token;
            objectFormatterSource = objectFormatter ?? new BasicJsonFormatter<object>();
        }

        public async Task<bool> StartFlow()
        {
            finalList = new List<Product>();

            try
            {
                SetWarehouseToZeroIfPriceError();

                Orchestrator orchestrator = new Orchestrator(new AllPartNumbersDomain(), objectFormatterSource, true);

                orchestrator.ManufacturerRenameDomain.SetDictionary(RenameManufacturerDictionary);

                orchestrator.PriceDomain.SetProfits(CategoryProfits, CustomProfits);

                orchestrator.AddMetCollection(products.MetProducts);
                orchestrator.SetCollections(products.AbProducts, products.LamaProducts, products.TechDataProducts);
                await orchestrator.Orchestrate();

                FinalListCombineDomain finalListCombineDomain = new FinalListCombineDomain();
                var finalList = finalListCombineDomain.CreateFinalList(orchestrator.GetGeneratedProductGroups());
                
                allProducts = orchestrator.GetGeneratedProductGroups();

                finalList.Sort(new ProductSorter());
                this.finalList = finalList;
                
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Generowanie przerwane.");
                return false;
            }
        }

        private void CheckLamaFile()
        {
            var fi = new FileInfo(settings.LamaSettings.CsvFile);
            if ((DateTime.Now - fi.LastWriteTime).Days > 50)
                MessageBox.Show($"Plik CSV Lamy był ostatnio aktualizowany więcej niż 50 dni temu. Pobierz ręcznie nowy plik i zapisz go tutaj: {fi.FullName}"); //todo remove it from here.
        }

        private async Task<bool> DownloadAndLoadAsync()
        {
            await StorageInitializeTask;
            Initialize();

            var met = ProductProviderBase.DownloadAndLoadAsync(_met);
            var lama = ProductProviderBase.DownloadAndLoadAsync(_lama);
            var techData = ProductProviderBase.DownloadAndLoadAsync(_techData);
            var ab = ProductProviderBase.DownloadAndLoadAsync(_ab);

            await Task.WhenAll(met, lama, techData, ab);

            return met.Result && lama.Result && techData.Result && ab.Result;
        }

        private void SetWarehouseToZeroIfPriceError()
        {
            PriceErrorDomain priceError;

            if (products.AbProducts_Old != null)
            {
                priceError = new PriceErrorDomain(products.AbProducts_Old, products.AbProducts, maximumPriceDifference, objectFormatterSource.GetNewInstance());
                priceError.ValidateSingleProduct();
            }

            if (products.LamaProducts_Old != null)
            {
                priceError = new PriceErrorDomain(products.LamaProducts_Old, products.LamaProducts, maximumPriceDifference, objectFormatterSource.GetNewInstance());
                priceError.ValidateSingleProduct();
            }

            if (products.TechDataProducts_Old != null)
            {
                priceError = new PriceErrorDomain(products.TechDataProducts_Old, products.TechDataProducts, maximumPriceDifference, objectFormatterSource.GetNewInstance());
                priceError.ValidateSingleProduct();
            }
        }
    }
}