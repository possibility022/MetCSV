using MET.Domain.Logic.Comparers;
using METCSV.Common;
using METCSV.Common.Formatters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MET.Domain.Logic
{
    public class ProductMerger
    {
        private ConcurrentDictionary<string, Product> _hiddenMetProducts;

        List<Product> _finalList;

        ConcurrentDictionary<int, byte> _allPartNumbers = new ConcurrentDictionary<int, byte>();
        ConcurrentDictionary<string, Product> _partNumbersConfilcts = new ConcurrentDictionary<string, Product>();

        /// <summary>
        /// The products out of analyze. Well, store here all the products which you want to omit in main logic and perform post generate method on them.
        /// </summary>
        ConcurrentBag<Product> _productsOutOfAnalyze = new ConcurrentBag<Product>();

        ConcurrentBag<Product> _metBag;
        ConcurrentBag<Product> _lamaProducts;
        ConcurrentBag<Product> _techDataProducts;
        ConcurrentBag<Product> _abProducts;

        ICollection<Product> _metInit;
        ICollection<Product> _lamaInit;
        ICollection<Product> _techInit;
        ICollection<Product> _abInit;

        public event EventHandler<int> StepChanged;

        public event EventHandler<OperationStatus> OnGenerateStateChange;

        readonly IObjectFormatter<Product> ObjectFormatter;

        private CancellationToken _token;

        public IReadOnlyList<Product> FinalList { get { return _finalList; } }

        public ProductMerger(ICollection<Product> met, ICollection<Product> lama, ICollection<Product> td, ICollection<Product> ab, CancellationToken token, IObjectFormatter<Product> objectFormatter = null)
        {
            _metInit = met;
            _lamaInit = lama;
            _techInit = td;
            _abInit = ab;
            _token = token;
            ObjectFormatter = objectFormatter ?? new BasicJsonFormatter<Product>();
        }

        public bool Generate()
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
                PreGenerateAction();


                // STEP 2
                StepChanged?.Invoke(this, 2);
                _metBag = new ConcurrentBag<Product>(_metInit);
                _lamaProducts = new ConcurrentBag<Product>(_lamaInit);
                _techDataProducts = new ConcurrentBag<Product>(_techInit);
                _abProducts = new ConcurrentBag<Product>(_abInit);

                // STEP 3
                StepChanged?.Invoke(this, 3);
                RemoveHiddenProducts();
                
                // STEP 4
                StepChanged?.Invoke(this, 4);
                _allPartNumbers = AllPartNumbersDomain.GetAllPartNumbers(_metBag, _lamaProducts, _techDataProducts, _abProducts);

                // STEP 5
                StepChanged?.Invoke(this, 5);
                var fillList = new FillListDomain(_metBag);
                _lamaProducts = fillList.FillList(_lamaProducts);
                _abProducts = fillList.FillList(_abProducts);
                _techDataProducts = fillList.FillList(_techDataProducts);

                // STEP 6
                StepChanged?.Invoke(this, 6);
                var setEndOfLive = new EndOfLiveDomain(_metBag, _lamaProducts, _abProducts, _techDataProducts);
                setEndOfLive.SetEndOfLife();

                // STEP 7
                StepChanged?.Invoke(this, 7);
                var compare = new CompareDomain(_allPartNumbers);
                compare.Compare(_abProducts, _techDataProducts, _lamaProducts);

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
                Log.Error(ex, "Generate przerwany.");
                StepChanged?.Invoke(this, -1);
                OnGenerateStateChange?.Invoke(this, OperationStatus.Faild);
                return false;
            }
        }

        private void PreGenerateAction()
        {
            var sb = new StringBuilder();

            HashSet<Product> productsToRemove = new HashSet<Product>();
            foreach (var p in _techInit)
            {
                if (p.OryginalnyKodProducenta.Contains("?TN"))
                {
                    p.StatusProduktu = false;
                    p.CenaZakupuNetto = 0;
                    p.SetCennaNetto(0);
                    p.Kategoria = "EOL_TN";
                    productsToRemove.Add(p);

                    sb.AppendLine("Produkt z oryginalnym kodem producenta: [] posiadał wartość ?TN. Ustawiam Status Produktu, Cene zakupu netto, Cene netto i kategorię. Produkt zostanie również usunięty z listy wejściowej.");
                    sb.AppendLine("Produkt po zmianach:");
                    ObjectFormatter.Get(sb, p);
                }
            }

            Log.LogProductInfo(sb.ToString());

            foreach (var p in productsToRemove)
            {
                _techInit.Remove(p);
                _productsOutOfAnalyze.Add(p);
            }
        }

        private void PostGenerateAction()
        {
            foreach(var p in _productsOutOfAnalyze)
            {
                _finalList.Add(p);
            }
        }

        private void RemoveHiddenProducts()
        {
            var hiddenEngine = new HiddenProductsDomain(ObjectFormatter);

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