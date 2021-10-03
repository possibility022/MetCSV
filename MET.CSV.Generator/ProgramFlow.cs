using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using MET.Data.Models;
using MET.Data.Models.Profits;
using MET.Data.Storage;
using MET.Domain.Logic;
using MET.Domain.Logic.Comparers;
using MET.Domain.Logic.Models;
using MET.Proxy.Configuration;
using MET.Proxy.Extensions;
using MET.Proxy.ProductProvider;
using METCSV.Common;
using METCSV.Common.Formatters;

namespace MET.CSV.Generator
{
    public class ProgramFlow
    {
        List<Product> finalList;

        private readonly StorageService storageService;
        private readonly ISettings settings;
        private readonly bool offlineMode;
        private readonly int maximumPriceDifference;

        private Products products;

        public IProductProvider Met;
        public IProductProvider Lama;
        public IProductProvider TechData;
        public IProductProvider AB;


        readonly IObjectFormatterConstructor<object> objectFormatterSource;

        private readonly CancellationToken token;

        public IReadOnlyCollection<CategoryProfit> CategoryProfits { get; private set; }
        public IReadOnlyCollection<CustomProfit> CustomProfits { get; private set; }
        public IReadOnlyCollection<ManufacturerProfit> ManufacturersProfits { get; private set; }

        public List<Product> MetCustomProducts { get; private set; }

        public IReadOnlyDictionary<string, string> RenameManufacturerDictionary { get; private set; }

        public IReadOnlyList<Product> FinalList => finalList;
        public IReadOnlyCollection<ProductGroup> AllProducts { get; private set; }

        private bool InProgress { get; set; }

        public ProgramFlow(
            StorageService storageService, 
            ISettings settings, 
            bool offlineMode, 
            int maximumPriceDifference, 
            CancellationToken token, 
            IObjectFormatterConstructor<object> objectFormatter)
        {
            this.storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.offlineMode = offlineMode;
            this.maximumPriceDifference = maximumPriceDifference;
            this.token = token;
            objectFormatterSource = objectFormatter ?? throw new ArgumentNullException(nameof(objectFormatter));
        }

        public Task<bool> FirstStep()
        {
            if (InProgress)
                throw new InvalidOperationException("Can not start again as flow is in progress.");

            InProgress = true;
            CheckLamaFile();
            storageService.MakeSureDbCreated();
            Initialize();
            return DownloadAndLoadAsync();
        }

        public async Task<bool> StepTwo()
        {
            var metProducts = Met.GetProducts();
            var metProd = new MetCustomProductsDomain();
            var metCustomProducts = metProd.ModifyList(metProducts);

            products = new Products()
            {
                AbProducts = AB.GetProducts(),
                AbProducts_Old = AB.LoadOldProducts(),
                MetProducts = metProducts,
                MetCustomProducts = metCustomProducts,
                TechDataProducts = TechData.GetProducts(),
                TechDataProducts_Old = TechData.LoadOldProducts(),
                LamaProducts = Lama.GetProducts(),
                LamaProducts_Old = Lama.LoadOldProducts()
            };

            CustomProfits = storageService.GetCustomProfits().ToList();
            CategoryProfits = storageService.GetCategoryProfits().ToList();
            ManufacturersProfits = storageService.GetManufacturersProfits().ToList();
            RenameManufacturerDictionary = storageService.GetRenameManufacturerDictionary();

            var success = await StartFlow();
            if (!success)
                return false;

            MetCustomProducts = metCustomProducts;
            return true;
        }

        private async Task<bool> StartFlow()
        {
            finalList = new List<Product>();

            try
            {
                SetWarehouseToZeroIfPriceError();
                Orchestrator orchestrator = new Orchestrator(new AllPartNumbersDomain(), objectFormatterSource, true);

                orchestrator.ManufacturerRenameDomain.SetDictionary(RenameManufacturerDictionary);
                orchestrator.PriceDomain.SetProfits(CategoryProfits, CustomProfits, ManufacturersProfits);

                orchestrator.AddMetCollection(products.MetProducts);
                orchestrator.SetCollections(products.AbProducts, products.LamaProducts, products.TechDataProducts);
                await orchestrator.Orchestrate();

                FinalListCombineDomain finalListCombineDomain = new FinalListCombineDomain();
                var producedList = finalListCombineDomain.CreateFinalList(orchestrator.GetGeneratedProductGroups());

                producedList.Sort(new ProductSorter());
                AllProducts = orchestrator.GetGeneratedProductGroups();
                finalList = producedList;

                InProgress = false;

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
            var fi = new FileInfo(settings.LamaDownloaderSettings.CsvFile);
            if ((DateTime.Now - fi.LastWriteTime).Days > 50)
                MessageBox.Show($"Plik CSV Lamy był ostatnio aktualizowany więcej niż 50 dni temu. Pobierz ręcznie nowy plik i zapisz go tutaj: {fi.FullName}"); //todo remove it from here.
        }

        private void Initialize()
        {
            Met = new MetProductProvider(
                settings.MetDownloaderSettings,
                offlineMode,
                token);

            Lama = new LamaProductProvider(
                settings.LamaDownloaderSettings,
                settings.LamaReaderSettings,
                offlineMode,
                token);

            TechData = new TechDataProductProvider(
                settings.TechDataReaderSettings,
                settings.TechDataDownloaderSettings,
                offlineMode,
                token);

            AB = new ABProductProvider(
                settings.AbReaderSettings,
                settings.AbDownloaderSettings,
                offlineMode,
                token);
        }

        private async Task<bool> DownloadAndLoadAsync()
        {
            var met = Met.DownloadAndLoadAsync();
            var lama = Lama.DownloadAndLoadAsync();
            var techData = TechData.DownloadAndLoadAsync();
            var ab = AB.DownloadAndLoadAsync();

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