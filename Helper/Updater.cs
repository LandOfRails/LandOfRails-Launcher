using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;

namespace LandOfRails_Launcher.Helper
{
    class Updater
    {
        private static readonly string APILatestURL = "https://api.github.com/repos/MarkenJaden/LandOfRails-Launcher/releases/latest";

        private static Update LatestUpdate;
        private static Version CurrentVersion;
        private static Version LatestVersion;
        private static bool NeedsUpdate = false;
        private static readonly string NewExe = Path.Combine(Path.GetDirectoryName(Utils.ExePath) ?? string.Empty, "LandOfRails-Launcher.exe");

        public static async Task<bool> CheckForUpdate()
        {
            var resp = await Http.HttpClient.GetAsync(APILatestURL);
            var body = await resp.Content.ReadAsStringAsync();
            LatestUpdate = JsonConvert.DeserializeObject<Update>(body);

            LatestVersion = new Version(LatestUpdate.tag_name);
            CurrentVersion = new Version(App.Version);

            return (LatestVersion > CurrentVersion);
        }

        public static async Task Run()
        {
            if (Path.GetFileName(Utils.ExePath).Equals("LandOfRails-Launcher.old.exe")) RunNew();
            try
            {
                NeedsUpdate = await CheckForUpdate();
            }
            catch (Exception e)
            {
                //Utils.SendNotify((string)Application.Current.FindResource("Updater:CheckFailed"));
                Console.WriteLine("Update Check failed.");
                Console.WriteLine(e);
            }

            if (NeedsUpdate) await StartUpdate();
        }

        public static async Task StartUpdate()
        {
            string OldExe = Path.Combine(Path.GetDirectoryName(Utils.ExePath), "LandOfRails-Launcher.old.exe");
            string DownloadLink = null;

            foreach (Update.Asset asset in LatestUpdate.assets)
            {
                if (asset.name.Equals("LandOfRails-Launcher.exe"))
                {
                    DownloadLink = asset.browser_download_url;
                }
            }

            if (string.IsNullOrEmpty(DownloadLink))
            {
                //Utils.SendNotify((string)Application.Current.FindResource("Updater:DownloadFailed"));
                Console.WriteLine("Download of new version failed.");
            }
            else
            {
                if (File.Exists(OldExe))
                {
                    File.Delete(OldExe);
                }

                File.Move(Utils.ExePath, OldExe);

                await Utils.Download(DownloadLink, NewExe);
                RunNew();
            }
        }

        private static void RunNew()
        {
            Process.Start(NewExe);
            Application.Current.Dispatcher.Invoke(() => {
                //string batchCommands = string.Empty;
                //string exeFileName = Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", string.Empty).Replace("/", "\\").Replace(".exe", ".old.exe");

                //batchCommands += "@ECHO OFF\n";                         // Do not show any output
                //batchCommands += "ping 127.0.0.1 > nul\n";              // Wait approximately 4 seconds (so that the process is already terminated)
                //batchCommands += "echo j | del /F ";                    // Delete the executeable
                //batchCommands += exeFileName + "\n";
                //batchCommands += "echo j | del deleteMyProgram.bat";    // Delete this bat file

                //File.WriteAllText("deleteMyProgram.bat", batchCommands);

                //Process.Start("deleteMyProgram.bat");

                Process.Start(new ProcessStartInfo()
                {
                    Arguments = "/C choice /C Y /N /D Y /T 3 & Del \"" + Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", string.Empty).Replace("/", "\\").Replace(".exe", ".old.exe") + "\"",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    FileName = "cmd.exe"
                });

                Application.Current.Shutdown(); });
        }
    }

    public class Update
    {
        public string url;
        public string assets_url;
        public string upload_url;
        public string html_url;
        public int id;
        public string node_id;
        public string tag_name;
        public string target_commitish;
        public string name;
        public bool draft;
        public User author;
        public bool prerelease;
        public string created_at;
        public string published_at;
        public Asset[] assets;
        public string tarball_url;
        public string zipball_url;
        public string body;

        public class Asset
        {
            public string url;
            public int id;
            public string node_id;
            public string name;
            public string label;
            public User uploader;
            public string content_type;
            public string state;
            public int size;
            public string created_at;
            public string updated_at;
            public string browser_download_url;
        }

        public class User
        {
            public string login;
            public int id;
            public string node_id;
            public string avatar_url;
            public string gravatar_id;
            public string url;
            public string html_url;
            public string followers_url;
            public string following_url;
            public string gists_url;
            public string starred_url;
            public string subscriptions_url;
            public string organizations_url;
            public string repos_url;
            public string events_url;
            public string received_events_url;
            public string type;
            public bool site_admin;

        }
    }
}
