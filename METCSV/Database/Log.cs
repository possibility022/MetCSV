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
            fileStream?.Close();
            fileStream?.Dispose();
        }


        public delegate void Write(string message);
        Write write;

        public Log()
        {
            if (File.Exists("log.log") == false)
                File.Create("log.log").Close();

            fileStream = new StreamWriter("log.log", true);

            write = writeToFile;
        }

        public void LogException(Exception ex)
        {
            log_message("WYJATEK! (ERROR!)");
            log_message("Message:");
            log_message(ex.Message);
            log_message("Stack Trace:");
            log_message(ex.StackTrace);


            if (ex.InnerException != null)
            {
                log_message("Inner Exception Message:");
                log_message(ex.InnerException.Message);
                log_message("Inner Exception Stack Trace:");
                log_message(ex.InnerException.StackTrace);
            }
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
