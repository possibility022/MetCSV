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
using METCSV.WPF.Models;

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

        IProductProvider _met;
        IProductProvider _lama;
        IProductProvider _techData;
        IProductProvider _ab;

        ProfitsView _profitsView;
        ProfitsViewModel _profitsViewModel;
        private bool _setProfits = true;

        public bool SetProfits { get => _setProfits; set => SetProperty(ref _setProfits, value); }


        public IconsController LamaController { get => _lamaController; set => SetProperty(ref _lamaController, value); }
        public IconsController MetController { get => _metController; set => SetProperty(ref _metController, value); }
        public IconsController TdController { get => _tdController; set => SetProperty(ref _tdController, value); }
        public IconsController AbController { get => _abController; set => SetProperty(ref _abController, value); }

        IconsController _lamaController;
        IconsController _metController;
        IconsController _tdController;
        IconsController _abController;

        private List<Product> Products;

        public MainWindowViewModel()
        {
            LamaController = new IconsController();
            MetController = new IconsController();
            TdController = new IconsController();
            AbController = new IconsController();
        }

        private void Initialize()
        {
            StepOneStatus = OperationStatus.InProgress;

            _cancellationTokenSource = new CancellationTokenSource();
            _met = new MetProductProvider(_cancellationTokenSource.Token);
            _lama = new LamaProductProvider(_cancellationTokenSource.Token);
            _techData = new TechDataProductProvider(_cancellationTokenSource.Token);
            _ab = new ABProductProvider(_cancellationTokenSource.Token);

            LamaController.SetProvider(_lama);
            MetController.SetProvider(_met);
            AbController.SetProvider(_ab);
            TdController.SetProvider(_techData);
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

            return true;
        }

        public async Task<bool> StartClickAsync()
        {
            StepOneStatus = OperationStatus.InProgress;
            Initialize();
            await DownloadAndLoadAsync();

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
                await StepTwoAsync();
            }

            return true;
        }

        public async Task<bool> StepTwoAsync()
        {
            var ab = ProfitsIO.LoadFromFile(Providers.AB);
            var td = ProfitsIO.LoadFromFile(Providers.TechData);
            var lama = ProfitsIO.LoadFromFile(Providers.Lama);

            await HelpMe.CalculatePricesInBackground(_ab.GetProducts(), ab);
            await HelpMe.CalculatePricesInBackground(_lama.GetProducts(), lama);
            await HelpMe.CalculatePricesInBackground(_techData.GetProducts(), td);

            ProductMerger productMerger = new ProductMerger(
                _met.GetProducts(),
                _lama.GetProducts(),
                _techData.GetProducts(),
                _ab.GetProducts());

            await Task.Run(() => productMerger.Generate());

            Products = new List<Product>(productMerger.FinalList);

            return true;
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
