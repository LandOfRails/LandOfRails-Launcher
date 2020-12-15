using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using LandOfRailsLauncher.Helper;
using log4net;

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

        private static readonly ILog log = LogManager.GetLogger(typeof(App));
        protected override void OnStartup(StartupEventArgs e)
        {
            log4net.Config.XmlConfigurator.Configure();
            log.Info("        =============  Started Logging  =============        ");
            base.OnStartup(e);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
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
