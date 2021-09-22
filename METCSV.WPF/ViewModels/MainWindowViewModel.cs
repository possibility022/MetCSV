using System;
using System.Threading;
using System.Threading.Tasks;
using METCSV.WPF.Interfaces;
using METCSV.WPF.ProductProvider;
using METCSV.WPF.Helpers;
using Prism.Mvvm;
using METCSV.WPF.Views;
using METCSV.WPF.Workflows;
using MET.Domain.Logic;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MET.Workflows;
using System.Windows;
using METCSV.Common;
using AutoUpdaterDotNET;
using Notifications.Wpf;
using MET.Domain.Logic.Models;
using System.IO;
using System.Linq;
using MET.Data.Models;
using MET.Data.Models.Profits;
using MET.Data.Storage;
using METCSV.WPF.Models;

namespace METCSV.WPF.ViewModels
{
    class MainWindowViewModel : BindableBase, IDisposable
    {
        private const string NotificationTitle = "CSV Generator";
        private CancellationTokenSource _cancellationTokenSource;
        
        IProductProvider _met;
        IProductProvider _lama;
        IProductProvider _techData;
        IProductProvider _ab;

        public IProductProvider Met { get => _met; set => SetProperty(ref _met, value); }
        public IProductProvider Lama { get => _lama; set => SetProperty(ref _lama, value); }
        public IProductProvider TechData { get => _techData; set => SetProperty(ref _techData, value); }
        public IProductProvider AB { get => _ab; set => SetProperty(ref _ab, value); }

        ProfitsWindow _profitsView;
        private bool _setProfits;

        OperationStatus _generatorProgess = OperationStatus.ReadyToStart;

        public bool SetProfits
        {
            get => _setProfits;
            set => SetProperty(ref _setProfits, value);
        }

        private bool _exportEnabled;
        public bool ExportEnabled
        {
            get { return _exportEnabled; }
            set { SetProperty(ref _exportEnabled, value); }
        }

        private List<Product> _products;
        public List<Product> Products
        {
            get => _products;
            set
            {
                SetProperty(ref _products, value);
                ExportEnabled = value != null;
            }
        }

        private ProductMerger _productMerger;

        public MainWindowViewModel()
        {
            SetProfits = App.Settings?.Engine?.SetProfits ?? true;
            storage = new StorageService(new StorageContext());
            StorageInitializeTask = storage.MakeSureDbCreatedAsync();
        }

        private StorageService storage;

        private Task StorageInitializeTask;

        private void CheckLamaFile()
        {
            var fi = new FileInfo(App.Settings.LamaDownloader.CsvFile);
            if ((DateTime.Now - fi.LastWriteTime).Days > 50)
                MessageBox.Show($"Plik CSV Lamy był ostatnio aktualizowany więcej niż 50 dni temu. Pobierz ręcznie nowy plik i zapisz go tutaj: {fi.FullName}");
        }

        private void Initialize()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            Met = new MetProductProvider(_cancellationTokenSource.Token);
            Lama = new LamaProductProvider(_cancellationTokenSource.Token);
            TechData = new TechDataProductProvider(_cancellationTokenSource.Token);
            AB = new ABProductProvider(_cancellationTokenSource.Token);
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

        public async Task<ProfitsViewModel> PrepareProfitWindow()
        {
            _profitsView = new ProfitsWindow();
            var profitsViewModel = (ProfitsViewModel)_profitsView.DataContext;
            LoadCategoryProfits(profitsViewModel);
            LoadCustomProfits(profitsViewModel);

            profitsViewModel.AddAllProductsLists(_lama.GetProducts(), _techData.GetProducts(), _ab.GetProducts());

            var lamaProviders = HelpMe.GetProvidersAsync(_lama);
            var techDataProviders = HelpMe.GetProvidersAsync(_techData);
            var abProviders = HelpMe.GetProvidersAsync(_ab);

            await Task.WhenAll(lamaProviders, techDataProviders, abProviders);

            var dataContext = profitsViewModel;

            dataContext.AddManufacturers(lamaProviders.Result);
            dataContext.AddManufacturers(techDataProviders.Result);
            dataContext.AddManufacturers(abProviders.Result);

            return profitsViewModel;
        }

