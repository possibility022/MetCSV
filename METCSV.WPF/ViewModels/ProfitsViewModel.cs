using System;
using METCSV.WPF.Helpers;
using METCSV.WPF.Models;
using METCSV.WPF.ProductProvider;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MET.Data.Models;
using METCSV.WPF.Commands;
using METCSV.WPF.Views;

namespace METCSV.WPF.ViewModels
{
    internal class ProfitsViewModel : BindableBase
    {
        public ProfitsViewModel()
        {
            profitsCollection = new ObservableCollection<Profits>();
            valuesCache = new Dictionary<Profits, ObservableCollection<EditableDictionaryKey<string, double>>>();
            ShowProductBrowserCommand = new BaseCommand(() => ShowProductNumberBrowser()); // I know I know
            RemoveProductFromCustomListCommand = new BaseCommand(() => { }); //todo
        }

        private ObservableCollection<Profits> profitsCollection;
        private Profits selectedProfits;
        private Profits customProfits;
        private ObservableCollection<EditableDictionaryKey<string, double>> customProfitsCollection = new();
        private ObservableCollection<EditableDictionaryKey<string, double>> values;
        private IList<Product>[] allProducts;
        private PartNumberSearchWindow partNumberSearchWindow;

        private readonly Dictionary<Profits, ObservableCollection<EditableDictionaryKey<string, double>>> valuesCache;
        
        public ObservableCollection<Profits> ProfitsCollections
        {
            get => profitsCollection;
            set => SetProperty(ref profitsCollection, value);
        }

        public string InfoText
        {
            get => $"Edytujesz marże dla: {SelectedProfits?.Provider}.";
        }

        public ICommand ShowProductBrowserCommand { get; }
        public ICommand RemoveProductFromCustomListCommand{ get; }

        public Profits SelectedProfits
        {
            get => selectedProfits;
            set
            {
                SaveCurrentProfits();
                SetProperty(ref selectedProfits, value);
                if (value != null)
                {
                    CacheValues();
                    Values = valuesCache[value];
                }
                RaisePropertyChanged(nameof(InfoText));
            }
        }

        public ObservableCollection<EditableDictionaryKey<string, double>> Values
        {
            get => values;
            set => SetProperty(ref values, value);
        }
        
        public ObservableCollection<EditableDictionaryKey<string, double>> CustomProfits
        {
            get => customProfitsCollection;
            set => SetProperty(ref customProfitsCollection, value);
        }

        public void AddCategoryProfit(Profits profit)
        {
            var profits = GetAlreadyExistingProfits(profit.Provider);

            if (profits == null)
            {
                ProfitsCollections.Add(profit);
            }
            else
            {
                throw new InvalidOperationException("This profit was already added. " + profit.Provider);
            }
        }

        public void AddCustomProfits(Profits profits)
        {
            this.customProfits = profits;
            foreach (var profitsValue in profits.Values)
            {
                CustomProfits.Add(new EditableDictionaryKey<string, double>(profitsValue.Key, profitsValue.Value));
            }

            SaveCustomProfits();
        }

        public List<Profits> GetCategoryProfits()
        {
            SaveCurrentProfits();
            return new List<Profits>(profitsCollection);
        }

        public Profits GetCustomProfits()
        {
            SaveCustomProfits();
            return customProfits;
        }

        private Profits GetAlreadyExistingProfits(Providers provider)
        {
            return ProfitsCollections.FirstOrDefault(p => p.Provider == provider);
        }

        public void AddManufacturers(ManufacturersCollection manufacturersCollection)
        {
            var profits = GetAlreadyExistingProfits(manufacturersCollection.Provider);

            if (profits != null)
            {
                profits.AddManufacturers(manufacturersCollection.Manufacturers);
            }
            else
            {
                var newProfits = new Profits(manufacturersCollection.Provider);
                newProfits.AddManufacturers(manufacturersCollection.Manufacturers);
                ProfitsCollections.Add(newProfits);
                RaisePropertyChanged(nameof(ProfitsCollections));
            }
        }

        public void AddAllProductsLists(params IList<Product>[] products)
        {
            allProducts = products;
        }

        private async Task ShowProductNumberBrowser()
        {
            if (partNumberSearchWindow == null)
            {
                partNumberSearchWindow = new PartNumberSearchWindow();
                PartNumberSearchWindowViewModel dataContext = (PartNumberSearchWindowViewModel)partNumberSearchWindow.DataContext;

                foreach (var products in allProducts)
                {
                    await dataContext.AddProducts(products);
                }
            }

            partNumberSearchWindow.ShowDialog();
        }

        private void SaveCurrentProfits()
        {
            if (Values != null && SelectedProfits != null)
            {
                SelectedProfits.SetNewProfits(Values);
            }
        }

        private void SaveCustomProfits()
        {
            customProfits.SetNewProfits(customProfitsCollection);
        }

        private void CacheValues()
        {
            if (SelectedProfits != null && valuesCache.ContainsKey(SelectedProfits) == false)
                valuesCache.Add(SelectedProfits, CustomConvert.ToObservableCollection(SelectedProfits.Values));
        }
    }
}
