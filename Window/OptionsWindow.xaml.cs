using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LandOfRailsLauncher.Properties;
using Microsoft.VisualBasic.Devices;

namespace LandOfRailsLauncher.Window
{
    /// <summary>
    /// Interaktionslogik für OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow
    {
        private int oldRAM = Settings.Default.RAM;
        public OptionsWindow()
        {
            InitializeComponent();
            Settings.Default.Upgrade();
            ramSlider.Maximum = new ComputerInfo().TotalPhysicalMemory / 1e+6 - 3000;
            ramSlider.Value = Settings.Default.RAM;
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
            Settings.Default.RAM = Convert.ToInt32(ramSlider.Value);
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            Settings.Default.Save();
        }

        private void OKButton_OnClick(object sender, RoutedEventArgs e)
        {
            Settings.Default.Save();
            Close();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            Settings.Default.RAM = oldRAM;
            Close();
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
