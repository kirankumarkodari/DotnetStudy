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

namespace Background_imageTest
{
    /// <summary>
    /// Interaction logic for Modal_dlg.xaml
    /// </summary>
    public partial class Modal_dlg : Window
    {
        public Modal_dlg()
        {
            InitializeComponent();
        }

        private void Buttoncancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               

                this.Close(); // Closing Modal Dailogue
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
