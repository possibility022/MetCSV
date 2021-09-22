using MET.Domain.Logic.Comparers;
using MET.Domain.Logic.Models;
using METCSV.Common;
using METCSV.Common.Formatters;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MET.Data.Models;
using MET.Data.Models.Profits;

namespace MET.Domain.Logic
{
    public class ProductMerger
    {
        List<Product> _finalList;

        Products _products;
        private readonly int maxiumumPriceDifference;


        readonly IObjectFormatterConstructor<object> ObjectFormatterSource;

        private CancellationToken _token;

        public IReadOnlyCollection<CategoryProfit> CategoryProfits { get; set; }
        public IReadOnlyCollection<CustomProfit> CustomProfits { get; set; }

        public IReadOnlyList<Product> FinalList { get { return _finalList; } }

        public ProductMerger(Products products, int maxiumumPriceDifference, CancellationToken token, IObjectFormatterConstructor<object> objectFormatter = null)
        {
            _products = products;
            this.maxiumumPriceDifference = maxiumumPriceDifference;
            _token = token;
            ObjectFormatterSource = objectFormatter ?? new BasicJsonFormatter<object>();
        }

        public async Task<bool> Generate()
        {
            if (_token.IsCancellationRequested)
            {
                Log.Info("Generowanie anulowane przez użytkownika.");
                return false;
            }


            _finalList = new List<Product>();

            try
            {
                // STEP 1
                SetWarehouseToZeroIfPriceError();

                Orchestrator orchestrator = new Orchestrator(new AllPartNumbersDomain(), ObjectFormatterSource, true);

                orchestrator.PriceDomain.SetProfits(CategoryProfits, CustomProfits);

                orchestrator.AddMetCollection(_products.MetProducts);
                orchestrator.SetCollections(_products.AbProducts, _products.LamaProducts, _products.TechDataProducts);
                await orchestrator.Orchestrate();

                FinalListCombineDomain finalListCombineDomain = new FinalListCombineDomain();
                var finalList = finalListCombineDomain.CreateFinalList(orchestrator.GetGeneratedProductGroups());

                finalList.Sort(new ProductSorter());
                _finalList = finalList;
                
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Generowanie przerwane.");
                return false;
            }
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

        //private List<Product> CombineList()
        //{
        //    var combinedList = new List<Product>();
        //    combinedList.AddRange(_lamaProducts);
        //    combinedList.AddRange(_techDataProducts);
        //    combinedList.AddRange(_abProducts);

        //    var endOfLife = _metBag.Where(p => p.Kategoria == EndOfLiveDomain.EndOfLifeCategory);
        //    combinedList.AddRange(endOfLife);

        //    combinedList.Sort(new ProductSorter());

        //    return combinedList;
        //}
    }
}