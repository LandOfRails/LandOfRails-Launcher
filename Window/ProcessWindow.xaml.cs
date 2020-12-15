using System;
using System.Diagnostics;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Threading;
using Timer = System.Timers.Timer;

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

            minecraftProcess.OutputDataReceived += OutputHandler;
            minecraftProcess.ErrorDataReceived += OutputHandler;

            minecraftProcess.Start();
            minecraftProcess.BeginOutputReadLine();
            minecraftProcess.WaitForExit();

            Timer timer = new Timer(2000);
            timer.Elapsed += TimerOnElapsed;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            consoleText.Text = text;
            ScrollViewer.ScrollToBottom();
        }

        private void KillButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (minecraftProcess == null)
            {
                Close();
                return;
            }
            if(!minecraftProcess.HasExited)
                minecraftProcess.Kill();
        }

        private String text;

        private void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
            {
                text += "\n" + outLine.Data;
            }));
        }
    }
}
