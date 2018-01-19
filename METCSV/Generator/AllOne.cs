using METCSV.FileSystem;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace METCSV.Generator
{
    class AllOne
    {
        object @lock = new object();

        public ProductsReader lamaReader = new ProductsReader();
        public ProductsReader techDataReader = new ProductsReader();
        public ProductsReader metReader = new ProductsReader();
        public ProductsReader abReader = new ProductsReader();

        public List<Product> finalList = new List<Product>();

        private System.Windows.Forms.OpenFileDialog openFileDialog_Lama = new System.Windows.Forms.OpenFileDialog();
        private System.Windows.Forms.OpenFileDialog openFileDialog_TechDatta_Materials = new System.Windows.Forms.OpenFileDialog();
        private System.Windows.Forms.OpenFileDialog openFileDialog_TechDatta_Prices = new System.Windows.Forms.OpenFileDialog();

        public Global.Result Load()
        {
            #region Pobieranie i wczytywanie danych
            const string techdataProfitsFile = "techdataProfits.data";
            const string abProfitsFile = "abProfits.data";
            const string lamaProfitsFile = "lamaProfits.data";

            Database.Log.Logging.log_message("Zaczynamy");
            Network.LamaWebService downloaderLama = new Network.LamaWebService(delegate () { Database.Log.log("Pobrano Lama"); });
            Network.TechData downloaderTechData = new Network.TechData(delegate() { Database.Log.log("Pobrano TechData"); }, ref this.openFileDialog_TechDatta_Materials, ref openFileDialog_TechDatta_Prices);
            Network.AB downloaderAB = new Network.AB(delegate () { Database.Log.log("Pobrano AB"); });
            Network.Met downloaderMET = new Network.Met(delegate () { Database.Log.log("Pobrano Met"); });

            Database.Log.Logging.log_message("Pobieram");
            downloaderLama.downloadFile();
            downloaderTechData.downloadFile();
            downloaderMET.downloadFile();
            downloaderAB.downloadFile();

            while (!(downloaderLama.TaskIsCompleted() &&
                downloaderTechData.TaskIsCompleted() &&
                downloaderAB.TaskIsCompleted() &&
                downloaderMET.TaskIsCompleted()
                ))
                System.Threading.Thread.Sleep(1000);


            if (downloaderAB.GetDownloadingResult() == Global.Result.faild) return Global.Result.faild;
            if (downloaderTechData.GetDownloadingResult() == Global.Result.faild) return Global.Result.faild;
            if (downloaderLama.GetDownloadingResult() == Global.Result.faild) return Global.Result.faild;
            if (downloaderMET.GetDownloadingResult() == Global.Result.faild) return Global.Result.faild;


            Database.Log.Logging.log_message("Wczytuję pliki");
            loadLamaProducts(downloaderLama.GetFileName(), "LamaCSV.csv");
            loadTechDataProduct("ExtractedFiles\\TD_material.csv", "ExtractedFiles\\TD_Prices.csv");
            loadMetProducts(downloaderMET.GetFileName());
            loadABProducts(downloaderAB.GetFileName());



            while (!(lamaThread.IsCompleted && techDataThread.IsCompleted && metThread.IsCompleted && abThread.IsCompleted))
                System.Threading.Thread.Sleep(1000);

            // Tutaj sprawdzam czy wczytywanie się powiodło. Jeśli operacja nie jest skończona to przerywane jest całe generowanie.
            // Dodatkowo spradzam czy wczytano jakieś produkty. Jeśli ich nie wczytano to znaczy że coś poszło nie tak i generowanie też jest przerywane.
            if (lamaReader.lamaLoadResult != Global.Result.complete || lamaReader.lamaFullList.Count == 0)
                return Global.Result.faild;
            if (metReader.metLoadResult != Global.Result.complete || metReader.metFullList.Count == 0)
                return Global.Result.faild;
            if (techDataReader.techDataLoadResult != Global.Result.complete || techDataReader.techdataFullList.Count == 0)
                return Global.Result.faild;
            if (abReader.abLoadResult != Global.Result.complete || abReader.abFullList.Count == 0)
                return Global.Result.faild;

            #endregion

            #region Wczytywanie kategori i ustawianie marży.

            Database.Log.Logging.log_message("Wczytuje kategorie AB");

            PriceCalculator abCalculator = new PriceCalculator();
            abCalculator.setProfits(Exporter.importProfits(abProfitsFile));
            abCalculator.loadProviders(abReader.abFullList, 1.1);

            Database.Log.Logging.log_message("Wczytuje kategorie Lamy");

            PriceCalculator lamaCalculator = new PriceCalculator();
            lamaCalculator.setProfits(Exporter.importProfits(lamaProfitsFile));
            lamaCalculator.loadProviders(lamaReader.lamaFullList, 1.1);

            Database.Log.Logging.log_message("Wczytuje kategorie Tech Daty");

            PriceCalculator tdCalculator = new PriceCalculator();
            tdCalculator.setProfits(Exporter.importProfits(techdataProfitsFile));
            tdCalculator.loadProviders(techDataReader.techdataFullList, 1.1);

            TestingForm fProfitSet = new TestingForm();

            fProfitSet.loadCategories(abCalculator.getProfits(), TestingForm.Provider.AB);
            fProfitSet.loadCategories(lamaCalculator.getProfits(), TestingForm.Provider.Lama);
            fProfitSet.loadCategories(tdCalculator.getProfits(), TestingForm.Provider.TechData);

            if (Global.ShowProfitsWindows)
            {
                Database.Log.Logging.log_message("Okienko do ręcznej konfiguracji");
                fProfitSet.ShowDialog();
                Database.Log.Logging.log_message("Zamknięto okienko.");
            }

            

            abCalculator.LoadProfitsFrom_ListViewItemCollection
                (fProfitSet.getList(TestingForm.Provider.AB));

            lamaCalculator.LoadProfitsFrom_ListViewItemCollection
                (fProfitSet.getList(TestingForm.Provider.Lama));

            tdCalculator.LoadProfitsFrom_ListViewItemCollection
                (fProfitSet.getList(TestingForm.Provider.TechData));

            Database.Log.Logging.log_message("Zapisuję marże do plików.");

            Exporter.exportProfits(techdataProfitsFile, tdCalculator.getProfits());
            Exporter.exportProfits(abProfitsFile, abCalculator.getProfits());
            Exporter.exportProfits(lamaProfitsFile, lamaCalculator.getProfits());

            Database.Log.Logging.log_message("Obliczam ceny dla Lamy");
            lamaCalculator.CountPrice(lamaReader.lamaFullList);

            Database.Log.Logging.log_message("Obliczam ceny dla Tech Daty");
            tdCalculator.CountPrice(techDataReader.techdataFullList);

            Database.Log.Logging.log_message("Obliczam ceny dla AB");
            abCalculator.CountPrice(abReader.abFullList);

            #endregion

            Database.Log.Logging.log_message("Generuję");
            var comparer = new ProductComparer(this.lamaReader.lamaFullList, this.techDataReader.techdataFullList, this.metReader.metFullList, this.abReader.abFullList);
            comparer.Generate();

            this.finalList = comparer.combineLists();
            Database.Log.Logging.log_message("Gotowe");
            return Global.Result.complete;
        }

        public void loadLamaProducts(string xmlDatabase_path, string csvProducNameDatabase_path)
        {
            lamaThread = new Task(() => lamaReader.GetLamaProducts(xmlDatabase_path, csvProducNameDatabase_path));
            lamaThread.Start();
        }

        public void loadTechDataProduct(string csvDatabase_path, string csvTDPrices_path)
        {
            techDataThread = new Task(() => techDataReader.GetTechDataProducts(csvDatabase_path, csvTDPrices_path));
            techDataThread.Start();
        }

        public void loadMetProducts(string csv_path)
        {
            metThread = new Task(() => metReader.GetMetProducts(csv_path));
            metThread.Start();
        }

        public void loadABProducts(string csv_path)
        {
            abThread = new Task(() => abReader.GetABProducts(csv_path, Encoding.GetEncoding("windows-1250")));
            abThread.Start();
        }

        private Task lamaThread;
        private Task techDataThread;
        private Task metThread;
        private Task abThread;

    }
}
