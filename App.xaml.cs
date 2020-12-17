using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using LandOfRailsLauncher.Helper;
using Microsoft.VisualBasic.Logging;
using Serilog;
using Log = Serilog.Log;

namespace LandOfRailsLauncher
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App
    {
        public static string Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static bool Update = true;
        public static bool GUI = true;
        public static LoginWindow window;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            String path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "LandOfRails Launcher", "logs");
            Console.WriteLine(path);
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(path+"/LauncherLog-.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

#pragma warning disable 4014
            Init();
#pragma warning restore 4014
        }
        private static async Task Init()
        {
            if (Update)
            {
                try
                {
                    await Task.Run(async () => await Updater.Run());
                }
                catch (UnauthorizedAccessException)
                {
                    Utils.StartAsAdmin(true);
                }
            }

            if (GUI)
            {
                window = new LoginWindow();
                window.Show();
            }
            else
            {
                Current.Shutdown();
            }
        }
    }
}
