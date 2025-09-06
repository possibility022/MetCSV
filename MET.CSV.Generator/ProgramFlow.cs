using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        private IProductProvider met;
        public IProductProvider Lama;
        public IProductProvider TechData;
        public IProductProvider Ab;


        readonly IObjectFormatterConstructor<object> objectFormatterSource;

        private readonly CancellationToken token;

        private IReadOnlyCollection<CategoryProfit> CategoryProfits { get; set; }
        private IReadOnlyCollection<CustomProfit> CustomProfits { get; set; }
        private IReadOnlyCollection<ManufacturerProfit> ManufacturersProfits { get; set; }

        public List<Product> MetCustomProducts { get; private set; }

        private IReadOnlyDictionary<string, string> RenameManufacturerDictionary { get; set; }

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
            storageService.MakeSureDbCreated();
            Initialize();
            return DownloadAndLoadAsync();
        }

        public async Task StepTwo()
        {
            var metProducts = met.GetProducts();
            var metProd = new MetCustomProductsDomain();
            var metCustomProducts = metProd.ModifyList(metProducts);

            products = new Products()
            {
                AbProducts = Ab.GetProducts(),
                AbProductsOld = Ab.LoadOldProducts(),
                MetProducts = metProducts,
                MetCustomProducts = metCustomProducts,
                TechDataProducts = TechData.GetProducts(),
                TechDataProductsOld = TechData.LoadOldProducts(),
                LamaProducts = Lama.GetProducts(),
                LamaProductsOld = Lama.LoadOldProducts()
            };

            CustomProfits = storageService.GetCustomProfits().ToList();
            CategoryProfits = storageService.GetCategoryProfits().ToList();
            ManufacturersProfits = storageService.GetManufacturersProfits().ToList();
            RenameManufacturerDictionary = storageService.GetRenameManufacturerDictionary();

            var success = await StartFlow();
            if (!success) return;

            MetCustomProducts = metCustomProducts;
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

                orchestrator.SetIgnoredCategories(storageService.GetIgnoredCategories().ToList());

                orchestrator.AddMetCollection(products.MetProducts);

                var productCollection = new ProductLists();
                productCollection.AddList(Providers.Ab, products.AbProducts);
                productCollection.AddList(Providers.Lama, products.LamaProducts);
                productCollection.AddList(Providers.TechData, products.TechDataProducts);

                orchestrator.SetCollections(productCollection);
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

        private void Initialize()
        {
            met = new MetProductProvider(
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

            Ab = new AbProductProvider(
                settings.AbReaderSettings,
                settings.AbDownloaderSettings,
                offlineMode,
                token);
        }

        private async Task<bool> DownloadAndLoadAsync()
        {
            var met = this.met.DownloadAndLoadAsync();
            var lama = Lama.DownloadAndLoadAsync();
            var techData = TechData.DownloadAndLoadAsync();
            var ab = Ab.DownloadAndLoadAsync();

            await Task.WhenAll(met, lama, techData, ab);

            return met.Result && lama.Result && techData.Result && ab.Result;
        }

        private void SetWarehouseToZeroIfPriceError()
        {
            PriceErrorDomain priceError;

            if (products.AbProductsOld != null)
            {
                priceError = new PriceErrorDomain(products.AbProductsOld, products.AbProducts, maximumPriceDifference, objectFormatterSource.GetNewInstance());
                priceError.ValidateSingleProduct();
            }

            if (products.LamaProductsOld != null)
            {
                priceError = new PriceErrorDomain(products.LamaProductsOld, products.LamaProducts, maximumPriceDifference, objectFormatterSource.GetNewInstance());
                priceError.ValidateSingleProduct();
            }

            if (products.TechDataProductsOld != null)
            {
                priceError = new PriceErrorDomain(products.TechDataProductsOld, products.TechDataProducts, maximumPriceDifference, objectFormatterSource.GetNewInstance());
                priceError.ValidateSingleProduct();
            }
        }

        public ICollection<Product>? GetFinalList(bool includeMetCustomProducts)
        {
            if (finalList == null)
                return null;

            if (includeMetCustomProducts)
            {
                var list = new List<Product>(finalList.Count + MetCustomProducts.Count);
                list.AddRange(finalList);
                list.AddRange(MetCustomProducts);
                return list;
            }
            else
            {
                var list = new List<Product>(finalList);
                return list;
            }
        }
    }
}