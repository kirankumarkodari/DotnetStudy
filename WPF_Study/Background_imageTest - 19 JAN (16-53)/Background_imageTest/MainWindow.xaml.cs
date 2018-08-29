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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Text.RegularExpressions;

namespace Background_imageTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            string hostName = Dns.GetHostName();
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            ip_textBox.Text = myIP;
            ip_textBox.IsEnabled = false;
            port_textBox.Focus();  // to set the port Textbox to be Focused 
       
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string trimed_port = port_textBox.Text.Trim();
            if(trimed_port == "")
            {
                // Error Related Port Number cannot be empty.\
                MessageBox.Show("Port number cannot be empty !!", "Warning",MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else  // SOme port number has entered.
            {
                int portnumber = Convert.ToInt32(trimed_port); // Converting Port  String to Int.
                string Ip_address = ip_textBox.Text;
                if (portnumber > 65535)  // I.e Empty port or Port number has greaterthan 65535
                {
                    MessageBox.Show("Port number should be lessthan 65535!!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    port_textBox.Text = "";  // Setting the Empty text in Port_textbox.
                    port_textBox.Focus();  // to set the port Textbox to be Focused 
                }
                else   // Perfect Port Number.
                {
                    /* Call Start_TcpServer() Method to satrt Tcp Server. */
                    HideBackgroundCntent();

                    Modal_dlg Dlg = new Modal_dlg();
                    Dlg.Owner = Window.GetWindow(this);
                  
                    Dlg.ShowDialog();
                    /*
                * 1. Show hidden components in the Mainwindow.
                * 2. Create one thread & to continously listen for Tcp Clients(Start Tcp Server) 
                * 3. Add Connected Tcp Client to TcpSocketsArr
                * 4. Maintain one thread for each Sockt created.

                */
                // 1
                    ShowBackgroundCntent();  // To Show the Background content 
                  // 2


                }
            }
            
            

        }

        public void HideBackgroundCntent()
        {
            ip_textBox.Visibility = Visibility.Hidden;
            Ip_label.Visibility= Visibility.Hidden;
            port_label.Visibility = Visibility.Hidden;
            port_textBox.Visibility = Visibility.Hidden;
            Button_connect.Visibility = Visibility.Hidden;
        }

        public void ShowBackgroundCntent()
        {
            ip_textBox.Visibility = Visibility.Visible;
            Ip_label.Visibility = Visibility.Visible;
            port_label.Visibility = Visibility.Visible;
            port_textBox.Visibility = Visibility.Visible;
            Button_connect.Visibility = Visibility.Visible;
            port_textBox.Focus();
        }

        private void Mouse_entered(object sender,RoutedEventArgs e)
        {

           
            Button_connect.Foreground = Brushes.DarkCyan;
        }

        private void Mouse_left(object sender, RoutedEventArgs e)
        {
            Color c = (Color)ColorConverter.ConvertFromString("#E91E63");
            Button_connect.Foreground = new SolidColorBrush(c);
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
   
    }
}
