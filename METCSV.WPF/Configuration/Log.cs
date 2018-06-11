using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.IO;

namespace METCSV.WPF.Configuration
{
    static class Log
    {

        private const string LAYOUT = "${longdate}|${level:uppercase=true}|Thread: [${threadid}] | Message: \t${message}\t${exception:format=tostring,StackTrace,Data}";

        public static Logger Logger { get; private set; }

        public static void ConfigureNLog()
        {
            var config = new LoggingConfiguration();

            var FileName = GenerateFileName();
            
            var logfile = new FileTarget() { FileName = GenerateFileName(), Layout = LAYOUT };

            config.AddRule(LogLevel.Info, LogLevel.Fatal, logfile);

            LogManager.Configuration = config;
            Logger = LogManager.GetCurrentClassLogger();
    }

        private static string GenerateFileName()
        {
            var fileNameBase = DateTime.Now.ToString("dd-M-yyyy_HH-mm-ss");
            var fileName = $"{fileNameBase}.log";

            for (int i = 0; i < 100; i++)
            {
                if (!File.Exists(fileName))
                {
                    break;
                }
                else
                {
                    fileName = $"{fileNameBase}_{i}.log";
                }
            }

            if (File.Exists(fileName))
                throw new Exception();

            return fileName;
        }

        public static void Info(string message)
        {
            Logger.Info(message);
        }

        public static void Error(Exception ex, string message)
        {
            Logger.Error(ex, message);
        }
    }
}
