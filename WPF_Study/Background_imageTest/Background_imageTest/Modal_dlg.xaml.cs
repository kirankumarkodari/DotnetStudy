using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Timers;
namespace Background_imageTest
{
    /// <summary>
    /// Interaction logic for Modal_dlg.xaml
    /// </summary>
   
    public partial class Modal_dlg : Window
    {
        Timer aTimer;
        private void Checker(object sender, ElapsedEventArgs elapsedEventArg)
        {
           
            if(ServerClass.Sockets_arr.Count>0) // Means One Client gets Connected to Server
            {
                RoutedEventArgs e = new RoutedEventArgs();
                this.Dispatcher.Invoke(() =>
                {
                    aTimer.Enabled = false;
                    aTimer.Stop();  // Stopping the Timer 
                    aTimer.Dispose();
                    Global.Is_Manuallyclosed = false;
                    this.Close();
                });
               
            }


        }
        public Modal_dlg()
        {
            InitializeComponent();
            
            aTimer = new Timer(500);  // Checking for evey 0.5 Sec whether any Client Connected or not.
            aTimer.Elapsed += new ElapsedEventHandler(Checker);
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private void Buttoncancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                aTimer.Enabled = false;
                aTimer.Stop();  // Stopping the Timer 
                aTimer.Dispose();
                this.Close();
               
            }
            catch (Exception ex)
            {
                this.Close(); // Closing Modal.. 
            }

            
        }
        private void Mouse_entered(object sender, RoutedEventArgs e)
        {


            Button_cancel.Foreground = Brushes.DarkCyan;
        }

        private void Mouse_left(object sender, RoutedEventArgs e)
        {
            Color c = (Color)ColorConverter.ConvertFromString("#E91E63");
            Button_cancel.Foreground = new SolidColorBrush(c);
        }
    }
}
