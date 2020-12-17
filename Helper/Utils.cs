using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.Logging;
using Application = System.Windows.Application;
using Log = Serilog.Log;
using MessageBox = System.Windows.MessageBox;

namespace LandOfRailsLauncher.Helper
{
    class Utils
    {
        public static bool IsAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        public static string ExePath = Process.GetCurrentProcess().MainModule.FileName;

        public static void SendNotify(string message, string title = null)
        {
            string defaultTitle = (string)Application.Current.FindResource("Utils:NotificationTitle");

            var notification = new NotifyIcon()
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
                    Log.Error("StartAsAdmin", e);
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
                Log.Error("Download", e);
            }
        }

        public static int CompareVersions(string version1, string version2)
        {
            var string1Vals = version1.Split('.');
            var string2Vals = version2.Split('.');

            int length = Math.Max(string1Vals.Length, string2Vals.Length);

            for (int i = 0; i < length; i++)
            {
                int v1 = (i < string1Vals.Length) ? int.Parse(string1Vals[i]) : 0;
                int v2 = (i < string2Vals.Length) ? int.Parse(string2Vals[i]) : 0;

                //Making sure Version1 bigger than version2
                if (v1 > v2)
                {
                    return 1;
                }
                //Making sure Version1 smaller than version2
                if (v1 < v2)
                {
                    return -1;
                }
            }

            //Both are equal
            return 0;
        }
    }
}
