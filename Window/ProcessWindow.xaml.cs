using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace LandOfRails_Launcher.Window
{
    /// <summary>
    /// Interaktionslogik für ProcessWindow.xaml
    /// </summary>
    public partial class ProcessWindow : System.Windows.Window
    {
        private Process minecraftProcess;
        public ProcessWindow()
        {
            InitializeComponent();
        }

        public void Start(Process process)
        {
            minecraftProcess = process;

            minecraftProcess.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
            minecraftProcess.ErrorDataReceived += new DataReceivedEventHandler(OutputHandler);

            minecraftProcess.Start();
            minecraftProcess.BeginOutputReadLine();
            minecraftProcess.WaitForExit();
        }
        private void KillButton_OnClick(object sender, RoutedEventArgs e)
        {
            if(!minecraftProcess.HasExited)
                minecraftProcess.Kill();
        }

        private void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
            {
                consoleText.Text += "\n" + outLine.Data;
                ScrollViewer.ScrollToBottom();
            }));
        }
    }
}
