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
using MET.Domain;
using MET.Workflows;
using System.Windows;
using METCSV.Common;
using AutoUpdaterDotNET;
using Notifications.Wpf;
using MET.Domain.Logic.Models;
using System.IO;
using MET.Data.Models;

namespace METCSV.WPF.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        private const string NotificationTitle = "CSV Generator";
        private CancellationTokenSource _cancellationTokenSource;

        private OperationStatus _stepOneStatus;

        public OperationStatus StepOneStatus
        {
            get { return _stepOneStatus; }
            set { SetProperty(ref _stepOneStatus, value); }
        }

        private OperationStatus _stepTwoStatus;
        public OperationStatus StepTwoStatus
        {
            get { return _stepTwoStatus; }
            set { SetProperty(ref _stepTwoStatus, value); }
        }

        private OperationStatus _stepThreeStatus;
        public OperationStatus StepThreeStatus
        {
            get { return _stepThreeStatus; }
            set { SetProperty(ref _stepThreeStatus, value); }
        }

        private OperationStatus _stepFourStatus;
        public OperationStatus StepFourStatus
        {
            get { return _stepFourStatus; }
            set { SetProperty(ref _stepFourStatus, value); }
        }

        private OperationStatus _stepFiveStatus;
        public OperationStatus StepFiveStatus
        {
            get { return _stepFiveStatus; }
            set { SetProperty(ref _stepFiveStatus, value); }
        }

        private OperationStatus _stepSixStatus;
        public OperationStatus StepSixStatus
        {
            get { return _stepSixStatus; }
            set { SetProperty(ref _stepSixStatus, value); }
        }

        private OperationStatus _stepSevenStatus;
        public OperationStatus StepSevenStatus
        {
            get { return _stepSevenStatus; }
            set { SetProperty(ref _stepSevenStatus, value); }
        }

        private OperationStatus _stepeightStatus;
        public OperationStatus StepEightStatus
        {
            get { return _stepeightStatus; }
            set { SetProperty(ref _stepeightStatus, value); }
        }

        private OperationStatus _stepNineStatus;
        public OperationStatus StepNineStatus
        {
            get { return _stepNineStatus; }
            set { SetProperty(ref _stepNineStatus, value); }
        }

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
        }

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
            ProfitsViewModel profitsViewModel;

            _profitsView = new ProfitsWindow();
            profitsViewModel = _profitsView.DataContext as ProfitsViewModel;

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

                        profitsViewModel.SaveAllProfits();
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

            if (_productMerger != null)
            {
                _productMerger.StepChanged -= _productMerger_StepChanged;
            }

            _productMerger = new ProductMerger(
                products,
                App.Settings.Engine.MaximumPriceErrorDifference,
                _cancellationTokenSource.Token);

            _productMerger.StepChanged += _productMerger_StepChanged;
            _productMerger.OnGenerateStateChange += _productMerger_OnGenerateStateChange;

            await _productMerger.Generate();

            Products = new List<Product>(_productMerger.FinalList);
            return true;
        }

        private void _productMerger_OnGenerateStateChange(object sender, OperationStatus e)
        {
            _generatorProgess = e;
        }

        private void _productMerger_StepChanged(object sender, int e)
        {
            switch (e)
            {
                case 1:
                    StepOneStatus = OperationStatus.InProgress;
                    break;
                case 2:
                    StepOneStatus = OperationStatus.Complete;
                    StepTwoStatus = OperationStatus.InProgress;
                    break;
                case 3:
                    StepTwoStatus = OperationStatus.Complete;
                    StepThreeStatus = OperationStatus.InProgress;
                    break;
                case 4:
                    StepThreeStatus = OperationStatus.Complete;
                    StepFourStatus = OperationStatus.InProgress;
                    break;
                case 5:
                    StepFourStatus = OperationStatus.Complete;
                    StepFiveStatus = OperationStatus.InProgress;
                    break;
                case 6:
                    StepFiveStatus = OperationStatus.Complete;
                    StepSixStatus = OperationStatus.InProgress;
                    break;
                case 7:
                    StepSixStatus = OperationStatus.Complete;
                    StepSevenStatus = OperationStatus.InProgress;
                    break;
                case 8:
                    StepSevenStatus = OperationStatus.Complete;
                    StepEightStatus = OperationStatus.InProgress;
                    break;
                case 9:
                    StepEightStatus = OperationStatus.Complete;
                    StepNineStatus = OperationStatus.InProgress;
                    break;
                case int.MaxValue:
                    StepNineStatus = OperationStatus.Complete;
                    App.NotificationManager.Show(new NotificationContent
                    {
                        Message = "Zakończono generowanie powodzeniem.",
                        Title = NotificationTitle,
                        Type = NotificationType.Success
                    });
                    break;
                case -1:
                    SetErrorIconOnWorkingStep(ref _stepOneStatus);
                    SetErrorIconOnWorkingStep(ref _stepTwoStatus);
                    SetErrorIconOnWorkingStep(ref _stepThreeStatus);
                    SetErrorIconOnWorkingStep(ref _stepFourStatus);
                    SetErrorIconOnWorkingStep(ref _stepFiveStatus);
                    SetErrorIconOnWorkingStep(ref _stepSixStatus);
                    SetErrorIconOnWorkingStep(ref _stepSevenStatus);
                    SetErrorIconOnWorkingStep(ref _stepeightStatus);
                    SetErrorIconOnWorkingStep(ref _stepNineStatus);
                    App.NotificationManager.Show(new NotificationContent
                    {
                        Message = "Zakończono generowanie niepowodzeniem.",
                        Title = NotificationTitle,
                        Type = NotificationType.Error
                    });
                    break;
            }
        }

        private void SetErrorIconOnWorkingStep(ref OperationStatus field)
        {
            if (field == OperationStatus.InProgress)
            {
                field = OperationStatus.Faild;

                //todo can we do something with this? RaisePropertyChangge() wont work.
                RaisePropertyChanged(nameof(StepOneStatus));
                RaisePropertyChanged(nameof(StepTwoStatus));
                RaisePropertyChanged(nameof(StepThreeStatus));
                RaisePropertyChanged(nameof(StepFourStatus));
                RaisePropertyChanged(nameof(StepFiveStatus));
                RaisePropertyChanged(nameof(StepSixStatus));
                RaisePropertyChanged(nameof(StepSevenStatus));
                RaisePropertyChanged(nameof(StepEightStatus));
                RaisePropertyChanged(nameof(StepNineStatus));
            }
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
    }
}
