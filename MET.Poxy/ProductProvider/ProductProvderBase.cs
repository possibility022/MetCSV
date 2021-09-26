using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using MET.Data.Models;
using MET.Proxy.Interfaces;
using METCSV.Common;

namespace MET.Proxy.ProductProvider
{
    abstract public class ProductProviderBase : IProductProvider
    {

        protected const string ArchiveFolder = "Archive";

        protected abstract string ArchiveFileNamePrefix { get; }

        private IDownloader _downloader;
        private IProductReader _productReader;
        private OperationStatus _downloaderStatus;
        private OperationStatus _readerStatus;

        public Providers Provider { get; protected set; }

        protected CancellationToken _token;

        IList<Product> _products;
        IList<Product> _oldProducts;

        public ProductProviderBase(CancellationToken cancellationToken)
        {
            _token = cancellationToken;
        }

        public IList<Product> GetProducts() => _products;

        public IList<Product> GetOldProducts() => _oldProducts;

        public void SetProductDownloader(IDownloader downloader)
        {
            if (_downloader?.Status == OperationStatus.InProgress)
            {
                throw new InvalidOperationException("Cannot set downloader when the old one is in progress state.");
            }

            _downloader = downloader;
        }

        public void SetProductReader(IProductReader reader)
        {
            if (_productReader?.Status == OperationStatus.InProgress)
            {
                throw new InvalidOperationException("Cannot set reader when the old one is in progress state.");
            }

            _productReader = reader;
        }

        private void DownloadData()
        {
            _downloader.Download();
        }

        private IList<Product> ReadFile(IProductReader productReader, IDownloader downloader)
        {
            if (downloader.Status != OperationStatus.Complete)
            {
                ReaderStatus = OperationStatus.Faild;
                Log.Info("Wczytywanie przerwane ze względu na nieukończone pobieranie.");
                return new List<Product>();
            }

            string filename2 =
                downloader.DownloadedFiles.Length >= 2 ?
                    downloader.DownloadedFiles[1]
                    : string.Empty;

            var producets = productReader.GetProducts(downloader.DownloadedFiles[0], filename2);
            

            return producets;
        }

        static public Task<bool> DownloadAndLoadAsync(IProductProvider productProvider)
        {
            Task<bool> task = new Task<bool>(productProvider.DownloadAndLoad);
            task.Start();
            return task;
        }

        public bool DownloadAndLoad()
        {
            DownloadData();
            _products = ReadFile(_productReader, _downloader);
            return _productReader.Status == OperationStatus.Complete && _downloader.Status == OperationStatus.Complete;
        }

        private const string fileExtension = ".bin";

        public ICollection<Product> LoadOldProducts()
        {
            FileInfo file = null;


            if (Directory.Exists(ArchiveFolder))
                file = Directory
                .GetFiles(ArchiveFolder)
                .Select(f => new FileInfo(f))
                .Where(r => r.Name.StartsWith(ArchiveFileNamePrefix) && r.Name.EndsWith(fileExtension) && r.LastWriteTime.Date != DateTime.Today.Date)
                //.Where(r => r.Name.EndsWith(fileExtension))
                .OrderByDescending(fi => fi.CreationTime)
                .FirstOrDefault();


            if (file == null)
                return null;

            using (Stream stream = File.Open(file.FullName, FileMode.Open))
            {
                var bin = new BinaryFormatter();
                return (List<Product>)bin.Deserialize(stream);
            }
        }

        private string GenerateFileName()
            => $"{ArchiveFileNamePrefix}_{DateTime.Now.Date.ToString("d")}{fileExtension}";

        public void SaveAsOldProducts(ICollection<Product> products)
        {
            try
            {
                if (!Directory.Exists(ArchiveFolder))
                    Directory.CreateDirectory(ArchiveFolder);

                using (Stream stream = File.Open(Path.Combine(ArchiveFolder, GenerateFileName()), FileMode.Create))
                {
                    var bin = new BinaryFormatter();
                    bin.Serialize(stream, products);
                }
            }
            catch (IOException io)
            {
                Log.Error(io);
            }
        }
    }
}



