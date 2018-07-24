using MET.Domain.Logic.Comparers;
using METCSV.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace MET.Domain.Logic
{
    public class ProductMerger
    {
        private ConcurrentDictionary<string, Product> _hiddenMetProducts;

        List<Product> _finalList;

        ConcurrentDictionary<int, byte> _allPartNumbers = new ConcurrentDictionary<int, byte>();
        ConcurrentDictionary<string, Product> _partNumbersConfilcts = new ConcurrentDictionary<string, Product>();

        ConcurrentBag<Product> _metBag;

        ConcurrentBag<Product> _lamaProducts;
        ConcurrentBag<Product> _techDataProducts;
        ConcurrentBag<Product> _abProducts;

        public event EventHandler<int> StepChanged;

        public IReadOnlyList<Product> FinalList { get { return _finalList; } }

        public ProductMerger(IEnumerable<Product> met, IEnumerable<Product> lama, IEnumerable<Product> td, IEnumerable<Product> ab)
        {
            _metBag = new ConcurrentBag<Product>(met);
            _lamaProducts = new ConcurrentBag<Product>(lama);
            _techDataProducts = new ConcurrentBag<Product>(td);
            _abProducts = new ConcurrentBag<Product>(ab);
        }

        public bool Generate()
        {
            _finalList = new List<Product>();

            try
            {
                // STEP 1
                StepChanged?.Invoke(this, 1);
                RemoveHiddenProducts();


                // STEP 2
                StepChanged?.Invoke(this, 2);
                _allPartNumbers = AllPartNumbersDomain.GetAllPartNumbers(_metBag, _lamaProducts, _techDataProducts, _abProducts);

                // STEP 3
                StepChanged?.Invoke(this, 3);
                var fillList = new FillListDomain(_metBag);
                _lamaProducts = fillList.FillList(_lamaProducts);
                _abProducts = fillList.FillList(_abProducts);
                _techDataProducts = fillList.FillList(_techDataProducts);

                // STEP 4
                StepChanged?.Invoke(this, 4);
                var setEndOfLive = new EndOfLiveDomain(_metBag, _lamaProducts, _abProducts, _techDataProducts);
                setEndOfLive.SetEndOfLife();

                // STEP 5
                StepChanged?.Invoke(this, 5);
                var compare = new CompareDomain(_allPartNumbers);
                compare.Compare(_abProducts, _techDataProducts, _lamaProducts);

                //SolveConflicts();
                StepChanged?.Invoke(this, 6);
                _finalList = CombineList();

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Generate przerwany.");
                StepChanged?.Invoke(this, -1);
                return false;
            }
        }

        private void RemoveHiddenProducts()
        {
            var hiddenEngine = new HiddenProductsDomain();

            _hiddenMetProducts = hiddenEngine.CreateListOfHiddenProducts(_metBag);

            _metBag = hiddenEngine.RemoveHiddenProducts(_metBag);
            _lamaProducts = hiddenEngine.RemoveHiddenProducts(_lamaProducts);
            _techDataProducts = hiddenEngine.RemoveHiddenProducts(_techDataProducts);
            _abProducts = hiddenEngine.RemoveHiddenProducts(_abProducts);
        }

        private List<Product> CombineList()
        {
            List<Product> combinedList = new List<Product>();
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