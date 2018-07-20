using System.Collections.Generic;
using System.Text;
using Microsoft.VisualBasic.FileIO;

namespace MET.Workflows
{
    public class CsvReader
    {

        public string Delimiter { get; set; }

        public FieldType FieldType { get; set; } = FieldType.Delimited;

        public IEnumerable<string[]> ReadCsv(string path, Encoding encoding)
        {
            using (TextFieldParser parser = new TextFieldParser(path, encoding))
            {
                parser.SetDelimiters(Delimiter);
                parser.TextFieldType = FieldType;

                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    yield return fields;
                }
            }
        }
    }
}
