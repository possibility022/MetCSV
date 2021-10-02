using System;
using System.Threading;
using System.Threading.Tasks;
using METCSV.WPF.Helpers;
using Prism.Mvvm;
using METCSV.WPF.Views;
using MET.Workflows;
using System.Windows;
using METCSV.Common;
using AutoUpdaterDotNET;
using System.Windows.Input;
using MET.CSV.Generator;
using MET.Data.Models;
using MET.Data.Models.Profits;
using MET.Data.Storage;
using METCSV.Common.Formatters;
using METCSV.WPF.Models;
using METCSV.WPF.ViewModels.ProfitsInnerModels;
using Microsoft.Toolkit.Mvvm.Input;

namespace METCSV.WPF.ViewModels
{
    class MainWindowViewModel : BindableBase, IDisposable
    {
        public MainWindowViewModel()
        {
            SetProfits = App.Settings?.Engine?.SetProfits ?? true;
            ShowMetProductListEditorCommand = new RelayCommand(() => ShowMetListEditor());
            ShowAllProductsGroupsCommand = new RelayCommand(() => ShowAllProductsWindow());
            storage = new StorageService(new StorageContext());
        }

        private const string NotificationTitle = "CSV Generator";
        
        public OperationStatus InProgress
        {
            get => inProgress;
            set
            {
                SetProperty(ref inProgress, value);
                ExportEnabled = value == OperationStatus.Complete;
            }
        }

        ProfitsWindow profitsView;
        private bool setProfits;


        public bool SetProfits
        {
            get => setProfits;
            set => SetProperty(ref setProfits, value);
        }

        private bool exportEnabled;
        public bool ExportEnabled
        {
            get => exportEnabled;
            private set => SetProperty(ref exportEnabled, value);
        }
        
        private ProgramFlow programFlow;
        private OperationStatus inProgress;
        private readonly StorageService storage;
        private CancellationTokenSource cancellationTokenSource;

        public ICommand ShowMetProductListEditorCommand { get; }
        public ICommand ShowAllProductsGroupsCommand { get; }


        public async Task<ProfitsViewModel> PrepareProfitWindow()
        {
            await storage.MakeSureDbCreatedAsync();
            profitsView = new ProfitsWindow();
            var profitsViewModel = (ProfitsViewModel)profitsView.DataContext;
            LoadCategoryProfits(profitsViewModel);
            LoadCustomProfits(profitsViewModel);
            
            profitsViewModel.AddAllProductsLists(programFlow.Lama.GetProducts(), programFlow.TechData.GetProducts(), programFlow.AB.GetProducts());

            var lamaProviders = HelpMe.GetCategoriesCollectionAsync(programFlow.Lama);
            var techDataProviders = HelpMe.GetCategoriesCollectionAsync(programFlow.TechData);
            var abProviders = HelpMe.GetCategoriesCollectionAsync(programFlow.AB);

            await Task.WhenAll(lamaProviders, techDataProviders, abProviders);

            var dataContext = profitsViewModel;

            dataContext.AddCategories(lamaProviders.Result);
            dataContext.AddCategories(techDataProviders.Result);
            dataContext.AddCategories(abProviders.Result);

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
                    storage.SetProfit(new CategoryProfit()
                    {
                        Category = key,
                        Profit = value,
                        Provider = categoryProfit.Provider
                    });
                }
            }

            storage.RemoveCategoryDefaultProfits(App.Settings.Engine.DefaultProfit);
            storage.RemoveCustomDefaultProfits(App.Settings.Engine.DefaultProfit);
        }

        public async Task<bool> StartClickAsync()
        {
            if (InProgress == OperationStatus.InProgress)
                return false;
            else
                InProgress = OperationStatus.InProgress;

            cancellationTokenSource = new CancellationTokenSource();


            programFlow = new ProgramFlow(
                storage,
                App.Settings,
                App.Settings.Engine.OfflineMode,
                100, //?
                cancellationTokenSource.Token,
                new BasicJsonFormatter<object>()
            );

            try
            {
                var result = await programFlow.FirstStep();

                if (result)
                {
                    if (SetProfits)
                    {
                        var profitsViewModel = await PrepareProfitWindow();

                        profitsView.ShowDialog();

                        SaveProfits(profitsViewModel);
                    }

                    await programFlow.StepTwo();
                    InProgress = OperationStatus.Complete;
                    return true;
                }
                else
                {
                    var message = "Pobieranie i wczytywanie nie powiodło się. Sprawdź logi.";
                    MessageBox.Show(message, "Uwaga!");
                    Log.Error(message);
                    InProgress = OperationStatus.Faild;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Main task exception");
                MessageBox.Show($"Mamy problem :(. {ex.Message}", "Uwaga!");
                InProgress = OperationStatus.Faild;
            }

            return false;
        }

        private void ShowMetListEditor()
        {
            var window = new MetProductListEditor();
            var context = (MetProductListEditorViewModel)window.DataContext;

            context.AddProducts(programFlow.MetCustomProducts);

            window.ShowDialog();
        }

        private void ShowAllProductsWindow()
        {
            var window = new BrowseAllProductsGroupsWindow();
            var context = (BrowseAllProductsGroupsViewModel)window.DataContext;

            context.AddProducts(programFlow.AllProducts);

            window.ShowDialog();
        }

        public void Export(string path)
        {
            //if (Products != null) // todo create settings for that. (To select json or csv)
            //{
            //    string json = JsonConvert.SerializeObject(Products, Formatting.Indented);
            //    File.WriteAllText(path, json);
            //}

            if (programFlow.FinalList != null)
            {
                CsvWriter cw = new CsvWriter();
                var success = cw.ExportProducts(path, programFlow.FinalList);
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
            cancellationTokenSource?.Cancel();
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
                cancellationTokenSource?.Dispose();
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
