using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using LandOfRailsLauncher.Helper;
using LandOfRailsLauncher.Models;
using Newtonsoft.Json;
using Serilog;
using Path = System.IO.Path;

namespace LandOfRailsLauncher.Window
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private string path;
        private Modpack currentSelectedModpack;
        private Dictionary<Modpack, BitmapImage> images = new Dictionary<Modpack, BitmapImage>();
        //Helper.DiscordRPC discord;

        private int lastSelected = 999;

        public MainWindow()
        {
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "LandOfRails Launcher");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            InitializeComponent();
#pragma warning disable 4014
            RefreshListAsync();
#pragma warning restore 4014
            modpackList.ItemsSource = Static.Modpacks;
            progressBar.DataContext = Static.login;
            progressLabel.DataContext = Static.login;
            startButton.DataContext = Static.login;
            startButton.Content = "Starten";

            ModpackImage.Source = images[Static.Modpacks[0]];

            Title = "LandOfRails Launcher v" + Assembly.GetExecutingAssembly().GetName().Version;
            
            Log.Information("Init main window completed.");

            //discord = new Helper.DiscordRPC();
            //discord.SetIdle();
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
#pragma warning disable 4014
            RefreshListAsync();
#pragma warning restore 4014
            foreach (Modpack modpack in modpackList.SelectedItems)
            {
                Static.login.start(modpack);
                break;
            }
#pragma warning disable 4014
            RefreshListAsync();
#pragma warning restore 4014
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
#pragma warning disable 4014
            RefreshListAsync();
#pragma warning restore 4014
            //discord.SetIdle();
            //Check for Launcher updates
        }

        private async Task RefreshListAsync()
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    var json = wc.DownloadString("https://launcher.landofrails.net/ModpackList.json");
                    Static.Modpacks = JsonConvert.DeserializeObject<List<Modpack>>(json);
                }

                Static.Modpacks = Static.Modpacks.OrderBy(t => t.Organisation).ToList();
                foreach (var modpack in Static.Modpacks)
                {
                    //Downloaded Icon stuff
                    if (!Static.login.isDownloaded(modpack))
                        modpack.DownloadedImage = "https://image.flaticon.com/icons/png/512/0/532.png";

                    //Background Image stuff
                    images.Add(modpack, new BitmapImage(new Uri(modpack.ImageUrl)));
                    if (Static.login.isDownloaded(modpack))
                    {
                        modpack.CurrentVersion = Static.login.getCurrentVersion(modpack);
                        if (!Static.login.UpdateAvailable(modpack))
                        {
                            modpack.ModpackVersion = string.Empty;
                        }
                    }
                    else
                    {
                        modpack.CurrentVersion = modpack.ModpackVersion;
                        modpack.ModpackVersion = string.Empty;
                    }
                }

                modpackList.ItemsSource = Static.Modpacks;
                if (lastSelected != 999)
                {
                    modpackList.SelectedIndex = lastSelected;
                }

            }
            catch (Exception e)
            {
                Log.Error("RefreshListAsync", e);
            }
        }

        private void ModpackList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                foreach (Modpack modpack in modpackList.SelectedItems)
                {
                    ModpackImage.Source = images[modpack];
                    currentSelectedModpack = modpack;
                    startButton.Content = Static.login.isDownloaded(modpack) ? "Starten" : "Herunterladen";
                    lastSelected = modpackList.SelectedIndex;
                    break;
                }
            }
            catch (Exception exception)
            {
                Log.Error("Error selecting Modpack", exception);
            }
        }
        private void OpenFolder_OnClick(object sender, RoutedEventArgs e)
        {
            if (Static.login.isDownloaded(currentSelectedModpack))
                Process.Start(Path.Combine(path, currentSelectedModpack.Name));
            else MessageBox.Show("Dafür musst du zuerst das Modpack herunterladen ^^");
#pragma warning disable 4014
            RefreshListAsync();
#pragma warning restore 4014
        }

        private void OpenCrashLogs_OnClick(object sender, RoutedEventArgs e)
        {
            if (Static.login.isDownloaded(currentSelectedModpack))
            {
                if (Directory.Exists(Path.Combine(path, currentSelectedModpack.Name, "crash-reports")))
                    Process.Start(Path.Combine(path, currentSelectedModpack.Name, "crash-reports"));
                else MessageBox.Show("Du hast noch keine Crash-Logs :)");
            } else MessageBox.Show("Dafür musst du zuerst das Modpack herunterladen ^^");
#pragma warning disable 4014
            RefreshListAsync();
#pragma warning restore 4014
        }

        private void DeleteModpack_OnClick(object sender, RoutedEventArgs e)
        {
            if (Static.login.isDownloaded(currentSelectedModpack))
            {
                MessageBoxResult result =
                    MessageBox.Show(
                        "Warnung! Es werden dabei alle Dateien, Einstellungen und Welten von dem Modpack " +
                        currentSelectedModpack.Title + " gelöscht.",
                        "Delete Confirmation", MessageBoxButton.YesNo);
                if (result != MessageBoxResult.Yes) return;
                Directory.Delete(Path.Combine(path, currentSelectedModpack.Name), true);
                MessageBox.Show(currentSelectedModpack.Title + " wurde erfolgreich gelöscht.");
                startButton.Content = "Herunterladen";
            } else MessageBox.Show("Dafür musst du zuerst das Modpack herunterladen ^^");
#pragma warning disable 4014
            RefreshListAsync();
#pragma warning restore 4014
        }

        private void ReinstallModpack_OnClick(object sender, RoutedEventArgs e)
        {
            if (Static.login.isDownloaded(currentSelectedModpack))
            {
                MessageBoxResult result =
                    MessageBox.Show(
                        "Warnung! Es werden dabei alle Dateien, Einstellungen und Welten von dem Modpack " +
                        currentSelectedModpack.Title + " gelöscht.",
                        "Reinstall Confirmation", MessageBoxButton.YesNo);
                if (result != MessageBoxResult.Yes) return;
                Directory.Delete(Path.Combine(path, currentSelectedModpack.Name), true);
                Static.login.start(currentSelectedModpack);
            } else MessageBox.Show("Dafür musst du zuerst das Modpack herunterladen ^^");
#pragma warning disable 4014
            RefreshListAsync();
#pragma warning restore 4014
        }

        private void OptionalMods_OnClick(object sender, RoutedEventArgs e)
        {
            //Open Window to edit optional mods
            MessageBox.Show("Noch nicht implementiert.");
#pragma warning disable 4014
            RefreshListAsync();
#pragma warning restore 4014
        }

        private void OptionsButton_OnClick(object sender, RoutedEventArgs e)
        {
            var options = new OptionsWindow {Owner = this, ShowInTaskbar = false};
            options.Show();
#pragma warning disable 4014
            RefreshListAsync();
#pragma warning restore 4014
        }
    }
}