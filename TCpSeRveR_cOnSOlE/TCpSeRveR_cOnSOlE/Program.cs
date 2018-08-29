using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;

namespace TCpSeRveR_cOnSOlE
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                IPAddress ipAddress = IPAddress.Parse("192.168.1.20");

                Console.WriteLine("Starting TCP listener...");

                TcpListener listener = new TcpListener(ipAddress, 500);

                listener.Start();

                Console.WriteLine("Waiting for TCp Client COnnection");

                TcpClient client = listener.AcceptTcpClient();

                Console.WriteLine("Connection accepted.");
                while (true) // Add your exit flag here
                    {
                        

                        NetworkStream ns;
                        ns = client.GetStream();
                       string response;
                       StringBuilder msg = new StringBuilder();
                       using (StreamReader reader = new StreamReader(ns))
                       {
                          while ((response = reader.ReadLine().Trim()) != ".")
                          {
                            msg.AppendLine(response);
                          }
                          Console.WriteLine(msg.ToString());
                       }
                    // ThreadPool.QueueUserWorkItem(ThreadProc, client);
                    }

                Console.WriteLine("Out of While Lopp");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.StackTrace);
                Console.ReadLine();
            }
        }
        /*
        private static void ThreadProc(object obj)
        {
            try
            {
                var client = (TcpClient)obj;
                Console.WriteLine("Client Connected or Disconnected!!");
                
                NetworkStream ns;
                ns = client.GetStream();
                string response;
                StringBuilder msg = new StringBuilder();
                using (StreamReader reader = new StreamReader(ns))
                {
                    while ((response = reader.ReadLine().Trim()) != ".")
                    {
                        msg.AppendLine(response);
                    }
                    Console.WriteLine(msg.ToString());
                }
                

                // Do your work here
            }

            catch (Exception ex)
            {
                Console.WriteLine("Exception In Client : " + ex.Message);
            }
        }*/
    }
}
