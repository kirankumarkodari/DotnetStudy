using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TcpListenerFinal
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                IPAddress ipAddress = IPAddress.Parse("127.0.0.1");

                Console.WriteLine("Starting TCP listener...");

                TcpListener listener = new TcpListener(ipAddress, 500);

                listener.Start();
                Form2 f2 = new Form2();
                f2.Show();
                while (true)
                {
                    Socket client = listener.AcceptSocket();
                    Console.WriteLine("Connection accepted.");

                    var childSocketThread = new Thread(() =>
                    {
                        byte[] data = new byte[100];
                        int size = client.Receive(data);
                        Console.WriteLine("Recieved data: ");
                        for (int i = 0; i < size; i++)
                            Console.Write(Convert.ToChar(data[i]));

                        Console.WriteLine();

                        client.Close();
                    });
                    childSocketThread.Start();

                }
                listener.Stop();


            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.StackTrace);
                Console.ReadLine();
            }

        }
    }
}
