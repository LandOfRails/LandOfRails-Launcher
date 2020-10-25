﻿using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows;
using ICSharpCode.SharpZipLib.Core;
using Ionic.Zip;
using LandOfRails_Launcher.MinecraftLaunch;
using LandOfRails_Launcher.MinecraftLaunch.Core;
using LandOfRails_Launcher.Models;
using Microsoft.VisualBasic.Devices;
using Newtonsoft.Json;

namespace LandOfRails_Launcher.Helper
{
    internal class StartHandler : BaseViewModel
    {
        private bool _progressIntermediate;
        private string _progressLabel;
        private int _progressMaxValue;
        private int _progressValue;
        private string _startButton;
        private bool _startButtonEnabled;
        private string compressedFileName; //the name of the file being extracted
        private long compressedSize; //the size of a single compressed file
        private long extractedSizeTotal; //the bytes total extracted

        private readonly BackgroundWorker extractFile;
        private readonly BackgroundWorker extractVersionFile;
        private readonly BackgroundWorker downloadMinecraftStuff;
        private long fileSize; //the size of the zip file
        private Modpack modpack;

        private readonly string path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "LandOfRails Launcher");

        private MSession session;
        private MProfile profile;

        public StartHandler()
        {
            startButtonEnabled = true;

            progressMaxValue = 100;

            extractFile = new BackgroundWorker();
            extractFile.DoWork += ExtractFile_DoWork;
            extractFile.ProgressChanged += ExtractFile_ProgressChanged;
            extractFile.RunWorkerCompleted += ExtractFile_RunWorkerCompleted;
            extractFile.WorkerReportsProgress = true;

            extractVersionFile = new BackgroundWorker();
            extractVersionFile.DoWork += extractVersionFile_DoWork;
            extractVersionFile.ProgressChanged += ExtractFile_ProgressChanged;
            extractVersionFile.RunWorkerCompleted += extractVersionFile_RunWorkerCompleted;
            extractVersionFile.WorkerReportsProgress = true;

            downloadMinecraftStuff = new BackgroundWorker();
            downloadMinecraftStuff.DoWork += downloadMinecraftStuff_DoWork;
            downloadMinecraftStuff.RunWorkerCompleted += downloadMinecraftStuff_RunWorkerCompleted;
            downloadMinecraftStuff.WorkerReportsProgress = true;
        }

