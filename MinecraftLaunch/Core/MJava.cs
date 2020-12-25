using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using LandOfRailsLauncher.MinecraftLaunch.Utils;
using Newtonsoft.Json.Linq;
using Serilog;

namespace LandOfRailsLauncher.MinecraftLaunch.Core
{
    public class MJava
    {
        public static string DefaultRuntimeDirectory = Path.Combine(Minecraft.GetOSDefaultPath(), "runtime");

        public event ProgressChangedEventHandler ProgressChanged;
        public event EventHandler DownloadCompleted;
        public string RuntimeDirectory { get; private set; }

        public MJava() : this(DefaultRuntimeDirectory) { }

        public MJava(string runtimePath)
        {
            RuntimeDirectory = runtimePath;
        }

        public string CheckJava()
        {
            var binaryName = "java";
            if (MRule.OSName == "windows")
                binaryName = "javaw.exe";

            return CheckJava(binaryName);
        }

        public string CheckJava(string binaryName)
        {
            var javapath = Path.Combine(RuntimeDirectory, "bin", binaryName);

            if (File.Exists(javapath)) return javapath;
            string json = "";

            var javaUrl = "";
            using (var wc = new WebClient())
            {
                json = wc.DownloadString(MojangServer.LauncherMeta);

                var job = JObject.Parse(json)[MRule.OSName];
                javaUrl = job[MRule.Arch]?["jre"]?["url"]?.ToString();
                Log.Information("Downloading java from: "+javaUrl);
                if (string.IsNullOrEmpty(javaUrl))
                {
                    Log.Error("Downloading JRE on current OS is not supported. Set JavaPath manually.");
                    throw new PlatformNotSupportedException("Downloading JRE on current OS is not supported. Set JavaPath manually.");
                }

                Directory.CreateDirectory(RuntimeDirectory);
            }

            var lzmapath = Path.Combine(Path.GetTempPath(), "jre.lzma");
            var zippath = Path.Combine(Path.GetTempPath(), "jre.zip");

            Log.Information($"Downloading to: {lzmapath}");

            var webdownloader = new WebDownload();
            webdownloader.DownloadProgressChangedEvent += Downloader_DownloadProgressChangedEvent;
            webdownloader.DownloadFile(javaUrl, lzmapath);

            DownloadCompleted?.Invoke(this, new EventArgs());

            Log.Information("Download completed. Start LZMAing...");
            try
            {
                LZMA.DecompressFile(lzmapath, zippath, (l, l1) =>
            {
                Console.WriteLine(l);
                Console.WriteLine(l1);
                Console.WriteLine();
            });
                //SevenZipWrapper.DecompressFileLZMA(lzmapath, zippath);

            }
            catch (Exception e)
            {
                Log.Error(e, "LZMA Error");
            }

            Log.Information("Start Unzipping...");

            var z = new SharpZip(zippath);
            z.ProgressEvent += Z_ProgressEvent;
            z.Unzip(RuntimeDirectory);

            if (!File.Exists(javapath))
            {
                Log.Error("Failed Download Java File exists: " + javapath);
                throw new Exception("Failed Download");
            }

            if (MRule.OSName != "windows")
                IOUtil.Chmod(javapath, IOUtil.Chmod755);

            Log.Information("Java path: " + javapath);

            return javapath;
        }

        private int downloadProgress, unzipProgress;

        private void Z_ProgressEvent(object sender, int e)
        {
            if (e != unzipProgress)
            {
                unzipProgress = e;
                Log.Information("Unzip progress: " + e);
            }

            ProgressChanged?.Invoke(this, new ProgressChangedEventArgs(e, null));
        }

        private void Downloader_DownloadProgressChangedEvent(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage != downloadProgress)
            {
                downloadProgress = e.ProgressPercentage;
                Log.Information("Download progress: " + e.ProgressPercentage + "%");
            }

            ProgressChanged?.Invoke(this, e);
        }
    }
}
