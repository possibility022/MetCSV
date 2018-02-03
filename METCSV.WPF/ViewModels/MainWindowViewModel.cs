using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using METCSV.WPF.Downloaders;
using METCSV.WPF.Enums;
using METCSV.WPF.Interfaces;
using METCSV.WPF.ProductProvider;
using METCSV.WPF.ProductProvider;
using METCSV.WPF.ProductReaders;
using METCSV.WPF.Helpers;
using Prism.Mvvm;
using System.Diagnostics;

namespace METCSV.WPF.ViewModels
{
    class MainWindowViewModel : BindableBase
    {

        private CancellationTokenSource _cancellationTokenSource;

        private bool _showProfitsWindow = false;

        IProductProvider _met;
        IProductProvider _lama;
        IProductProvider _techData;
        IProductProvider _ab;

        public MainWindowViewModel()
        {

        }

        private void Initialize()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _met = new MetProductProvider(_cancellationTokenSource.Token);
            _lama = new LamaProductProvider(_cancellationTokenSource.Token);
            _techData = new TechDataProductProvider(_cancellationTokenSource.Token);
            _ab = new ABProductProvider(_cancellationTokenSource.Token);
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

        private async void ShowProfitsWindow()
        {
            var lamaProviders = HelpMe.GetProvidersAsync(_lama.GetProducts());
            var techDataProviders = HelpMe.GetProvidersAsync(_techData.GetProducts());
            var abProviders = HelpMe.GetProvidersAsync(_ab.GetProducts());

            await lamaProviders;
            await techDataProviders;
            await abProviders;

            
        }

        public async Task<bool> StartClickAsync()
        {
            Initialize();
            return await DownloadAndLoadAsync();
        }

        private void OnStatusChanged(object sender, OperationStatus eventArgs)
        {
            //todo implement
        }

        private void OnDownloadingStatusChanged(object sender, OperationStatus eventArgs)
        {
            //todo implement?
        }
    }
}
