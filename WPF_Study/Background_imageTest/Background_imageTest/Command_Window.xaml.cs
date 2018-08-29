using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Background_imageTest
{
    /// <summary>
    /// Interaction logic for Command_Window.xaml
    /// </summary>
    public partial class Command_Window : Window
    {
        System.Timers.Timer Processing_Timer;
        Timer Command_Timer;
        ServerClass Server;
        public Command_Window()
        {
            InitializeComponent();
            Server = new ServerClass();
            /* Start ProcessDataBuffer Timer */
            /* Start ProcessCommandDataBuffer Timer */
            StartProcessDataTimer();
            StartProcessCommandTimer();
        }
        public void StartProcessDataTimer()
        {
            Processing_Timer = new System.Timers.Timer(500);  // Checking for evey 0.5 Sec whether any Client Connected or not.
            Processing_Timer.Elapsed += new ElapsedEventHandler(Server.ProcessDataBuffer);
            Processing_Timer.AutoReset = true;
            Processing_Timer.Enabled = true;
        }
        public void StartProcessCommandTimer()
        {
            Command_Timer = new System.Timers.Timer(100);  // Checking for evey 0.1 Sec whether any Client Connected or not.
            Command_Timer.Elapsed += new ElapsedEventHandler(Server.ProcessCommand_Buffer);
            Command_Timer.AutoReset = true;
            Command_Timer.Enabled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            KeyCode_CMd Cmd = new KeyCode_CMd();
            Cmd.Owner = Window.GetWindow(this);
            Cmd.ShowDialog(); // Show the KeyCode Command Window
        }
        protected override void OnClosed(EventArgs e) // On Closing the MainWindow We Should Close the Application Also.
        {
            if (!Global.Is_ClosedbySW) // It has to close the entire Application whenevr Manually closes the Mainwindow.
            {
                base.OnClosed(e);
                Application.Current.Shutdown();
            }

        }
    }
}
