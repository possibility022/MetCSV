using MET.Domain.Logic.Comparers;
using MET.Domain.Logic.Models;
using METCSV.Common;
using METCSV.Common.Formatters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MET.Domain.Logic
{
    public class ProductMerger
    {
        private ConcurrentDictionary<string, Product> _hiddenMetProducts;

        List<Product> _finalList;

        ConcurrentDictionary<string, byte> _allPartNumbers = new ConcurrentDictionary<string, byte>();
        ConcurrentDictionary<string, Product> _partNumbersConfilcts = new ConcurrentDictionary<string, Product>();

        /// <summary>
        /// The products out of analyze. Well, store here all the products which you want to omit in main logic and perform post generate method on them.
        /// </summary>
        ConcurrentBag<Product> _productsOutOfAnalyze = new ConcurrentBag<Product>();

        ConcurrentBag<Product> _metBag;
        ConcurrentBag<Product> _lamaProducts;
        ConcurrentBag<Product> _techDataProducts;
        ConcurrentBag<Product> _abProducts;

        Products _products;
        private readonly int maxiumumPriceDifference;

        public event EventHandler<int> StepChanged;

        public event EventHandler<OperationStatus> OnGenerateStateChange;

        readonly IObjectFormatterConstructor<object> ObjectFormatterSource;
        readonly IObjectFormatter<object> ObjectFormatter;

        private CancellationToken _token;

        public IReadOnlyList<Product> FinalList { get { return _finalList; } }

        public ProductMerger(Products products, int maxiumumPriceDifference, CancellationToken token, IObjectFormatterConstructor<object> objectFormatter = null)
        {
            _products = products;
            this.maxiumumPriceDifference = maxiumumPriceDifference;
            _token = token;
            ObjectFormatterSource = objectFormatter ?? new BasicJsonFormatter<object>();
            ObjectFormatter = ObjectFormatterSource.GetNewInstance();
        }

        public async Task<bool> Generate()
        {
            if (_token.IsCancellationRequested)
            {
                Log.Info("Generowanie anulowane przez użytkownika.");
                return false;
            }

            OnGenerateStateChange?.Invoke(this, OperationStatus.InProgress);

            _finalList = new List<Product>();

            try
            {
                // STEP 1
                StepChanged?.Invoke(this, 1);
                await PreGenerateAction();
                SetWarehouseToZeroIfPriceError();


                // STEP 2
                StepChanged?.Invoke(this, 2);
                _metBag = new ConcurrentBag<Product>(_products.MetProducts);
                _lamaProducts = new ConcurrentBag<Product>(_products.LamaProducts);
                _techDataProducts = new ConcurrentBag<Product>(_products.TechDataProducts);
                _abProducts = new ConcurrentBag<Product>(_products.AbProducts);


                // STEP 3
                StepChanged?.Invoke(this, 3);
                RemoveHiddenProducts();

                // STEP 4
                StepChanged?.Invoke(this, 4);

                AllPartNumbersDomain allPartNumbers = new AllPartNumbersDomain();

                allPartNumbers.AddPartNumbers(_metBag, _lamaProducts, _techDataProducts, _abProducts);
                _allPartNumbers = allPartNumbers.GetAllPartNumbers();

                foreach (var partNumber in _allPartNumbers.Keys)
                {
                    ObjectFormatter.WriteLine(partNumber.ToString());
                }

                ObjectFormatter.Flush();

                // STEP 5
                StepChanged?.Invoke(this, 5);
                var fillList = new FillListDomain(_metBag, ObjectFormatterSource);
                _lamaProducts = fillList.FillList(_lamaProducts);
                _abProducts = fillList.FillList(_abProducts);
                _techDataProducts = fillList.FillList(_techDataProducts);

                // STEP 6
                StepChanged?.Invoke(this, 6);
                var setEndOfLive = new EndOfLiveDomain(_metBag, ObjectFormatterSource, _lamaProducts, _abProducts, _techDataProducts);
                setEndOfLive.SetEndOfLife();

                // STEP 7
                // Combine products into groups
                StepChanged?.Invoke(this, 7);

                //var groupingEngine = new GroupingDomain();
                //var combined = groupingEngine.CombineIntoGroups(_lamaProducts, _abProducts, _techDataProducts);
                
                //// Compare products
                //var compare = new PriceDomain(_allPartNumbers, ObjectFormatterSource);
                //compare.Compare(combined);

                // Set Correct Names
                // todo

                // STEP 8 //SolveConflicts();
                StepChanged?.Invoke(this, 8);
                _finalList = CombineList();

                // STEP 9
                StepChanged?.Invoke(this, 9);
                PostGenerateAction();

                // Complete
                StepChanged?.Invoke(this, int.MaxValue);

                OnGenerateStateChange?.Invoke(this, OperationStatus.Complete);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Generowanie przerwane.");
                StepChanged?.Invoke(this, -1);
                OnGenerateStateChange?.Invoke(this, OperationStatus.Faild);
                return false;
            }
        }

        private async Task PreGenerateAction()
        {
            var formatter = ObjectFormatterSource.GetNewInstance();

            var productsToRemove = new HashSet<Product>();
            foreach (var p in _products.TechDataProducts)
            {
                if (p.OryginalnyKodProducenta.Contains("?TN"))
                {
                    p.StatusProduktu = false;
                    p.CenaZakupuNetto = 0;
                    p.SetCennaNetto(0);
                    p.Kategoria = "EOL_TN";
                    productsToRemove.Add(p);

                    formatter.WriteLine("Produkt z oryginalnym kodem producenta: [] posiadał wartość ?TN. Ustawiam Status Produktu, Cene zakupu netto, Cene netto i kategorię. Produkt zostanie również usunięty z listy wejściowej.");
                    formatter.WriteLine("Produkt po zmianach:");
                    formatter.WriteObject(p);
                }
            }

            formatter.Flush();

            foreach (var p in productsToRemove)
            {
                _products.TechDataProducts.Remove(p);
                _productsOutOfAnalyze.Add(p);
            }

            await RemoveProductsWithSpecificCode();
        }

        private async Task RemoveProductsWithSpecificCode()
        {
            var filter = new ProductFilterDomain();
            await filter.RemoveProductsWithSpecificCode(_products);
        }

        private void SetWarehouseToZeroIfPriceError()
        {
            PriceErrorDomain priceError;

            if (_products.AbProducts_Old != null)
            {
                priceError = new PriceErrorDomain(_products.AbProducts_Old, _products.AbProducts, maxiumumPriceDifference, ObjectFormatterSource.GetNewInstance());
                priceError.ValidateSingleProduct();
            }

            if (_products.LamaProducts_Old != null)
            {
                priceError = new PriceErrorDomain(_products.LamaProducts_Old, _products.LamaProducts, maxiumumPriceDifference, ObjectFormatterSource.GetNewInstance());
                priceError.ValidateSingleProduct();
            }

            if (_products.TechDataProducts_Old != null)
            {
                priceError = new PriceErrorDomain(_products.TechDataProducts_Old, _products.TechDataProducts, maxiumumPriceDifference, ObjectFormatterSource.GetNewInstance());
                priceError.ValidateSingleProduct();
            }
        }

        



        private void PostGenerateAction()
        {
            foreach (var p in _productsOutOfAnalyze)
            {
                _finalList.Add(p);
            }
        }

        private void RemoveHiddenProducts()
        {
            var hiddenEngine = new HiddenProductsDomain(ObjectFormatterSource);

            _hiddenMetProducts = hiddenEngine.CreateListOfHiddenProducts(_metBag);

            _metBag = hiddenEngine.RemoveHiddenProducts(_metBag);
            _lamaProducts = hiddenEngine.RemoveHiddenProducts(_lamaProducts);
            _techDataProducts = hiddenEngine.RemoveHiddenProducts(_techDataProducts);
            _abProducts = hiddenEngine.RemoveHiddenProducts(_abProducts);
        }

        private List<Product> CombineList()
        {
            var combinedList = new List<Product>();
            combinedList.AddRange(_lamaProducts);
            combinedList.AddRange(_techDataProducts);
            combinedList.AddRange(_abProducts);

            var endOfLife = _metBag.Where(p => p.Kategoria == EndOfLiveDomain.EndOfLifeCategory);
            combinedList.AddRange(endOfLife);

            combinedList.Sort(new ProductSorter());

            return combinedList;
        }
    }
}