        private void LoadCategoryProfits(ProfitsViewModel profitsViewModel)
        {
            var abProfits = new Profits(Providers.AB, App.Settings.Engine.DefaultProfit);
            var tdProfits = new Profits(Providers.TechData, App.Settings.Engine.DefaultProfit);
            var lamaProfits = new Profits(Providers.Lama, App.Settings.Engine.DefaultProfit);

            foreach (var profit in storage.GetCategoryProfits())
            {
                Profits profits;
                switch (profit.Provider)
                {
                    case Providers.AB:
                        profits = abProfits;
                        break;
                    case Providers.Lama:
                        profits = lamaProfits;
                        break;

                    case Providers.TechData:
                        profits = tdProfits;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
                
                profits.SetNewProfit(profit.Category, profit.Profit);
            }

            profitsViewModel.AddCategoryProfit(abProfits);
            profitsViewModel.AddCategoryProfit(tdProfits);
            profitsViewModel.AddCategoryProfit(lamaProfits);
        }

        private void LoadCustomProfits(ProfitsViewModel profitsViewModel)
        {
            var profits = new Profits(Providers.None, App.Settings.Engine.DefaultProfit);
            foreach (var profit in storage.GetCustomProfits())
            {
                profits.SetNewProfit(profit.PartNumber, profit.Profit);
            }

            profitsViewModel.AddCustomProfits(profits);
        }

        private void SaveProfits(ProfitsViewModel profitsViewModel)
        {
            storage.RemoveCategoryDefaultProfits(App.Settings.Engine.DefaultProfit);
            storage.RemoveCustomDefaultProfits(App.Settings.Engine.DefaultProfit);

            var customProfits = profitsViewModel.GetCustomProfits();
            foreach (var (key, value) in customProfits.Values)
            {
                storage.SetProfit(new CustomProfit()
                {
                    Profit = value,
                    PartNumber = key
                });
            }

            var categoryProfits = profitsViewModel.GetCategoryProfits();
            foreach (var categoryProfit in categoryProfits)
            {
                foreach (var (key, value) in categoryProfit.Values)
                {
                    if (value == categoryProfit.DefaultProfit)
                        continue;

                    storage.SetProfit(new CategoryProfit()
                    {
                        Category = key,
                        Profit = value,
                        Provider = categoryProfit.Provider
                    });
                }
            }
        }

        public async Task<bool> StartClickAsync()
        {
            CheckLamaFile();

            if (_generatorProgess == OperationStatus.InProgress)
                return false;
            else
                _generatorProgess = OperationStatus.InProgress;

            try
            {
                var result = await DownloadAndLoadAsync();

                if (result)
                {
                    if (SetProfits)
                    {
                        var profitsViewModel = await PrepareProfitWindow();

                        _profitsView.ShowDialog();

                        SaveProfits(profitsViewModel);
                    }

                    await StepTwoAsync();
                }
                else
                {
                    var message = "Pobieranie i wczytywanie nie powiodło się. Sprawdź logi.";
                    MessageBox.Show(message, "Uwaga!");
                    Log.Error(message);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Main task exception");
                MessageBox.Show($"Mamy problem :(. {ex.Message}", "Uwaga!");
            }

            return false;
        }

        public async Task<bool> StepTwoAsync()
        {
            var ab = ProfitsIO.LoadFromFile(Providers.AB);
            var td = ProfitsIO.LoadFromFile(Providers.TechData);
            var lama = ProfitsIO.LoadFromFile(Providers.Lama);

            HelpMe.CalculatePrices(_ab.GetProducts(), ab);
            HelpMe.CalculatePrices(_lama.GetProducts(), lama);
            HelpMe.CalculatePrices(_techData.GetProducts(), td);

            var metProducts = _met.GetProducts();
            var metProd = new MetCustomProductsDomain();
            var metCustomProducts = metProd.ModifyList(metProducts);

            var products = new Products()
            {
                AbProducts = _ab.GetProducts(),
                AbProducts_Old = _ab.LoadOldProducts(),
                MetProducts = metProducts,
                MetCustomProducts = metCustomProducts,
                TechDataProducts = _techData.GetProducts(),
                TechDataProducts_Old = _techData.LoadOldProducts(),
                LamaProducts = _lama.GetProducts(),
                LamaProducts_Old = _lama.LoadOldProducts()
            };
            
            _productMerger = new ProductMerger(
                products,
                App.Settings.Engine.MaximumPriceErrorDifference,
                _cancellationTokenSource.Token)
            {
                CustomProfits = storage.GetCustomProfits().ToList(),
                CategoryProfits = storage.GetCategoryProfits().ToList()
            };

            await _productMerger.Generate();

            Products = new List<Product>(_productMerger.FinalList);
            return true;
        }

        public void Export(string path)
        {
            //if (Products != null) // todo create settings for that. (To select json or csv)
            //{
            //    string json = JsonConvert.SerializeObject(Products, Formatting.Indented);
            //    File.WriteAllText(path, json);
            //}

            if (Products != null)
            {
                CsvWriter cw = new CsvWriter();
                var success = cw.ExportProducts(path, Products);
                if (!success)
                {
                    Log.Error("Coś poszło nie tak z zapisem.");
                    MessageBox.Show("Coś poszło nie tak z zapisem.");
                }
            }
        }

        internal void Closing()
        {
            App.Settings.Engine.SetProfits = SetProfits;
            App.NotificationManager.CloseWindow();
        }

        internal void Stop()
        {
            _cancellationTokenSource?.Cancel();
        }

        internal void Loaded()
        {
            AutoUpdater.Start(App.Settings.Engine.NewVersionURL, System.Reflection.Assembly.GetExecutingAssembly());
        }

        private void ReleaseUnmanagedResources()
        {
            // TODO release unmanaged resources here
        }

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                _cancellationTokenSource?.Dispose();
                storage?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MainWindowViewModel()
        {
            Dispose(false);
        }
    }
}