        private void downloadMinecraftStuff_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            startButtonEnabled = true;
        }

        private void downloadMinecraftStuff_DoWork(object sender, DoWorkEventArgs e)
        {
            startButtonEnabled = false;
            var minecraft = new Minecraft(Path.Combine(path, modpack.Name));
            minecraft.SetAssetsPath(path);

            var launcher = new Launcher(minecraft);
            launcher.ProgressChanged += Downloader_ChangeProgress;
            launcher.FileChanged += Downloader_ChangeFile;

            launcher.UpdateProfiles();

            progressLabel = "Download der Assets abgeschlossen.";

            MJava java = new MJava();
            var javaBinary = java.CheckJava();

            var option = new MLaunchOption()
            {
                GameLauncherName = "LandOfRailsLauncher",
                GameLauncherVersion = "0.1",
                JavaPath = javaBinary,
                Session = session,
                StartProfile = profile,

                MaximumRamMb = Properties.Settings.Default.RAM,
            };

            string forgeVersion;
            using (StreamReader r = new StreamReader(Path.Combine(path, modpack.Name, "bin", "version.txt")))
            {
                forgeVersion = r.ReadLine();
            }

            var process = launcher.CreateProcess(modpack.MinecraftVersion, forgeVersion, option);
            process.Start();
        }

        public class Version
        {
            public string id { get; set; }
        }

        private void extractVersionFile_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            File.Delete(Path.Combine(path, modpack.Name, "modpack.zip"));
            string id;
            using (StreamReader r = new StreamReader(Path.Combine(path, modpack.Name, "bin", "version.json")))
            {
                string json = r.ReadToEnd();
                id = GetFirstInstance<string>("id", json);
            }

            id = id.ToLower();
            id = id.Replace(modpack.MinecraftVersion+"-forge","");
            id = id.Replace(modpack.MinecraftVersion+"-", "");

            using (StreamWriter sw = File.CreateText(Path.Combine(path, modpack.Name, "bin", "version.txt")))
            {
                sw.WriteLine(id);
            }

            progressIntermediate = false;
            progressValue = 0;
            progressLabel = modpack.Title + " ist startklar! =)";
            startButtonEnabled = true;
            progressValue = 100;
            progressMaxValue = 100;
            startButton = "Starten";
            MessageBox.Show("Modpack heruntergeladen!");
        }

        public T GetFirstInstance<T>(string propertyName, string json)
        {
            using (var stringReader = new StringReader(json))
            using (var jsonReader = new JsonTextReader(stringReader))
            {
                while (jsonReader.Read())
                {
                    if (jsonReader.TokenType == JsonToken.PropertyName
                        && (string)jsonReader.Value == propertyName)
                    {
                        jsonReader.Read();

                        var serializer = new JsonSerializer();
                        return serializer.Deserialize<T>(jsonReader);
                    }
                }
                return default(T);
            }
        }

        private void extractVersionFile_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var fileName = Path.Combine(path, modpack.Name, "bin", "modpack.jar");
                var extractPath = Path.Combine(path, modpack.Name, "bin");

                //get the size of the zip file
                var fileInfo = new FileInfo(fileName);
                fileSize = fileInfo.Length;
                using (var zipFile = ZipFile.Read(fileName))
                {
                    //reset the bytes total extracted to 0
                    extractedSizeTotal = 0;
                    var fileAmount = zipFile.Count;
                    var fileIndex = 0;
                    zipFile.ExtractProgress += Zip_ExtractVersionProgress;
                    foreach (var ZipEntry in zipFile)
                    {
                        fileIndex++;
                        compressedFileName = "(" + fileIndex + "/" + fileAmount + "): " + ZipEntry.FileName;
                        //get the size of a single compressed file
                        compressedSize = ZipEntry.CompressedSize;
                        if (ZipEntry.FileName.Equals("version.json"))
                        {
                            ZipEntry.Extract(extractPath, ExtractExistingFileAction.OverwriteSilently);
                            break;
                        }

                        //calculate the bytes total extracted
                        extractedSizeTotal += compressedSize;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public bool progressIntermediate
        {
            get => _progressIntermediate;
            set => SetProperty(ref _progressIntermediate, value);
        }

        public bool startButtonEnabled
        {
            get => _startButtonEnabled;
            set => SetProperty(ref _startButtonEnabled, value);
        }

        public string progressLabel
        {
            get => _progressLabel;
            set => SetProperty(ref _progressLabel, value);
        }

        public int progressValue
        {
            get => _progressValue;
            set => SetProperty(ref _progressValue, value);
        }

        public int progressMaxValue
        {
            get => _progressMaxValue;
            set => SetProperty(ref _progressMaxValue, value);
        }

        public string startButton
        {
            get => _startButton;
            set => SetProperty(ref _startButton, value);
        }

        public bool autoLogin()
        {
            var login = new MLogin();
            var sessionAuto = login.TryAutoLogin();
            if (sessionAuto.Result != MLoginResult.Success) return false;

            session = sessionAuto;
            return true;
        }

        public bool login(string mail, string password)
        {
            var login = new MLogin();
            session = login.Authenticate(mail, password);

            return session.Result == MLoginResult.Success;
        }

        public bool start(Modpack modpack)
        {
            this.modpack = modpack;
            if (!isDownloaded(modpack)) //Gibt es das Modpack?
                downloadModpack();
            else if (updateAvailable(modpack)) //Ist ein Update da?
                switch (MessageBox.Show("Es ist ein Update verfügbar. Möchtest du es herunterladen?", "Update",
                    MessageBoxButton.YesNo, MessageBoxImage.Question))
                {
                    case MessageBoxResult.Yes:
                        downloadModpack();
                        break;
                    case MessageBoxResult.No:
                        StartSession();
                        break;
                    default:
                        return false;
                }
            else 
                StartSession();

            return true;
        }

        private void StartSession()
        {
            downloadMinecraftStuff.RunWorkerAsync();
        }

        private void Downloader_ChangeProgress(object sender, ProgressChangedEventArgs e)
        {
            progressValue = e.ProgressPercentage;
        }

        private void Downloader_ChangeFile(DownloadFileChangedEventArgs e)
        {
            progressLabel = "[" + e.ProgressedFileCount + "/" + e.TotalFileCount + "] " + e.FileName;
        }

        public bool updateAvailable(Modpack modpack)
        {
            var currentVersion = Convert.ToDouble(getCurrentVersion(modpack));
            return Convert.ToDouble(modpack.ModpackVersion) > currentVersion;
        }

        public string getCurrentVersion(Modpack modpack)
        {
            return File.ReadAllText(Path.Combine(path, modpack.Name, "version"));
        }

        public bool isDownloaded(Modpack modpack)
        {
            return Directory.Exists(Path.Combine(path, modpack.Name));
        }

        private void downloadModpack()
        {
            startButtonEnabled = false;
            startButton = "Herunterladen...";
            Directory.CreateDirectory(Path.Combine(path, modpack.Name));
            using (var file = File.CreateText(Path.Combine(path, modpack.Name, "version")))
            {
                file.Write(modpack.CurrentVersion);
                file.Flush();
                file.Close();
            }
            using (var webClient = new WebClient())
            {
                webClient.DownloadProgressChanged += ProgressChanged;
                webClient.DownloadFileCompleted += unzipFile;
                webClient.DownloadFileAsync(new Uri(modpack.LocationOnServer),
                    Path.Combine(path, modpack.Name, "modpack.zip"));
            }
        }

        private void unzipFile(object sender, AsyncCompletedEventArgs e) //Start unzip
        {
            startButton = "Entpacken...";
            progressMaxValue = int.MaxValue;
            extractFile.RunWorkerAsync();
        }

        private void ProgressHandler(object o, ProgressEventArgs args)
        {
            progressValue = Convert.ToInt32(args.PercentComplete);
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressValue = e.ProgressPercentage;
            progressLabel = Math.Round(e.BytesReceived / 1e+6, 3) + " MB von " +
                            Math.Round(e.TotalBytesToReceive / 1e+6, 3) + " MB heruntergeladen...";
        }

        private void ExtractFile_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) //When finished unzipping
        {
            extractVersionFile.RunWorkerAsync();
        }

        private void ExtractFile_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressLabel = compressedFileName;

            //calculate the totalPercent
            var totalPercent = (e.ProgressPercentage * compressedSize + extractedSizeTotal * int.MaxValue) / fileSize;
            if (totalPercent > int.MaxValue)
                totalPercent = int.MaxValue;
            progressValue = (int) totalPercent;
        }

        private void ExtractFile_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var fileName = Path.Combine(path, modpack.Name, "modpack.zip");
                var extractPath = Path.Combine(path, modpack.Name);

                //get the size of the zip file
                var fileInfo = new FileInfo(fileName);
                fileSize = fileInfo.Length;
                using (var zipFile = ZipFile.Read(fileName))
                {
                    //reset the bytes total extracted to 0
                    extractedSizeTotal = 0;
                    var fileAmount = zipFile.Count;
                    var fileIndex = 0;
                    zipFile.ExtractProgress += Zip_ExtractProgress;
                    foreach (var ZipEntry in zipFile)
                    {
                        fileIndex++;
                        compressedFileName = "(" + fileIndex + "/" + fileAmount + "): " + ZipEntry.FileName;
                        //get the size of a single compressed file
                        compressedSize = ZipEntry.CompressedSize;
                        ZipEntry.Extract(extractPath, ExtractExistingFileAction.OverwriteSilently);
                        //calculate the bytes total extracted
                        extractedSizeTotal += compressedSize;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Zip_ExtractProgress(object sender, ExtractProgressEventArgs e)
        {
            if (e.TotalBytesToTransfer <= 0) return;
            var percent = e.BytesTransferred * int.MaxValue / e.TotalBytesToTransfer;
            //Console.WriteLine("Indivual: " + percent);
            extractFile.ReportProgress((int) percent);
        }
        private void Zip_ExtractVersionProgress(object sender, ExtractProgressEventArgs e)
        {
            if (e.TotalBytesToTransfer <= 0) return;
            var percent = e.BytesTransferred * int.MaxValue / e.TotalBytesToTransfer;
            //Console.WriteLine("Indivual: " + percent);
            extractVersionFile.ReportProgress((int)percent);
        }
    }
}