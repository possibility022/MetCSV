using MET.Data.Models;
using METCSV.WPF.Models;
using Microsoft.Toolkit.Mvvm.Input;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace METCSV.WPF.ViewModels
{
    internal class CategoryFilterViewModel : BindableBase
    {
        private ObservableCollection<string> ignoredCategories;
        private ObservableCollection<string> categories;
        private ObservableCollection<string> providers;
        private Dictionary<Providers, ObservableCollection<string>> categoriesInProviders;
        private Dictionary<Providers, ObservableCollection<string>> ignoredInProviders;
        private string selectedProvider;
        private string selectedItemInIgnoreList;
        private string selectedItemInCategoriesList;

        public ICommand AddToIgnoredCommand { get; }
        public ICommand RemoveFromIgnoredCommand { get; }

        public ObservableCollection<string> Categories
        {
            get => categories;
            private set { this.SetProperty(ref categories, value); }
        }
        public ObservableCollection<string> IgnoredCategories
        {
            get => ignoredCategories;
            private set { this.SetProperty(ref ignoredCategories, value); }
        }

        public ObservableCollection<string> Providers
        {
            get => providers;
            private set { this.SetProperty(ref providers, value); }
        }

        public string SelectedProvider
        {
            get => selectedProvider;
            set
            {
                SetProperty(ref selectedProvider, value);
                SetLists();
            }
        }

        public string SelectedItemInIgnoreList
        {
            get => selectedItemInIgnoreList;
            set => SetProperty(ref selectedItemInIgnoreList, value);
        }
        public string SelectedItemInCategoriesList
        {
            get => selectedItemInCategoriesList;
            set => SetProperty(ref selectedItemInCategoriesList, value);
        }
        public bool Save { get; internal set; } = false;
        public RelayCommand<Window> CloseWindowAndSave { get; private set; }
        public RelayCommand<Window> CloswWindowWithourSaving { get; private set; }

        private void CloseWindow(Window window, bool save)
        {
            this.Save = save;
            if (window != null)
            {
                window.Close();
            }
        }

        public CategoryFilterViewModel()
        {
            AddToIgnoredCommand = new RelayCommand(() => AddToIgnore());
            RemoveFromIgnoredCommand = new RelayCommand(() => RemoveFromIgnore());
            CloseWindowAndSave = new RelayCommand<Window>((w) => this.CloseWindow(w, true));
            CloswWindowWithourSaving = new RelayCommand<Window>((w) => this.CloseWindow(w, false));

            categoriesInProviders = new();
            ignoredCategories = new();
            ignoredInProviders = new();
            categories = new();
            providers = new();
        }

        public void LoadCategories(CategoryCollection categoryCollection)
        {
            if (categoriesInProviders.ContainsKey(categoryCollection.Provider))
                throw new InvalidOperationException($"Can not add categories to provider ({categoryCollection.Provider}). Already contains list for that provider.");

            categoriesInProviders.Add(categoryCollection.Provider, new ObservableCollection<string>(categoryCollection.Categories));

            Providers.Add(categoryCollection.Provider.ToString());
        }

        public void LoadIgnoredCategories(IEnumerable<IgnoreCategory> ignoreCategories)
        {
            foreach (var ignored in ignoreCategories)
            {
                ObservableCollection<string> collection;
                var provider = ignored.ProviderAsType();

                if (ignoredInProviders.ContainsKey(provider))
                {
                    collection = ignoredInProviders[provider];
                }
                else
                {
                    collection = new ObservableCollection<string>();
                    ignoredInProviders.Add(provider, collection);
                }

                collection.Add(ignored.CategoryName);
            }
        }

        private void SetLists()
        {
            if (!string.IsNullOrEmpty(SelectedProvider))
            {
                var provider = Enum.Parse<Providers>(SelectedProvider);

                if (categoriesInProviders.ContainsKey(provider))
                    Categories = categoriesInProviders[provider];
                else
                {
                    var collection = new ObservableCollection<string>();
                    categoriesInProviders.Add(provider, collection);
                    Categories = collection;
                }

                if (ignoredInProviders.ContainsKey(provider))
                    IgnoredCategories = ignoredInProviders[provider];
                else
                {
                    var collection = new ObservableCollection<string>();
                    ignoredInProviders.Add(provider, collection);
                    IgnoredCategories = collection;
                }
            }
        }

        private void RemoveFromIgnore()
        {
            if (!string.IsNullOrEmpty(SelectedItemInIgnoreList))
                if (IgnoredCategories.Contains(SelectedItemInIgnoreList))
                {
                    IgnoredCategories.Remove(SelectedItemInIgnoreList);
                }
        }

        private void AddToIgnore()
        {
            if (!string.IsNullOrEmpty(SelectedItemInCategoriesList))
                IgnoredCategories.Add(SelectedItemInCategoriesList);
        }

        public IEnumerable<(Providers, ICollection<string>)> GetIgnoredCategories()
        {
            foreach(var ignoredList in ignoredInProviders)
            {
                yield return (ignoredList.Key, ignoredList.Value);
            }
        }

    }
}
