using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using LandOfRails_Launcher.Helper;

namespace LandOfRails_Launcher
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        public static string Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static bool Update = true;
        public static bool GUI = true;
        public static LoginWindow window;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Init();
        }
        private async Task Init()
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
