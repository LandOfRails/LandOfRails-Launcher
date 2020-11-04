using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LandOfRails_Launcher.Models;
using LandOfRails_Launcher.Properties;
using Microsoft.VisualBasic.Devices;
using Path = System.IO.Path;

namespace LandOfRails_Launcher.Window
{
    /// <summary>
    /// Interaktionslogik für OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : System.Windows.Window
    {
        private int oldRAM = Properties.Settings.Default.RAM;
        public OptionsWindow()
        {
            InitializeComponent();
            Settings.Default.Upgrade();
            ramSlider.Maximum = new ComputerInfo().TotalPhysicalMemory / 1e+6 - 2000;
            ramSlider.Value = Properties.Settings.Default.RAM;
            consoleCheckBox.IsChecked = Settings.Default.openConsole;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void RamSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ramBox.Text = e.NewValue.ToString();
            writeRAM();
        }

        private void RamBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!ramBox.Text.Any())
            {
                ramBox.Text = 0.ToString();
            }
            var newValue = Math.Round(Convert.ToDouble(ramBox.Text));
            if (newValue > ramSlider.Maximum)
            {
                newValue = Convert.ToInt32(Math.Round(ramSlider.Maximum));
            } else if (newValue < 0)
            {
                newValue = 0;
            }

            ramBox.Text = newValue.ToString();
            ramSlider.Value = newValue;
            writeRAM();
        }

        private void writeRAM()
        {
            Properties.Settings.Default.RAM = Convert.ToInt32(ramSlider.Value);
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void OKButton_OnClick(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
            this.Close();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.RAM = oldRAM;
            this.Close();
        }

        private void DeleteCredentials_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result =
                MessageBox.Show(
                    "Dies wird alle gespeicherten Login Daten löschen. Diesen Schritt kann man nicht rückgängig machen.",
                    "Bist du sicher?", MessageBoxButton.OKCancel);
            if (result != MessageBoxResult.OK) return;
            Settings.Default.EMail = "null";
            Settings.Default.Password = "null";
            Settings.Default.Save();
        }

        private void ConsoleCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            Settings.Default.openConsole = true;
        }

        private void ConsoleCheckBox_OnUnchecked(object sender, RoutedEventArgs e)
        {
            Settings.Default.openConsole = false;
        }
    }
}
