using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace LandOfRailsLauncher.Window
{
    /// <summary>
    /// Interaktionslogik für ProcessWindow.xaml
    /// </summary>
    public partial class ProcessWindow
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
            consoleText.Text += "\n" + minecraftProcess;
        }
        private void KillButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (minecraftProcess == null)
            {
                Close();
                return;
            }
            if (!minecraftProcess.HasExited)
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
