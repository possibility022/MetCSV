using System;
using System.Collections.Generic;
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
using METCSV.WPF.ViewModels.ProfitsInnerModels;
using Microsoft.Toolkit.Mvvm.Input;

namespace METCSV.WPF.ViewModels
{
    class MainWindowViewModel : BindableBase, IDisposable
    {
        public MainWindowViewModel()
        {
            SetProfits = App.Settings?.Engine?.SetProfits ?? true;
            SetIgnoredCategories = App.Settings?.Engine?.SetIgnoredCategories ?? true;
            OfflineModeVisibility = App.Settings?.Engine?.OfflineMode == true ? Visibility.Visible : Visibility.Hidden;

            ShowMetProductListEditorCommand = new RelayCommand(() => ShowMetListEditor());
            ShowAllProductsGroupsCommand = new RelayCommand(() => ShowAllProductsWindow());
            ShowSettingsWindowCommand = new RelayCommand(() =>
            {
                var w = new SettingsWindow();
                w.ShowDialog();
                OfflineModeVisibility = App.Settings.Engine.OfflineMode == true ? Visibility.Visible : Visibility.Hidden;
            });
            StartCommand = new RelayCommand(() =>
            {
                if (mainTask == null || mainTask.IsCompleted)
                {
                    mainTask = StartClickAsync();
                }
                else
                {
                    MessageBox.Show("Zaczekaj, jeszcze generuje.");
                }
            });

            StopCommand = new RelayCommand(() =>
            {
                if (InProgress == OperationStatus.InProgress)
                {
                    cancellationTokenSource.Cancel();
                }
            });

            storage = new StorageService(new StorageContext());
        }

        public OperationStatus InProgress
        {
            get => inProgress;
            set
            {
                SetProperty(ref inProgress, value);
                ExportEnabled = value == OperationStatus.Complete;
            }
        }

        private Task mainTask;
        ProfitsWindow profitsView;
        CategoryFilter categoryFilter;
        private bool setProfits;
        private bool setIgnoredCategories;

        public bool SetIgnoredCategories
        {
            get => setIgnoredCategories;
            set => SetProperty(ref setIgnoredCategories, value);
        }

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
        private Visibility offlineModeVisibility = Visibility.Hidden;

        public ICommand ShowMetProductListEditorCommand { get; }
        public ICommand ShowAllProductsGroupsCommand { get; }

        public Visibility OfflineModeVisibility
        {
            get => offlineModeVisibility;
            set => SetProperty(ref offlineModeVisibility, value);
        }

        public ICommand ShowSettingsWindowCommand { get; }

        public ICommand StartCommand { get; }

        public ICommand StopCommand { get; }


        public async Task<(ProfitsViewModel, CategoryFilterViewModel)> PrepareWindows()
        {
            await storage.MakeSureDbCreatedAsync();
            ProfitsViewModel profitsViewModel = null;
            CategoryFilterViewModel categoryFilterViewModel = null;

            var lamaCategories = HelpMe.GetCategoriesCollectionAsync(programFlow.Lama);
            var techDataCategories = HelpMe.GetCategoriesCollectionAsync(programFlow.TechData);
            var abCategories = HelpMe.GetCategoriesCollectionAsync(programFlow.AB);

            if (SetProfits)
            {
                profitsView = new ProfitsWindow();
                profitsViewModel = (ProfitsViewModel)profitsView.DataContext;
                LoadCategoryProfits(profitsViewModel);
                LoadManufacturersProfits(profitsViewModel);
                LoadCustomProfits(profitsViewModel);

                profitsViewModel.AddAllProductsLists(programFlow.Lama.GetProducts(), programFlow.TechData.GetProducts(), programFlow.AB.GetProducts());


                var lamaManufacturers = HelpMe.GetManufacturersAsync(programFlow.Lama);
                var techDataManufacturers = HelpMe.GetManufacturersAsync(programFlow.TechData);
                var abManufacturers = HelpMe.GetManufacturersAsync(programFlow.AB);

                await Task.WhenAll(lamaCategories, techDataCategories, abCategories, lamaManufacturers, techDataManufacturers, abManufacturers);

                var dataContext = profitsViewModel;

                dataContext.AddCategories(lamaCategories.Result);
                dataContext.AddCategories(techDataCategories.Result);
                dataContext.AddCategories(abCategories.Result);

                dataContext.AddManufacturers(lamaManufacturers.Result);
                dataContext.AddManufacturers(techDataManufacturers.Result);
                dataContext.AddManufacturers(abManufacturers.Result);
            }

            if (SetIgnoredCategories)
            {
                await Task.WhenAll(lamaCategories, techDataCategories, abCategories);

                categoryFilter = new CategoryFilter();
                categoryFilterViewModel = (CategoryFilterViewModel)categoryFilter.DataContext;

                categoryFilterViewModel.LoadCategories(lamaCategories.Result);
                categoryFilterViewModel.LoadCategories(techDataCategories.Result);
                categoryFilterViewModel.LoadCategories(abCategories.Result);

                categoryFilterViewModel.LoadIgnoredCategories(storage.GetIgnoredCategories());
            }

            return (profitsViewModel, categoryFilterViewModel);
        }

