using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using METCSV.WPF.Downloaders;
using METCSV.WPF.Enums;
using METCSV.WPF.Interfaces;
using METCSV.WPF.Models;
using METCSV.WPF.ProductProvider;
using METCSV.WPF.ProductReaders;
using Prism.Mvvm;

namespace METCSV.WPF.ViewModels
{
    class MainWindowViewModels : BindableBase
    {

        private CancellationTokenSource _cancellationTokenSource;

        IProductProvider _met;
        IProductProvider _lama;
        IProductProvider _techData;
        IProductProvider _ab;

        public MainWindowViewModels()
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

        private void Start()
        {
            var met = _met.GetProducts();
            var lama = _lama.GetProducts();
            var techData = _techData.GetProducts();
            var ab = _ab.GetProducts();
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
