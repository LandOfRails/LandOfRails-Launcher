using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using log4net;

namespace LandOfRailsLauncher.Helper
{
    class Utils
    {
        public static bool IsAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        public static string ExePath = Process.GetCurrentProcess().MainModule.FileName;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Utils()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        public static void SendNotify(string message, string title = null)
        {
            string defaultTitle = (string)Application.Current.FindResource("Utils:NotificationTitle");

            var notification = new System.Windows.Forms.NotifyIcon()
            {
                Visible = true,
                BalloonTipTitle = title ?? defaultTitle,
                BalloonTipText = message
            };

            notification.ShowBalloonTip(5000);

            notification.Dispose();
        }

        public static void StartAsAdmin(bool Close = false)
        {
            using (Process process = new Process())
            {
                process.StartInfo.FileName = Process.GetCurrentProcess().MainModule.FileName;
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.Verb = "runas";

                try
                {
                    process.Start();

                    if (!Close)
                    {
                        process.WaitForExit();
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show((string)Application.Current.FindResource("Utils:RunAsAdmin"));
                    log.Error("StartAsAdmin", e);
                }

                if (Close) Application.Current.Shutdown();
            }
        }
        public static async Task Download(string link, string output)
        {
            try
            {
                var resp = await Http.HttpClient.GetAsync(link);
                using (var stream = await resp.Content.ReadAsStreamAsync())
                using (var fs = new FileStream(output, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    await stream.CopyToAsync(fs);
                }
            }
            catch (Exception e)
            {
                log.Error("Download", e);
            }
        }
    }
}
