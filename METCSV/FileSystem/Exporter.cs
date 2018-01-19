using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace METCSV.FileSystem
{
    class Exporter
    {
        private static Encoding defaultEncoding = Encoding.UTF8;//Encoding.GetEncoding("windows-1250");

        public static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        public static void exportProducts(string path, List<Product> products)
        {
            try {
                using (StreamWriter stream = new StreamWriter(File.Open(path, FileMode.Create), Encoding.GetEncoding("windows-1250")))
                {
                    if (products.Count > 0) stream.WriteLine(products[0].getHeader());
                    foreach (Product p in products)
                    {
                        string line = p.getLine();
                        stream.WriteLine(line);
                    }
                }
            }catch (IOException ex)
            {
                System.Windows.Forms.MessageBox.Show("Błąd przy zapisie: " + ex.Message);
            }
        }

        public static void saveToFile(string path, string[] data)
        {
            try
            {
                using (StreamWriter stream = new StreamWriter(File.Open(path, FileMode.Create), defaultEncoding))
                {
                    foreach (string s in data)
                        stream.WriteLine(s);
                }
            }
            catch (IOException ex)
            {
                System.Windows.Forms.MessageBox.Show("Błąd przy zapisie: " + ex.Message);
            }
            catch (ArgumentException ex)
            {
                System.Windows.Forms.MessageBox.Show("Błąd przy zapisie: " + ex.Message);
            }
        }

        private const string profitSeparator = "|";

        public static void exportProfits(string path, Dictionary<string, double> profits)
        {
            string[] lines = new string[profits.Keys.Count];

            int i = 0;

            foreach(string  key in profits.Keys)
            {
                lines[i] = key + profitSeparator + profits[key].ToString();
                i++;
            }

            saveToFile(path, lines);
        }

        public static Dictionary<string, double> importProfits(string path)
        {
            Dictionary<string, double> profits = new Dictionary<string, double>();

            try {
                string[] lines = File.ReadAllLines(path, defaultEncoding);

                foreach (string line in lines)
                {
                    string[] parts = line.Split(profitSeparator.ToCharArray());
                    profits.Add(parts[0], double.Parse(parts[1].Replace(".", ",")));
                }
                return profits;
            }
            catch (Exception ex)
            {
                if (ex is IOException || ex is FileNotFoundException)
                {
                    System.Windows.Forms.MessageBox.Show("Plik nie istnieje lub problem z dostępem do pliku.\r\n Path: " + path + "\r\n" + ex.Message);
                }
                else if (ex is FormatException)
                {
                    System.Windows.Forms.MessageBox.Show("Problem z parsowaniem wartości.\r\n Path: " + path + "\r\n" + ex.Message);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Problem z odczytaniem pliku.\r\n Path: " + path + "\r\n" + ex.Message);
                }
            }

            return new Dictionary<string, double>();
        }

        public static void exportGroups(string path, List<Generator.ProductGroup> groups)
        {
            if (groups == null)
                return;
            string[] lines = new string[groups.Count];

            int i = 0;

            foreach (Generator.ProductGroup group in groups)
            {
                lines[i] = Convert.ToBase64String(group.exportToBytes());
                i++;
            }

            saveToFile(path, lines);
        }

        public static List<Generator.ProductGroup> importGroups(string path)
        {
            List<Generator.ProductGroup> groups = new List<Generator.ProductGroup>();

            try
            {
                string[] lines = File.ReadAllLines(path, defaultEncoding);

                foreach (string line in lines)
                {
                    Generator.ProductGroup group = Generator.ProductGroup.loadFromByte(Convert.FromBase64String(line));
                    groups.Add(group);
                }
                return groups;
            }
            catch (Exception ex)
            {
                if (ex is IOException || ex is FileNotFoundException)
                {
                    System.Windows.Forms.MessageBox.Show("Plik nie istnieje lub problem z dostępem do pliku.\r\nPath: " + path + "\r\n" + ex.Message);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Problem z odczytaniem pliku.\r\nPath: " + path + "\r\n" + ex.Message);
                }
            }

            return null;
        }

    }
}