        private void LoadCategoryProfits(ProfitsViewModel profitsViewModel)
        {
            var method = new Action<Profits>((p) => profitsViewModel.AddCategoryProfit(p));
            LoadProfits(storage.GetCategoryProfits(), method);
        }

        private void LoadManufacturersProfits(ProfitsViewModel profitsViewModel)
        {
            var method = new Action<Profits>((p) => profitsViewModel.AddManufacturerProfit(p));
            LoadProfits(storage.GetManufacturersProfits(), method);
        }

        private void LoadProfits<T>(
            IEnumerable<T> savedProfits,
            Action<Profits> addProfitsAction)
        where T : IProviderProfit, IProfit, IProfitKey
        {
            var abProfits = new Profits(Providers.AB, App.Settings.Engine.DefaultProfit);
            var tdProfits = new Profits(Providers.TechData, App.Settings.Engine.DefaultProfit);
            var lamaProfits = new Profits(Providers.Lama, App.Settings.Engine.DefaultProfit);

            foreach (var profit in savedProfits)
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

                profits.SetNewProfit(profit.ProfitKey, profit.Profit);
            }

            addProfitsAction(abProfits);
            addProfitsAction(tdProfits);
            addProfitsAction(lamaProfits);
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

            var manufacturersProfits = profitsViewModel.GetManufacturersProfits();
            foreach (var manufacturersProfit in manufacturersProfits)
            {
                foreach (var (key, value) in manufacturersProfit.Values)
                {
                    storage.SetProfit(new ManufacturerProfit()
                    {
                        Manufacturer = key,
                        Profit = value,
                        Provider = manufacturersProfit.Provider
                    });
                }
            }

            storage.RemoveManufacturersDefaultProfits(App.Settings.Engine.DefaultProfit);
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
                    if (SetProfits || SetIgnoredCategories)
                    {
                        var (profitsViewModel, categoryFilterViewModel) = await PrepareWindows();

                        if (SetProfits)
                        {
                            profitsView.ShowDialog();
                            if (profitsViewModel.Save)
                                await Task.Run(() => SaveProfits(profitsViewModel));
                        }

                        if (SetIgnoredCategories)
                        {
                            categoryFilter.ShowDialog();
                            if (categoryFilterViewModel.Save)
                            {
                                await Task.Run(() => SaveIgnoredCategoriesAsync(categoryFilterViewModel));
                            }
                        }
                    }

                    await programFlow.StepTwo();
                    InProgress = OperationStatus.Complete;
                    Notification.ShowNotification(true);
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

            Notification.ShowNotification(false);
            return false;
        }

        private void SaveIgnoredCategoriesAsync(CategoryFilterViewModel categoryFilterViewModel)
        {
            foreach((var provider, var categories) in categoryFilterViewModel.GetIgnoredCategories())
            {
                storage.AddIgnoredCategories(provider, categories);
            }
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
                programFlow.MergeCustomMetProducts();
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
            App.Settings.Engine.SetIgnoredCategories = SetIgnoredCategories;
        }

        internal void Stop()
        {
            cancellationTokenSource?.Cancel();
        }

        internal void Loaded()
        {
            //AutoUpdater.Start(App.Settings.Engine.NewVersionURL, System.Reflection.Assembly.GetExecutingAssembly());
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
