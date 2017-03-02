using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace METCSV.Database
{
    class Log
    {
        public static Log Logging = new Log();
        private static StreamWriter fileStream;

        public static void log(string message)
        {
            Log.Logging.log_message(message);
        }

        public static void close()
        {
            fileStream.Close();
        }


        public delegate void Write(string message);
        Write write;

        public Log()
        {
            if (File.Exists("log.log") == false)
                File.Create("log.log");

            Log.fileStream = new StreamWriter("log.log");

            write = writeToFile;
        }

        private void empty(string s) { }

        private void writeToFile(string message)
        {
            fileStream.WriteLine(message);
        }

        public void log_message(string message)
        {
            string fullmessage = getLineInfo() + message;
            write(fullmessage);
        }

        private string getLineInfo()
        {
            return DateTime.Now.ToString(@"MM\/dd\/yyyy HH:mm:ss") + " : ";
        }

        public void addWriteHandler(Write _delegate)
        {
            write += _delegate;
        }
    }
}
