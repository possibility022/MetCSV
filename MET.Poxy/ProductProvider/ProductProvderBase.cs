using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using MET.Data.Models;
using MET.Proxy.Interfaces;
using METCSV.Common;

namespace MET.Proxy.ProductProvider
{
    public abstract class ProductProviderBase : IProductProvider
    {
        protected const string ArchiveFolder = "Archive";
        protected abstract string ArchiveFileNamePrefix { get; }

        private IDownloader downloader;
        private IProductReader productReader;

        public Providers Provider { get; protected set; }

        protected CancellationToken Token;

        private IList<Product> products;

        public ProductProviderBase(CancellationToken cancellationToken)
        {
            Token = cancellationToken;
        }

        public IList<Product> GetProducts() => products;

        public void SetProductDownloader(IDownloader downloader)
        {
            this.downloader = downloader;
        }

        public void SetProductReader(IProductReader reader)
        {
            if (productReader?.Status == OperationStatus.InProgress)
            {
                throw new InvalidOperationException("Cannot set reader when the old one is in progress state.");
            }

            productReader = reader;
        }

        private IList<Product> ReadFile()
        {
            string filename2 =
                downloader.DownloadedFiles.Length >= 2 ?
                    downloader.DownloadedFiles[1]
                    : string.Empty;

            return productReader.GetProducts(downloader.DownloadedFiles[0], filename2);
        }

        public bool DownloadAndLoad()
        {
            var downloaderStatus = downloader.StartDownloading(Token);

            if (!downloaderStatus)
            {
                Log.Info("Wczytywanie przerwane ze względu na nieukończone pobieranie.");
                return false;
            }

            products = ReadFile();
            return true;
        }

        private const string FileExtension = ".bin";

        public ICollection<Product> LoadOldProducts()
        {
            FileInfo file = null;


            if (Directory.Exists(ArchiveFolder))
                file = Directory
                .GetFiles(ArchiveFolder)
                .Select(f => new FileInfo(f))
                .Where(r => r.Name.StartsWith(ArchiveFileNamePrefix) && r.Name.EndsWith(FileExtension) && r.LastWriteTime.Date != DateTime.Today.Date)
                //.Where(r => r.Name.EndsWith(fileExtension))
                .OrderByDescending(fi => fi.CreationTime)
                .FirstOrDefault();


            if (file == null)
                return null;

            using (Stream stream = File.Open(file.FullName, FileMode.Open))
            {
                throw new NotImplementedException();
                //var bin = new BinaryFormatter();
                //return (List<Product>)bin.Deserialize(stream);
            }
        }

        private string GenerateFileName()
            => $"{ArchiveFileNamePrefix}_{DateTime.Now.Date.ToString("d")}{FileExtension}";

        public void SaveAsOldProducts(ICollection<Product> products)
        {
            try
            {
                if (!Directory.Exists(ArchiveFolder))
                    Directory.CreateDirectory(ArchiveFolder);

                using (Stream stream = File.Open(Path.Combine(ArchiveFolder, GenerateFileName()), FileMode.Create))
                {
                    throw new NotImplementedException();
                    //var bin = new BinaryFormatter();
                    //bin.Serialize(stream, products);
                }
            }
            catch (IOException io)
            {
                Log.Error(io);
            }
        }
    }
}



