using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using LandOfRails_Launcher.Helper;

namespace LandOfRails_Launcher
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        public static string Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }
}
