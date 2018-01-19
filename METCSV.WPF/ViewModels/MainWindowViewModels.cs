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

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

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
            _met = new MetProductProvider();
            _lama = new LamaProductProvider();
            _techData = new TechDataProductProvider();
            _ab = new ABProductProvider();
        }

        private void Start()
        {
            
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
