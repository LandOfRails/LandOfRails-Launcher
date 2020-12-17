using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using Serilog;

namespace LandOfRailsLauncher.Helper
{
    class Updater
    {
        private static readonly string APILatestURL = "https://api.github.com/repos/LandOfRails/LandOfRails-Launcher/releases/latest";

        private static Update LatestUpdate;
        private static Version CurrentVersion;
        private static Version LatestVersion;
        private static bool NeedsUpdate = false;
        private static readonly string NewExe = Path.Combine(Path.GetDirectoryName(Utils.ExePath) ?? string.Empty, "LandOfRails-Launcher.exe");
        private static readonly string OldExe = Path.Combine(Path.GetDirectoryName(Utils.ExePath), "LandOfRails-Launcher.old.exe");

        public static async Task<bool> CheckForUpdate()
        {
            try
            {
                var resp = await Http.HttpClient.GetAsync(APILatestURL);
                var body = await resp.Content.ReadAsStringAsync();
                LatestUpdate = JsonConvert.DeserializeObject<Update>(body);

                LatestVersion = new Version(LatestUpdate.tag_name.Substring(1));
                CurrentVersion = new Version(App.Version);
            }
            catch (Exception e)
            {
                Log.Error("CheckForUpdate", e);
            }
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
                Log.Error("CheckForUpdate", e);
            }

            if (NeedsUpdate) await StartUpdate();
            else if (File.Exists(Path.Combine(Path.GetDirectoryName(Utils.ExePath), "LandOfRails-Launcher.old.exe")))
            {
                try
                {
                    File.Delete(OldExe);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Old file delete failed.");
                    Console.WriteLine(e);
                    Log.Error("Delete old exe", e);
                }
            }
        }

        public static async Task StartUpdate()
        {
            try
            {
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
            catch (Exception e)
            {
                Log.Error("StartUpdate", e);
            }
        }

        private static void RunNew()
        {
            try
            {
                Process.Start(NewExe);
                Application.Current.Dispatcher.Invoke(() => { Application.Current.Shutdown(); });
            }
            catch (Exception e)
            {
                Log.Error("RunNew", e);
            }
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
