using System;
using System.IO;
using System.Windows;
using LandOfRailsLauncher.Helper;
using LandOfRailsLauncher.Properties;
using LandOfRailsLauncher.Window;

namespace LandOfRailsLauncher
{
    /// <summary>
    /// Interaktionslogik für LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow
    {
        private bool remember;
        private readonly string path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "LandOfRails Launcher");
        public LoginWindow()
        {
            //AutoUpdater.Start("https://launcher.landofrails.net/launcher/version.xml");
            InitializeComponent();
            if (Static.login.autoLogin()) ShowMainWindow();
            else if (!Settings.Default.EMail.Equals("null") && !Settings.Default.Password.Equals("null"))
            {
                if (Static.login.login(Settings.Default.EMail, Settings.Default.Password))
                {
                    ShowMainWindow();
                }
                else
                {
                    Wrong.Opacity = 1;
                }
            }
        }

        public void ShowMainWindow()
        {
            var win = new MainWindow();
            Close();
            win.Show();
        }

        private void LoginButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (Static.login.login(EMailBox.Text, PasswordBox.Password))
            {
                if (remember)
                {
                    //SAVE PASSWORD AND EMAIL LOCAL
                    //byte[] plainTextEmail = Encoding.UTF8.GetBytes(EMailBox.Text);
                    //byte[] plainTextPassword = Encoding.UTF8.GetBytes(PasswordBox.Password);

                    //byte[] entropy = new byte[20];
                    //using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                    //{
                    //    rng.GetBytes(entropy);
                    //}
                    //using (StreamWriter sw = File.CreateText(Path.Combine(path, "login")))
                    //{
                    //    sw.WriteLine(EMailBox.Text);
                    //    sw.WriteLine(PasswordBox.Password);
                    //    sw.Flush();
                    //    sw.Close();
                    //}

                    Settings.Default.Upgrade();
                    Settings.Default.EMail = EMailBox.Text;
                    Settings.Default.Password = PasswordBox.Password;
                    Settings.Default.Save();
                }
                ShowMainWindow();
            }
            else Wrong.Opacity = 1;
        }

        private void RembemberLoginCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            //MessageBoxResult result = MessageBox.Show("Dies wird die verwendete E-Mail Adresse sowie das Passwort lokal (aktuell noch in Klartext, ohne Verschlüsselung) auf deinem Computer speichern. Dies kann ein Sicherheitsrisiko für deinen Account darstellen. Mehr Informationen findest du auf WEBSEITE. \n\nMöchtest du dies wirklich lokal abspeichern? (Jederzeit in den Einstellungen änderbar.)"/*WEBSEITE DAZU ERSTELLEN*/, "Security alert", MessageBoxButton.YesNo);
            //if (result == MessageBoxResult.Yes)
            //{
                remember = true;
            //}
            //else RembemberLoginCheckBox.IsChecked = false;
        }

        private void RembemberLoginCheckBox_OnUnchecked(object sender, RoutedEventArgs e)
        {
            remember = false;
        }
    }
}
