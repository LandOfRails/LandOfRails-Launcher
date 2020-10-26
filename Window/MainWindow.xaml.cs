using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using LandOfRails_Launcher.Helper;
using LandOfRails_Launcher.Models;
using LandOfRails_Launcher.Window;
using Newtonsoft.Json;
using DiscordRPC = LandOfRails_Launcher.Helper.DiscordRPC;
using Path = System.IO.Path;

namespace LandOfRails_Launcher
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private string path;
        private Modpack currentSelectedModpack;
        private Dictionary<Modpack, BitmapImage> images = new Dictionary<Modpack, BitmapImage>();
        Helper.DiscordRPC discord;
        public MainWindow()
        {
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "LandOfRails Launcher");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            InitializeComponent();
            //update.checkForUpdates();
            RefreshListAsync();
            modpackList.ItemsSource = Static.Modpacks;
            progressBar.DataContext = Static.login;
            progressLabel.DataContext = Static.login;
            startButton.DataContext = Static.login;
            startButton.Content = "Starten";

            ModpackImage.Source = images[Static.Modpacks[0]];

            discord = new Helper.DiscordRPC();
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Modpack modpack in modpackList.SelectedItems)
            {
                Static.login.start(modpack);
                break;
            }
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
#pragma warning disable 4014
            RefreshListAsync();
#pragma warning restore 4014
            modpackList.ItemsSource = Static.Modpacks;
            discord.SetIdle();
            //Check for Launcher updates
        }

        private async Task RefreshListAsync()
        {
            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString("https://launcher.landofrails.net/ModpackList.json");
                Static.Modpacks = JsonConvert.DeserializeObject<List<Modpack>>(json);
            }

            Static.Modpacks = Static.Modpacks.OrderBy(t => t.Organisation).ToList();
            foreach (var modpack in Static.Modpacks)
            {
                images.Add(modpack, new BitmapImage(new Uri(modpack.ImageUrl)));
                if (Static.login.isDownloaded(modpack))
                {
                    modpack.CurrentVersion = Static.login.getCurrentVersion(modpack);
                    if (!Static.login.updateAvailable(modpack))
                    {
                        modpack.ModpackVersion = String.Empty;
                    }
                }
                else
                {
                    modpack.CurrentVersion = modpack.ModpackVersion;
                    modpack.ModpackVersion = String.Empty;
                }
            }
        }

        private void ModpackList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (Modpack modpack in modpackList.SelectedItems)
            {
                ModpackImage.Source = images[modpack];
                currentSelectedModpack = modpack;
                startButton.Content = Static.login.isDownloaded(modpack) ? "Starten" : "Herunterladen";
                break;
            }
        }
        private void OpenFolder_OnClick(object sender, RoutedEventArgs e)
        {
            if (Static.login.isDownloaded(currentSelectedModpack))
                Process.Start(Path.Combine(path, currentSelectedModpack.Name));
            else MessageBox.Show("Dafür musst du zuerst das Modpack herunterladen ^^");
        }

        private void OpenCrashLogs_OnClick(object sender, RoutedEventArgs e)
        {
            if (Static.login.isDownloaded(currentSelectedModpack))
            {
                Process.Start(Path.Combine(path, currentSelectedModpack.Name));
            }
        }

        private void DeleteModpack_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result =
                MessageBox.Show(
                    "Warnung! Es werden dabei alle Dateien, Einstellungen und Welten von dem Modpack " +
                    currentSelectedModpack.Title + " gelöscht.",
                    "Delete Confirmation", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes) return;
            Directory.Delete(Path.Combine(path, currentSelectedModpack.Name),true);
            MessageBox.Show(currentSelectedModpack.Title + " wurde erfolgreich gelöscht.");
            startButton.Content = "Herunterladen";
        }

        private void ReinstallModpack_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result =
                MessageBox.Show(
                    "Warnung! Es werden dabei alle Dateien, Einstellungen und Welten von dem Modpack " +
                    currentSelectedModpack.Title + " gelöscht.",
                    "Reinstall Confirmation", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes) return;
            Directory.Delete(Path.Combine(path, currentSelectedModpack.Name),true);
            Static.login.start(currentSelectedModpack);
        }

        private void OptionalMods_OnClick(object sender, RoutedEventArgs e)
        {
            //Open Window to edit optional mods
            MessageBox.Show("Noch nicht implementiert.");
        }

        private void OptionsButton_OnClick(object sender, RoutedEventArgs e)
        {
            var options = new OptionsWindow();
            options.Owner = this;
            options.ShowInTaskbar = false;
            options.Show();
        }
    }
}