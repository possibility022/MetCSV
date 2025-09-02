using AutoUpdaterDotNET;
using METCSV.Common;
using METCSV.WPF.Configuration;
using METCSV.WPF.Workflows;
using System;
using System.Windows;
using System.Windows.Threading;
using WpfBindingErrors;

namespace METCSV.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public static readonly string ProfitsFileExtension = ".prof";

        public static Settings Settings { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            // Start listening for WPF binding error.
            // After that line, a BindingException will be thrown each time
            // a binding error occurs
            //BindingExceptionThrower.Attach();
            AutoUpdater.ShowSkipButton = false;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            Log.ConfigureNLog();
            SettingsIO.LoadSettings();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            SettingsIO.SaveSettings();
            base.OnExit(e);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            if (ex != null)
                Log.Error(ex);
            else
                Log.Error($"Cannot cast ExceptionObject to Exception ${e.ExceptionObject.GetType()}");
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Error(e.Exception);
        }
    }
}
