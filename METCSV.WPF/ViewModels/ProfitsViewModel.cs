using System;
using METCSV.WPF.Models;
using METCSV.WPF.ProductProvider;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MET.Data.Models;
using METCSV.WPF.ViewModels.ProfitsInnerModels;
using METCSV.WPF.Views;
using Microsoft.Toolkit.Mvvm.Input;

namespace METCSV.WPF.ViewModels
{
    internal class ProfitsViewModel : BindableBase
    {
        public ProfitsViewModel()
        {
            ShowProductBrowserCommand = new RelayCommand(() => ShowProductNumberBrowser()); // I know I know
            RemoveProductFromCustomListCommand = new RelayCommand(() => { RemoveSelectedCustomProfit(); }); //todo
        }

        public ProfitsTabInnerViewModel CategoryProfits { get; } = new ProfitsTabInnerViewModel();
        public ProfitsTabInnerViewModel ManufacturersProfits { get; } = new ProfitsTabInnerViewModel();
        private Profits customProfits;
        private ObservableCollection<EditableDictionaryKey<string, double>> customProfitsCollection = new();
        private IList<Product>[] allProducts;
        private PartNumberSearchWindow partNumberSearchWindow;

        private EditableDictionaryKey<string, double> selectedCustomProfitCustomProfitItem;
        
        public EditableDictionaryKey<string, double> SelectedCustomProfitItem
        {
            get => selectedCustomProfitCustomProfitItem;
            set => SetProperty(ref selectedCustomProfitCustomProfitItem, value);
        }



        public ICommand ShowProductBrowserCommand { get; }
        public ICommand RemoveProductFromCustomListCommand { get; }



        public ObservableCollection<EditableDictionaryKey<string, double>> CustomProfits
        {
            get => customProfitsCollection;
            set => SetProperty(ref customProfitsCollection, value);
        }
        public bool Save { get; internal set; } = false;

        public void AddCategoryProfit(Profits profit)
        {
            AddProfit(CategoryProfits, profit);
        }

        public void AddManufacturerProfit(Profits profit)
        {
            AddProfit(ManufacturersProfits, profit);
        }

        private void AddProfit(ProfitsTabInnerViewModel profitsContainer, Profits profit)
        {
            var profits = GetAlreadyExistingProfits(profitsContainer, profit.Provider);

            if (profits == null)
            {
                profitsContainer.ProfitsCollections.Add(profit);
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
            CategoryProfits.SaveCurrentProfits();
            return new List<Profits>(CategoryProfits.ProfitsCollections);
        }

        public Profits GetCustomProfits()
        {
            SaveCustomProfits();
            return customProfits;
        }

        public List<Profits> GetManufacturersProfits()
        {
            ManufacturersProfits.SaveCurrentProfits();
            return new List<Profits>(ManufacturersProfits.ProfitsCollections);
        }

        private Profits GetAlreadyExistingProfits(ProfitsTabInnerViewModel profitsTabInnerViewModel,
            Providers provider)
        {
            return profitsTabInnerViewModel.ProfitsCollections.FirstOrDefault(p => p.Provider == provider);
        }

        private void RemoveSelectedCustomProfit()
        {
            if (selectedCustomProfitCustomProfitItem != null)
            {
                CustomProfits.Remove(selectedCustomProfitCustomProfitItem);
            }
        }

        public void AddManufacturers(ManufacturersCollection manufacturersCollection)
        {
            AddProfits(manufacturersCollection.Manufacturers, ManufacturersProfits, manufacturersCollection.Provider);
        }

        public void AddCategories(CategoryCollection categoryCollection)
        {
            AddProfits(categoryCollection.Categories, CategoryProfits, categoryCollection.Provider);
        }

        private void AddProfits(IEnumerable<string> keys, ProfitsTabInnerViewModel innerModel, Providers provider)
        {
            var profits = GetAlreadyExistingProfits(innerModel, provider);

            if (profits != null)
            {
                profits.AddKeys(keys);
            }
            else
            {
                var newProfits = new Profits(provider, App.Settings.Engine.DefaultProfit);
                newProfits.AddKeys(keys);
                CategoryProfits.ProfitsCollections.Add(newProfits);
                RaisePropertyChanged(nameof(CategoryProfits.ProfitsCollections));
            }
        }

        public void AddAllProductsLists(params IList<Product>[] products)
        {
            allProducts = products;
        }

        private void ShowProductNumberBrowser()
        {

            partNumberSearchWindow = new PartNumberSearchWindow();
            PartNumberSearchWindowViewModel dataContext = (PartNumberSearchWindowViewModel)partNumberSearchWindow.DataContext;

            foreach (var products in allProducts)
            {
                dataContext.AddProducts(products);
            }
            partNumberSearchWindow.ShowDialog();

            var selectedItem = dataContext.SelectedItem;

            if (selectedItem != null)
            {
                CustomProfits.Add(new EditableDictionaryKey<string, double>(selectedItem.PartNumber, 0.1));
            }

            partNumberSearchWindow = null;
        }



        private void SaveCustomProfits()
        {
            customProfits.SetNewProfits(customProfitsCollection);
        }


    }
}
