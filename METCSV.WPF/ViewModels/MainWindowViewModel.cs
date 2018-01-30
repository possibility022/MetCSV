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

        Task<IEnumerable<Product>> _metTask;
        Task<IEnumerable<Product>> _lamaTask;
        Task<IEnumerable<Product>> _techDataTask;
        Task<IEnumerable<Product>> _abTask;

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

        private void DownloadAndLoad()
        {
            //var met = _met.GetProducts();
            //var lama = _lama.GetProducts();
            //var techData = _techData.GetProducts();
            //var ab = _ab.GetProducts();
            

            //HelpMe.GetProviders(met);
        }

        public void StartClick()
        {
            Initialize();
            DownloadAndLoad();
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
