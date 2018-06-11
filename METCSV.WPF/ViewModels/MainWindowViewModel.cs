using System;
using System.Threading;
using System.Threading.Tasks;
using METCSV.WPF.Enums;
using METCSV.WPF.Interfaces;
using METCSV.WPF.ProductProvider;
using METCSV.WPF.Helpers;
using Prism.Mvvm;
using System.Diagnostics;
using METCSV.WPF.Views;
using METCSV.WPF.Workflows;
using METCSV.WPF.Engine;
using System.Collections.Generic;
using METCSV.Common;
using Newtonsoft.Json;
using System.IO;
using METCSV.WPF.Configuration;
using System.Text;

namespace METCSV.WPF.ViewModels
{
    class MainWindowViewModel : BindableBase
    {

        private CancellationTokenSource _cancellationTokenSource;

        private bool _showProfitsWindow = false;


        private OperationStatus _stepOneStatus = OperationStatus.ReadyToStart;

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

        IProductProvider _met;
        IProductProvider _lama;
        IProductProvider _techData;
        IProductProvider _ab;

        public IProductProvider Met { get => _met; set => SetProperty(ref _met, value); }
        public IProductProvider Lama { get => _lama; set => SetProperty(ref _lama, value); }
        public IProductProvider TechData { get => _techData; set => SetProperty(ref _techData, value); }
        public IProductProvider AB { get => _ab; set => SetProperty(ref _ab, value); }

        ProfitsView _profitsView;
        ProfitsViewModel _profitsViewModel;
        private bool _setProfits = true;

        public bool SetProfits { get => _setProfits; set => SetProperty(ref _setProfits, value); }

        private ProductMerger _productMerger;

        private List<Product> Products;

        public MainWindowViewModel()
        {
            Log.LogConsumersDelegate += LogMessage;
        }

        #region logger

        StringBuilder logBuilder = new StringBuilder();

        private string _logContent;
        public string LogContent
        {
            get { return _logContent; }
            set { SetProperty(ref _logContent, value); }
        }

        void LogMessage(string message)
        {
            logBuilder.AppendLine($"{DateTime.Now.ToString("HH:mm")} : {message}");
            LogContent = logBuilder.ToString(); //todo you have to do something with logger!
        }

        #endregion

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


            var met = ProductProviderBase.DownloadAndLoadAsync(_met);
            var lama = ProductProviderBase.DownloadAndLoadAsync(_lama);
            var techData = ProductProviderBase.DownloadAndLoadAsync(_techData);
            var ab = ProductProviderBase.DownloadAndLoadAsync(_ab);

            await met;
            await lama;
            await techData;
            await ab;

            Debug.WriteLine($"MET: Object: {_met.GetProducts()}; Status: {met.Result}");
            Debug.WriteLine($"LAMA: Object: {_lama.GetProducts()}; Status: {lama.Result}");
            Debug.WriteLine($"TechData Object: {_techData.GetProducts()}; Status: {techData.Result}");
            Debug.WriteLine($"AB Object: {_ab.GetProducts()}; Status: {ab.Result}");

            return met.Result && lama.Result && techData.Result && ab.Result;
        }

        public async Task<bool> StartClickAsync()
        {
            Initialize();

            var result = await DownloadAndLoadAsync();

            if (result)
            {


                if (SetProfits)
                {
                    if (_profitsViewModel == null)
                    {
                        _profitsView = new ProfitsView();
                        _profitsViewModel = _profitsView.DataContext as ProfitsViewModel;
                        _profitsView.Closed += ProfitsWindowClosed;
                    }

                    var lamaProviders = HelpMe.GetProvidersAsync(_lama);
                    var techDataProviders = HelpMe.GetProvidersAsync(_techData);
                    var abProviders = HelpMe.GetProvidersAsync(_ab);

                    await lamaProviders;
                    await techDataProviders;
                    await abProviders;

                    var dataContext = _profitsViewModel as ProfitsViewModel;

                    dataContext.AddManufacturers(lamaProviders.Result);
                    dataContext.AddManufacturers(techDataProviders.Result);
                    dataContext.AddManufacturers(abProviders.Result);

                    _profitsView.Show();
                }
                else
                {
                    result = await StepTwoAsync();
                    return result;
                }
                return true;
            }

            return false;
        }

        public async Task<bool> StepTwoAsync()
        {
            var ab = ProfitsIO.LoadFromFile(Providers.AB);
            var td = ProfitsIO.LoadFromFile(Providers.TechData);
            var lama = ProfitsIO.LoadFromFile(Providers.Lama);

            await HelpMe.CalculatePricesInBackground(_ab.GetProducts(), ab);
            await HelpMe.CalculatePricesInBackground(_lama.GetProducts(), lama);
            await HelpMe.CalculatePricesInBackground(_techData.GetProducts(), td);


            if (_productMerger != null)
            {
                _productMerger.StepChanged -= _productMerger_StepChanged; //todo move to final
            }

            _productMerger = new ProductMerger(
                _met.GetProducts(),
                _lama.GetProducts(),
                _techData.GetProducts(),
                _ab.GetProducts());

            _productMerger.StepChanged += _productMerger_StepChanged;

            await Task.Run(() => _productMerger.Generate());

            Products = new List<Product>(_productMerger.FinalList);

            return true;
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
                    break;
                case -1:
                    SetErrorIconOnWorkingStep(ref _stepOneStatus);
                    SetErrorIconOnWorkingStep(ref _stepTwoStatus);
                    SetErrorIconOnWorkingStep(ref _stepThreeStatus);
                    SetErrorIconOnWorkingStep(ref _stepFourStatus);
                    SetErrorIconOnWorkingStep(ref _stepFiveStatus);
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
            }
        }

        private void ProfitsWindowClosed(object sender, EventArgs e)
        {
            _profitsViewModel.SaveAllProfits();
            var t = Task.Run(() => StepTwoAsync());
        }

        public void Export(string path)
        {
            if (Products != null)
            {
                string json = JsonConvert.SerializeObject(Products, Formatting.Indented);
                File.WriteAllText(path, json);
            }
        }
    }
}
