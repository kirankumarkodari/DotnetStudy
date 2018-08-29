using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace TcpListener_Program
{
    
    class Program
    {
        TcpListener server = null;
        Byte[] bytes = new Byte[256];
        String data = null;
        NetworkStream stream;
        TcpClient client;
        int i;
        public static System.Timers.Timer aTimer;
        static void Main(string[] args)
        {

            try
            {
                Program p = new Program();
                 p.Connection_Initializer();
            

                 ThreadPool.QueueUserWorkItem(p.ThreadProc);
              
                    aTimer = new System.Timers.Timer(2000);
                    aTimer.Elapsed += new ElapsedEventHandler(p.DataSender);
                    aTimer.AutoReset = true;
                    aTimer.Enabled = true;
                
               





                Console.WriteLine("Program End");
                Console.ReadKey();

            }
            catch(Exception ex)
            {

            }
        }
        
        public  void ThreadProc(object obj)
        {

            DataReciever();
        }

        public  void DataReciever()
        {
            // Get a stream object for reading and writing
          
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                // Translate data bytes to a ASCII string.
                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                Console.WriteLine("Received: {0}", data);

                // Process the data sent by the client.
                data = data.ToUpper();

                byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                // Send back a response.
                stream.Write(msg, 0, msg.Length);
                Console.WriteLine("Sent: {0}", data);
            }
            
        }

        public void DataSender(object sender, ElapsedEventArgs elapsedEventArg)
        {
            string msg = "Test Success!!";
            byte[] str = Encoding.ASCII.GetBytes(msg);
            stream.Write(str, 0, str.Length);
           
        }
        
         public void Connection_Initializer()
        {
            server = null;
            Int32 port = 13000;
            IPAddress localAddr = IPAddress.Parse("192.168.1.20");

            // TcpListener server = new TcpListener(port);
            server = new TcpListener(localAddr, port);

            // Start listening for client requests.
            server.Start();

            Console.WriteLine("Waiting for Conn!");

            client = server.AcceptTcpClient();

            Console.WriteLine("Connected!");

            data = null;
            stream = client.GetStream();

        }

        public void Connection_closer()
        {
            client.Close();
            server.Stop();
        }


        }
    }

