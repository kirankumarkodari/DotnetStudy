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
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections.Concurrent;
using System.Timers;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Background_imageTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
         Modal_dlg Dlg;
        ServerClass Server;
        System.Timers.Timer Processing_Timer;
        public MainWindow()
        {
            InitializeComponent();
            string hostName = Dns.GetHostName();
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            Global.LoadDeviceIds();
            ip_textBox.Text = myIP;
            ip_textBox.IsEnabled = false;
            port_textBox.Focus();  // to set the port Textbox to be Focused 
       
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
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
                        Dlg = new Modal_dlg();
                        Dlg.Owner = Window.GetWindow(this);
                        Server = new ServerClass();
                        // To Start the TcpListener
                        Server.Initialize_Server(Ip_address, portnumber);   // To Start the Tcp Server.
                        Dlg.ShowDialog();
                        if (Global.Is_Manuallyclosed)
                        {
                            Global.Is_Manuallyclosed = false;
                            Server.ClearAllObbjectsused();
                            // To Show the Background content
                            ShowBackgroundCntent();
                        }
                        else //I.e The Tcp Client has been Connected to the Server , SO you should not stop the Server.
                        {
                            /*
                             * 0. Start the Processing Timer to Process the Data_Buffer.
                             * */
                           
                            Processing_Timer = new System.Timers.Timer(500);  // Checking for evey 0.5 Sec whether any Client Connected or not.
                            Processing_Timer.Elapsed += new ElapsedEventHandler(Server.ProcessDataBuffer);
                            Processing_Timer.AutoReset = true;
                            Processing_Timer.Enabled = true;
                            /*
                            *   SendDatatoClient() Should be accessible from outside 
                            * 1. Close the MainWindow 
                            * 2. Display the  Commands Window 
                            * 
                            * /*/

                        }

                    }
                   
                    // Should close the Main Form & Will Another Form can be Shown.

                }
            }
            catch(Exception ex)
            {
                DateTime now_time = DateTime.Now;
                string time = Convert.ToString(now_time);
                Global.AppendTexttoFile(Global.exception_filepath, "Exception Ocuured On Main Form Connect Click:  " + ex.Message + time); // Logging the Execptions Ocuured

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
        protected override void OnClosed(EventArgs e) // On Closing the MainWindow We Should Close the Application Also.
        {
            base.OnClosed(e);
            Server.ClearAllObbjectsused();  // On Closing the Application Need to clear the Memory of static even thogh CLR 4.0 will clear the Static Mmbers Jus Used for safty 
            Application.Current.Shutdown();
        }

    }

    public class DataClass
    {
        public IPAddress Ip_addr;
        public byte[] data;

        public DataClass(IPAddress Ip_add,byte[] data)
        {
            this.Ip_addr = Ip_add;
            this.data = data;
        }
        
    }
    public class DeviceInfo
    {
        public byte[] DeviceId;
        public DateTime LastActivetime;
        public bool IsConnected;
        public IPAddress Ipaddr;
        public NetworkStream ClientStream;
    }
    [Serializable()]
    public class PacketClass
    {
        public byte[] packetHeader = new byte[2];
        public byte[] Packet_Length = new byte[2];
        public  byte[] DeviceId = new byte[8];
        public byte[] Password = new byte[4];
        public byte[] TypeId = new byte[1];
        public byte[] Packet_sno = new byte[2];
        public byte[] Continue_Sts = new byte[1];
        public byte[] Data = new byte[1024];
        public byte[] Checksum = new byte[2];
    }
    public class AckClass
    {
        public byte[] packetHeader = new byte[2];
        public byte[] Packet_Length = new byte[2];
        public byte[] DeviceId = new byte[8];
        public byte[] Password = new byte[4];
        public byte[] TypeId = new byte[1];
        public byte[] Packet_sno = new byte[2];
        public byte[] Continue_Sts = new byte[1];
        public byte[] Data = new byte[1024];
        public byte[] Checksum = new byte[2];
    }
    public class SocketClass
    {
        public IPAddress IpAddr;
        public TcpClient Clientobj;
    }

    public class ServerClass
    {
     //   List<TcpClient> TcpClients_arr = new List<TcpClient>(Global.Max_clients); // To store connected Client Objects.
   //     public static List<IPAddress> ClientIp_arr = new List<IPAddress>(Global.Max_clients); // To Store the Connected CLient IP.

        public static List<SocketClass> Sockets_arr = new List<SocketClass>(Global.Max_clients); // To store connected Client Objects.

        public static BlockingCollection<DataClass> Data_Buffer = new BlockingCollection<DataClass>(Global.Max_databuff_size);  // Creating Blocking Collection to hadle data insertion from multiple clients

        public static TcpListener TcpServer;  // To Listen Continuosly for All Clients.

        public static List<DeviceInfo> Devices_Buffer = new List<DeviceInfo>(); // To Store the information of the Devices.. 

        Thread Listening_thread;
        Thread Client_thread;
        public void Initialize_Server(string Ipaddress,int port)
        {
            IPAddress Ip_addr = IPAddress.Parse(Ipaddress);
            TcpServer = new TcpListener(Ip_addr,port);
            
            this.Listening_thread = new Thread(new ThreadStart(ListenforClients));  // Creating Thread to Continuously Listen for Cients.
            this.Listening_thread.Start();
        }
        public void ProcessDataBuffer(object sender, ElapsedEventArgs elapsedEventArg) // To Process the DataBuffer 
        {
            try
            {
            if(!Global.Is_dataprocessing)  // If No dataprocessing beore the 
            {

            Global.Is_dataprocessing = true;  // It Is a Lock For this Procedure 

            if (Data_Buffer!=null) // If Data Buffer has some data elements
            {
                while(Data_Buffer.Count>0)
                {
                    DataClass data_item = Data_Buffer.Take();// Takes the first data_item from the Data_Buffer Queue.

                    bool Is_IpExists = false;  // Mark it as IP has not existed with data_item ip address

                    for(int i=0;i<= Sockets_arr.Count-1;i++)
                    {
                        if(Sockets_arr[i].IpAddr.Equals(data_item.Ip_addr))// If Ip_Exists In Connected IPs List
                        {
                            Is_IpExists = true;
                            break;

                        }
                    }
                    if(Is_IpExists) // I.e If Ip Exists So that Only you need to process otherwise discard it 
                    {
                        if(data_item.data.Length<=Global.Max_bytes && data_item.data.Length>=Global.Min_bytes) // Valid databyte
                        {
                                

                                PacketClass Packet_obj = DecodeRecievedBytestoPacket_Object(data_item.data); // Decoding bytes Array into Packetclass

                                Packet_obj=Reverse_PacketBytes(Packet_obj); // Reversing the Packet_obj Except DataBytes.

                                short cal_checksum = Calc_ChecksumOfPacketObj(Packet_obj);  // Pasing the Packet object to calculate checksum;

                                bool Is_valid = IsValidPacket(Packet_obj.Checksum,Packet_obj.Password,Packet_obj.DeviceId, cal_checksum);  // Verifies Checksum, DeviceID 

                                if (Is_valid)   // I.e Recieved Pakcet Is Valid 
                                    {
                                    if(Packet_obj.packetHeader.SequenceEqual(Global.CommInit_pkt_header))
                                    {
                                        ProcessCommInitPacket(Packet_obj, data_item.Ip_addr);  // Communication Initialization Packet
                                    }
                                    else if(Packet_obj.packetHeader.SequenceEqual(Global.CommInitResponse_pkt_header))
                                    {
                                        ProcessCommandResponsePacket(Packet_obj, data_item.Ip_addr); // Command Response Packet

                                    }
                                    else if(Packet_obj.packetHeader.SequenceEqual(Global.Event_pkt_header))
                                        {
                                        ProcessEventPacket(Packet_obj, data_item.Ip_addr);          // Event Packet
                                    }
                                    else if(Packet_obj.packetHeader.SequenceEqual(Global.Event_pkt_header))
                                    { 
                                        ProcessModuleAlivePacket(Packet_obj, data_item.Ip_addr);    // Module Alive Packet
                                    }
                                }
                        }

                    }

                }
            }
             Global.Is_dataprocessing = false;

                }// End of Isdataprocessing
            }
            catch(Exception ex)
            {
                DateTime now_time = DateTime.Now;
                string time = Convert.ToString(now_time);
                Global.AppendTexttoFile(Global.exception_filepath, "Exception Ocuured on PrcoessDataBuffer:  " + ex.Message + time); // Logging the Execptions Ocuured
                Global.Is_dataprocessing = false;
            }
        }
        public void ProcessCommInitPacket(PacketClass PacketObj, IPAddress Ipaddr)
        {
            /* Communication Init Packet */
            try
            {
            bool IsDeviceExists = false;
           

            for (int i = 0; i <= Devices_Buffer.Count - 1; i++)
            {
                if (Devices_Buffer[i].DeviceId.SequenceEqual(PacketObj.DeviceId))
                {
                    IsDeviceExists = true;/* Comm had failed and it is trying with the new IP. */
                    /* Check IsConnected */
                    if (Devices_Buffer[i].IsConnected)  // Because Connection_Checking Method still not Removed Old Socket
                    {
                        RemoveSocketByusingIP(Devices_Buffer[i].Ipaddr); 
                    }

                     SocketClass Socketobj=FindSocketbyusingIp(Ipaddr); // To Find the Socket by using new IP Address
                    if(Socketobj!=null)  // Socketobj will be null when same IP requests 
                    {
                        Devices_Buffer[i].IsConnected = true;

                        Devices_Buffer[i].LastActivetime = DateTime.Now;

                        Devices_Buffer[i].Ipaddr = Socketobj.IpAddr;  // New Ip Address

                       // Devices_Buffer[i].ClientStream.Close();  // To Close the Previous Network Stream bw Client & Server.

                        Devices_Buffer[i].ClientStream = Socketobj.Clientobj.GetStream(); // New Socket Stream 


                            /* Frame Ack for Communication Initialization Packet */
                         byte[] CommAckdata = FrameCommunicationAck(PacketObj);

                         SendDatatoClient(CommAckdata, Devices_Buffer[i].ClientStream);  // Device is Re Connected with new Ip Address
                           
                    }

                    break;

                    }
            }


            if (!IsDeviceExists)  // If No Device Exists 
            {
                DeviceInfo Device = new DeviceInfo();
                Device.DeviceId = new byte[PacketObj.DeviceId.Length]; // Need to Initialize the DeviceID array
                PacketObj.DeviceId.CopyTo(Device.DeviceId,0) ; // It is Dummy 
                Device.IsConnected = true;
                Device.LastActivetime = DateTime.Now;
                SocketClass Socketobj = FindSocketbyusingIp(Ipaddr); // To Find the Socket by using new IP Address
                Device.Ipaddr = Socketobj.IpAddr;
                Device.ClientStream = Socketobj.Clientobj.GetStream();
                Devices_Buffer.Add(Device); // Adding New Device to the Devices_buff

                /* Frame Ack for Communication Initialization Packet */
                byte[] CommAckdata=FrameCommunicationAck(PacketObj);

                    // Don't know why DeviceID is getting reversed when i reverse the DeviceID in AckPacket
              //  Array.Reverse(Device.DeviceId); 

                SendDatatoClient(CommAckdata, Device.ClientStream);  // Device is New & Sending  Bytes to the Client (Device ).
            }
            }
            catch(Exception ex)
            {
                DateTime now_time = DateTime.Now;
                string time = Convert.ToString(now_time);
                Global.AppendTexttoFile(Global.exception_filepath, "Exception Ocuured in ProcessCommInitPacket:  " + ex.Message + time); // Logging the Execptions Ocuured
                
            }

        }
        public static bool SendDatatoClient(byte[] data,NetworkStream stream)
        {
            try
            {
                /* Prepare Byte[] & Client Stream to send data to Client */
                stream.Write(data, 0, data.Length);     //Using stream to write serialized data bytes to the client.
                return true;
            }
            catch (Exception ex)
            {
                DateTime now_time = DateTime.Now;
                string time = Convert.ToString(now_time);
                Global.AppendTexttoFile(Global.exception_filepath, "Exception Ocuured While SendDatatoClient:  " + ex.Message + time); // Logging the Execptions Ocuured
                return false;
            }
           
        }
        private byte[] FrameCommunicationAck(PacketClass PacketObj)
        {
            try
            {
                AckClass AckPacket = new AckClass();
                AckPacket.packetHeader = Global.CommInitResponse_pkt_header;
                // Length ?? 18
                // AckPacket.Packet_Length = PacketObj.Packet_Length;  { Should not do like this beacuse it is going to change Original object }
                PacketObj.Packet_Length.CopyTo(AckPacket.Packet_Length, 0);
            
                PacketObj.DeviceId.CopyTo(AckPacket.DeviceId, 0);

                PacketObj.Password.CopyTo(AckPacket.Password, 0);

                Global.CommInitAckPkt_TypeID.CopyTo(AckPacket.TypeId, 0);

                Global.CommInitAckPkt_SNo.CopyTo(AckPacket.Packet_sno, 0);

                Global.CommInitAckContinue_Sts.CopyTo(AckPacket.Continue_Sts, 0);

                AckPacket.Data = new byte[] { }; // Empty Data Feild

                short checksum = Calc_ChecksumOfAckPacketObj(AckPacket);

                AckPacket.Checksum = BitConverter.GetBytes(checksum); // Getting Bytes from checksum..

                AckPacket=Reverse_AckPacketBytes(AckPacket);   // Reversing the AckPacket Object Is Causing the Device Id in Reverse 
               
                
                byte[] Final_bytes = ObjectToByteArray(AckPacket); // Serializing Object to Byte Array.

                return Final_bytes; // Returning Serialozed Byte array.
            }
            catch(Exception ex)
            {
                DateTime now_time = DateTime.Now;
                string time = Convert.ToString(now_time);
                Global.AppendTexttoFile(Global.exception_filepath, "Exception Ocuured While FrameCommunicationAck:  " + ex.Message + time); // Logging the Execptions Ocuured
                return null;
            }
        }

        public short Calc_ChecksumOfPacketObj(PacketClass PktObj)
        {

            short checksum = 0;
            foreach (byte chData in PktObj.DeviceId)
            {
                checksum += chData;
            }
            foreach (byte chData in PktObj.Password)
            {
                checksum += chData;
            }
            foreach (byte chData in PktObj.TypeId)
            {
                checksum += chData;
            }
            foreach (byte chData in PktObj.Packet_sno)
            {
                checksum += chData;
            }
            foreach (byte chData in PktObj.Continue_Sts)
            {
                checksum += chData;
            }
            foreach (byte chData in PktObj.Data)
            {
                checksum += chData;
            }
            /* Condition should be added if and only if checksum is greater than 65535(2bytes).*/
         //   checksum &= 0xff;
            return checksum;
        }

        public short Calc_ChecksumOfAckPacketObj(AckClass PktObj)
        {

            short checksum = 0;
            foreach (byte chData in PktObj.DeviceId)
            {
                checksum += chData;
            }
            foreach (byte chData in PktObj.Password)
            {
                checksum += chData;
            }
            foreach (byte chData in PktObj.TypeId)
            {
                checksum += chData;
            }
            foreach (byte chData in PktObj.Packet_sno)
            {
                checksum += chData;
            }
            foreach (byte chData in PktObj.Continue_Sts)
            {
                checksum += chData;
            }
            foreach (byte chData in PktObj.Data)
            {
                checksum += chData;
            }
            /* Condition should be added if and only if checksum is greater than 65535(2bytes).*/
            //   checksum &= 0xff;
            return checksum;
        }
        private byte[] ObjectToByteArray(AckClass obj)
        {
            try
            {
                /* Need to place the bytes[] into Object */
                /* Get the data bytes length */
                int array_len = 0;
                array_len = Global.Packet_overhead_len; // Asumming Packet has no data.

                int data_bytes_len = obj.Data.Length;// Packet_overhead_len is Fixed for all packets but data changes.

                if(data_bytes_len>0)/* Packet has Some Databyte need to change the length */
                {
                    array_len = array_len + data_bytes_len;
                }
                /* Creating new Array */
                byte[] Ack_bytes = new byte[array_len];

                int Packet_Checksum_StartIdx = Global.Packet_data_start_Idx + data_bytes_len;
                /* Frame Packet Object from byte[] data */

                Array.Copy(obj.packetHeader,0 , Ack_bytes, Global.Packet_Header_start_Idx, Global.Packet_Header_len);  // Copy PacketHeader
                Array.Copy(obj.Packet_Length, 0, Ack_bytes, Global.PacketLength_start_Idx, Global.PacketLength_len);  // Copy Length of Packet
                Array.Copy(obj.DeviceId,0 , Ack_bytes, Global.Packet_DeviceID_start_Idx, Global.Packet_DeviceID_len);  // Copy Device Id of Packet
                Array.Copy(obj.Password, 0, Ack_bytes, Global.Packet_Password_start_Idx, Global.Packet_Password_len);  // Copy System Password of Packet
                Array.Copy(obj.TypeId, 0, Ack_bytes, Global.Packet_TypeId_start_Idx, Global.Packet_TypeId_len);      // Copy  TYpeID of Packet
                Array.Copy(obj.Packet_sno,0 , Ack_bytes, Global.Packet_Sno_start_Idx, Global.Packet_Sno_len);    // Copy Packet SNo of Packet
                Array.Copy(obj.Continue_Sts,0 , Ack_bytes, Global.Packet_ContinueSts_start_Idx, Global.Packet_ContinueSts_len);     // Copy Continue_Sts of th Packet
                Array.Copy(obj.Data, 0, Ack_bytes, Global.Packet_data_start_Idx, data_bytes_len);     // Copy Data  of Packet

                Array.Copy(obj.Checksum, 0, Ack_bytes, Packet_Checksum_StartIdx, Global.Packet_Chcksum_len);     // Copy Checksum  of Packet

                return Ack_bytes; // Returning Ack bytes 
            }
            catch(Exception ex)
            {
                DateTime now_time = DateTime.Now;
                string time = Convert.ToString(now_time);
                Global.AppendTexttoFile(Global.exception_filepath, "Exception Occured in ObjectToByteArray:  " + ex.Message + time); // Logging the Execptions Ocuured
                return null;
            }
        }
        private SocketClass FindSocketbyusingIp(IPAddress IpAddr)
        {
            try
            {

            bool IsSocket_Exists = false;
            int i;
            for(i=0;i<=Sockets_arr.Count-1;i++)
            {
                if(Sockets_arr[i].IpAddr.Equals(IpAddr)) // Socket is there with that IP
                    {
                    IsSocket_Exists = true;
                    break;
                }
            }
            if(IsSocket_Exists)
            {
                return Sockets_arr[i];
            }
            else
            {
                return null;   // There is no Socket 
            }
            }
            catch(Exception ex)
            {
                DateTime now_time = DateTime.Now;
                string time = Convert.ToString(now_time);
                Global.AppendTexttoFile(Global.exception_filepath, "Exception Occured in FindSocketbyusingIp:  " + ex.Message + time); // Logging the Execptions Ocuured
                return null;
            }
        }
        private void RemoveSocketByusingIP(IPAddress Ip_addr)
        {
            /* Remove the Socket From Sockets_arr */
            try
            {

            int Socket_loc =Global.default_socket_loc;
            for(int j=0;j<=Sockets_arr.Count-1;j++)
            {
                if(Sockets_arr[j].IpAddr.Equals(Ip_addr)) /* This is the Old Socket of the Device */
                {
                    Socket_loc = j;
                    break;
                }

            }

            if(!(Socket_loc==Global.default_socket_loc)) // Old Socket is there  So remove it by using Index.
            {
                Sockets_arr.RemoveAt(Socket_loc); // Removing Old Socket.
                
            }
            }
            catch(Exception ex)
            {
                DateTime now_time = DateTime.Now;
                string time = Convert.ToString(now_time);
                Global.AppendTexttoFile(Global.exception_filepath, "Exception Occured in RemoveSocketByusingIP:  " + ex.Message + time); // Logging the Execptions Ocuured
              
            }
        }
        public void ProcessCommandResponsePacket(PacketClass PacketObj, IPAddress Ipaddr)
        {
            /* Command Response Packet */
        }
        public void ProcessEventPacket(PacketClass PacketObj, IPAddress Ipaddr)
        {
            /* Event Packt */

        }
        public void ProcessModuleAlivePacket(PacketClass PacketObj, IPAddress Ipaddr)
        {
            /* Module Alive Packet */

        }
       
        public PacketClass DecodeRecievedBytestoPacket_Object(byte[] data)
        {
            try
            {

            
            PacketClass obj = new PacketClass();
            /* Need to place the bytes[] into Object */
            /* Get the data bytes length */
            int data_bytes_len = data.Length - Global.Packet_overhead_len; // Packet_overhead_len is Fixed for all packets but data changes.
            int Packet_Checksum_StartIdx = Global.Packet_data_start_Idx + data_bytes_len;
            /* Frame Packet Object from byte[] data */

            Array.Copy(data, Global.Packet_Header_start_Idx, obj.packetHeader, 0, Global.Packet_Header_len);  // Copy PacketHeader
            Array.Copy(data, Global.PacketLength_start_Idx, obj.Packet_Length, 0, Global.PacketLength_len);  // Copy Length of Packet
            Array.Copy(data, Global.Packet_DeviceID_start_Idx, obj.DeviceId, 0, Global.Packet_DeviceID_len);  // Copy Device Id of Packet
            Array.Copy(data, Global.Packet_Password_start_Idx, obj.Password, 0, Global.Packet_Password_len);  // Copy System Password of Packet
            Array.Copy(data, Global.Packet_TypeId_start_Idx, obj.TypeId, 0, Global.Packet_TypeId_len);      // Copy  TYpeID of Packet
            Array.Copy(data, Global.Packet_Sno_start_Idx, obj.Packet_sno, 0, Global.Packet_Sno_len);    // Copy Packet SNo of Packet
            Array.Copy(data, Global.Packet_ContinueSts_start_Idx, obj.Continue_Sts, 0, Global.Packet_ContinueSts_len);     // Copy Continue_Sts of th Packet
            Array.Copy(data, Global.Packet_data_start_Idx, obj.Data, 0, data_bytes_len);     // Copy Data  of Packet
            
            Array.Copy(data, Packet_Checksum_StartIdx, obj.Checksum, 0, Global.Packet_Chcksum_len);     // Copy Data  of Packet

            return obj;
            }
            catch(Exception ex)
            {
                DateTime now_time = DateTime.Now;
                string time = Convert.ToString(now_time);
                Global.AppendTexttoFile(Global.exception_filepath, "Exception Occured in DecodeRecievedBytestoPacket_Object:  " + ex.Message + time); // Logging the Execptions Ocuured
                return null;
            }
        }

        public PacketClass Reverse_PacketBytes(PacketClass Packet_obj)
        {
            try
            {
            Array.Reverse(Packet_obj.packetHeader); // Reversing the Packet Header Bytes
            Array.Reverse(Packet_obj.Packet_Length);
            Array.Reverse(Packet_obj.DeviceId);
            Array.Reverse(Packet_obj.Password);
            Array.Reverse(Packet_obj.TypeId);
            Array.Reverse(Packet_obj.Packet_sno);
            Array.Reverse(Packet_obj.Continue_Sts);
            Array.Reverse(Packet_obj.Checksum);

            return Packet_obj;
            }
            catch(Exception ex)
            {
                DateTime now_time = DateTime.Now;
                string time = Convert.ToString(now_time);
                Global.AppendTexttoFile(Global.exception_filepath, "Exception Occured in Reverse_PacketBytes:  " + ex.Message + time); // Logging the Execptions Ocuured
                return null;
            }
        }

        public AckClass Reverse_AckPacketBytes(AckClass Ack_obj)
        {
            try
            {
                Array.Reverse(Ack_obj.packetHeader); // Reversing the Packet Header Bytes
                Array.Reverse(Ack_obj.Packet_Length);
                Array.Reverse(Ack_obj.DeviceId);
                Array.Reverse(Ack_obj.Password);
                Array.Reverse(Ack_obj.TypeId);
                Array.Reverse(Ack_obj.Packet_sno);
                Array.Reverse(Ack_obj.Continue_Sts);
                Array.Reverse(Ack_obj.Checksum);

                return Ack_obj;
            }
            catch (Exception ex)
            {
                DateTime now_time = DateTime.Now;
                string time = Convert.ToString(now_time);
                Global.AppendTexttoFile(Global.exception_filepath, "Exception Occured in Reverse_PacketBytes:  " + ex.Message + time); // Logging the Execptions Ocuured
                return null;
            }
        }

        public short Calculate_Checksum(byte[] data)
        {
            short checksum = 0;
            foreach (byte chData in data)
            {
                checksum += chData;
            }
            checksum &= 0xff;
            return checksum;
        }
        public bool IsValidPacket(byte[] checksum,byte[] Syspassword,byte[] Deviceid,short cal_checksum)
        {
            /* Fetch the Checksum byte from byte[] data and */
            /* Compare the Calculated checksum with Original checksum*/
            bool IsValidChecksum = false;
            bool IsValidSysPassword = false;
            bool IsVaildDeviceId = false;
            byte[] Cal_checksum_arr = BitConverter.GetBytes(cal_checksum);
            Array.Reverse(Cal_checksum_arr); // Reversing the bytes in byte[] array because calculated checksum is in reverse by BitConverter
            /* If two are equal then return true or false */
            if(Cal_checksum_arr.SequenceEqual(checksum))
            {
                IsValidChecksum = true;
            }

            /* Fetch Device Id from the byte[] data */

            /* Check whether fetched DeviceID is in Allowed_DeviceID's Array */
            for(int i=0;i<=Global.Allowed_DevicesList.Count-1;i++)
            {
                if(Global.Allowed_DevicesList[i].Device_Id.SequenceEqual(Deviceid))
                {
                    IsVaildDeviceId = true;
                    break;
                }
            }
          

            if(Syspassword.SequenceEqual(Global.SystemPassword)) // iF sYSTEM password is valid 
            {
                IsValidSysPassword = true;
            }
            
            /* Length feild validation has benn ignored */ 
            if(IsValidChecksum && IsVaildDeviceId && IsValidSysPassword)  // Totally Valid Packet
            {
                return true;  // added temparorly.
            }
            else            // In Valid Packet 
            {
                return false;
            }
            
        }

        public  void ClearAllObbjectsused()
        {
            try
            {


                TcpServer.Stop();  // to stop the  TCp Sever for listening clients
                if(this.Listening_thread!=null)
                {
                    this.Listening_thread.Abort(); // Aborting Listening thread
                }
               if(this.Client_thread!=null)
                {
                    this.Client_thread.Abort(); // Abort Client_thread 
                }
                
                

                for (int i = 0; i <= Sockets_arr.Count - 1; i++) // Clearing the Memory Used by Client Sockets, ClientIP_arr
                {
                    if (Sockets_arr[i] != null)
                    {
                        Sockets_arr[i] = null;
                    }
                    

                }
                if (Data_Buffer != null)
                {
                    while (Data_Buffer.Count > 0)
                    {

                        var obj = Data_Buffer.Take();
                        obj = null;
                    }

                }
            }
            catch(Exception ex)
            {
                if(ex!=null)
                {
                    DateTime now_time = DateTime.Now;
                    string time = Convert.ToString(now_time);
                    Global.AppendTexttoFile(Global.exception_filepath, "Exception Ocuured While Clearing Objects of Server class:  " + ex.Message + time); // Logging the Execptions Ocuured

                }
            }


        }

        public void ListenforClients()
        {

            TcpServer.Start();  // To Start The TCP Server 
            while(true)
            {
                try
                {
                    TcpClient Client = TcpServer.AcceptTcpClient();

                    IPEndPoint Endpoint = Client.Client.RemoteEndPoint as IPEndPoint;
                    IPAddress Ip_addr = Endpoint.Address;

                    bool IsIp_exists = false;
                    for (int i = 0; i <= Sockets_arr.Count - 1; i++)
                    {
                        if (Sockets_arr[i].IpAddr.Equals(Ip_addr))  // If Ip Already Exists
                        {
                            IsIp_exists = true;
                            break;
                        }
                    }
                    if (!(IsIp_exists)) //New Client so need to process It.
                    {
                        SocketClass Socketobj = new SocketClass();
                        Socketobj.IpAddr = Ip_addr;
                        Socketobj.Clientobj = Client;
                        Sockets_arr.Add(Socketobj);  // Add New Socket 

                        Client_thread = new Thread(new ParameterizedThreadStart(HandleComm));
                        
                        Client_thread.Start(Client);  // To Start the Thread 
                    }
                    else /* Existed Tcp Client & trying to update the TCp Cient  Whether with Same Ipaddress Tcp Client will try or not */
                    {

                    }
                }
                catch (Exception ex)
                {
                    DateTime now_time = DateTime.Now;
                    string time = Convert.ToString(now_time);
                    if(ex!=null)
                    {
                        Global.AppendTexttoFile(Global.exception_filepath, "Exception Ocuured While Listening For Clients:  " + ex.Message + time); // Logging the Execptions Ocuured

                    }
                }
            }
        }

      

        private void HandleComm(object client)
        {
            TcpClient Tcp_Client = (TcpClient)client;
            using (NetworkStream stream = Tcp_Client.GetStream())
            {
                // get the ip address of the Client.
                IPEndPoint Endpoint = Tcp_Client.Client.RemoteEndPoint as IPEndPoint;

                IPAddress Ip_addr = Endpoint.Address;

                // Read the data from stream &  Insert it to Blocking Collection.
                Byte[] bytes = new Byte[Global.Max_bytes_read];
                int Read_len = 0;

                /*Open the Stream to Continuosly read the bytes from Client */
                while ((Read_len = stream.Read(bytes, 0, bytes.Length)) != 0)
                {


                    // Translate data bytes to a ASCII string.
                    try
                    {

                       
                        // create  byte array based on the stream length.
                        byte[] msg = new byte[Read_len];

                        // Copy the bytes from bytes[] array up to i(220 Original data to msg byte[] array
                        Array.Copy(bytes, 0, msg, 0, Read_len);

                        // Create new dataobject, 
                        bool IsValidClientIp = false;
                        for(int i=0;i<=Sockets_arr.Count-1;i++)
                        {
                            if(Sockets_arr[i].IpAddr.Equals(Ip_addr))
                            {
                                IsValidClientIp = true;
                                break;
                            }
                        }
                        if(IsValidClientIp)
                        {
                            DataClass data_item = new DataClass(Ip_addr, msg);

                            // Add the created dataobject to data_buffer(Blocking Collection)\
                            Data_Buffer.Add(data_item);
                        }
                        else
                        {
                            break;  // To Break the While Loop 
                          //  Client_thread.Abort(); { termnating thread }
                        }
                        

                    }

                    // stream.Write(msg,0, msg.Length);
                    catch (Exception ex)
                    {
                        if (ex != null)
                        {
                            DateTime now_time = DateTime.Now;
                            string time = Convert.ToString(now_time);
                            Global.AppendTexttoFile(Global.exception_filepath, "Exception Ocuured While Reading Stream Data from the Client :  " + ex.Message + time); // Logging the Execptions Ocuured
                        }
                        }
                }
            }
           
        }

    }
}
