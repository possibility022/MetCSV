using System.Collections.Generic;
using System.IO;
using System.Text;

namespace METCSV.Common
{
    public class CsvWriter
    {

        private const string delimiter = ";";

        private StringBuilder _builder = new StringBuilder();

        public bool ExportProducts(string path, IEnumerable<string> lines, string headers)
        {
            _builder = new StringBuilder();
            try
            {
                using (StreamWriter stream = new StreamWriter(File.Open(path, FileMode.Create), Encoding.GetEncoding("windows-1250")))
                {
                    stream.WriteLine(headers);


                    foreach (var line in lines)
                    {
                        stream.WriteLine(line);
                    }
                }

                return true;
            }
            catch (IOException ex)
            {
                return false;
            }
        }
    }
}
