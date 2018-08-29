using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Background_imageTest
{
    public class DeviceIDClass
    {
       public byte[] Device_Id;
       public string Device_name;
    }
    public static class Global
    {
        public  const int Max_clients = 15;
        public const int Default_updateindx = 99;
        public const int Max_bytes_read = 1100;
        public const int Max_databuff_size= 500;
        public const string exception_filepath = "C:\\AAC\\Logs\\Exceptions.txt";
        public static bool Is_Manuallyclosed = true;
        public static bool Is_dataprocessing = false;
        public static bool Is_Commandprocessing = false;
        public static bool Is_ClosedbySW = false;
        public static bool IsDeviceConnectedtoSw = false;
        public const int Max_bytes=1050;
        public const int Min_bytes = 22;
        public const int default_socket_loc = 999;
        public const byte MaxResendCnt = 3;
        public const byte ResendwaitTime = 5; // 5 Seconds Should wait.
        public readonly static byte[] SystemPassword = new byte[]{ 0x65,0x85,0x94,0x65};  //65859465
        public static List<DeviceIDClass> Allowed_DevicesList=new List<DeviceIDClass>();

        /* These all are for Packet Decoding & Encoding */
        /* Packet Header */
        public const int Packet_Header_start_Idx = 0;
        public const int Packet_Header_len = 2;
        /* Length */
        public const int PacketLength_start_Idx = 2;
        public const int PacketLength_len = 2;
        /* Packet Device Id */
        public const int Packet_DeviceID_start_Idx = 4;
        public const int Packet_DeviceID_len = 8;
        /*Password */
        public const int Packet_Password_start_Idx = 12;
        public const int Packet_Password_len = 4;
        /* TypedID*/
        public const int Packet_TypeId_start_Idx = 16;
        public const int Packet_TypeId_len = 1;
        /* Packet Sno */
        public const int Packet_Sno_start_Idx = 17;
        public const int Packet_Sno_len = 2;
        /* Continue Status */
        public const int Packet_ContinueSts_start_Idx = 19;
        public const int Packet_ContinueSts_len = 1;
        /* Data */
        public const int Packet_data_start_Idx = 20;
        public const int Packet_data_len = 1044;
        /*Checksum*/
        public const int Packet_Checksum_Idx = 1044;
        public const int Packet_Chcksum_len = 2;

        /* All Packt_Overhead Length */
        public const int Packet_overhead_len = 22;
        public const int Packet_Default_length = 18;

        /* Keycode Data Length */
        public const int keycode_data_length = 994; /* I.e 142 KeyCodes at a time & Each Keycode is 7 bytes Length.*/
        public const int Keycode_total_array_length = keycode_data_length + Packet_overhead_len;

        /*Packet Headres */
        public readonly static byte[] Command_pkt_header = new byte[] { 0xAA,0x66 };
        public readonly static byte[] CommandResponse_pkt_header = new byte[] { 0xAA, 0x33 };
        public readonly static byte[] Event_pkt_header = new byte[] { 0xAA, 0x55 };
        public readonly static byte[] EventResponse_pkt_header = new byte[] { 0xAA, 0x99 };
        public readonly static byte[] CommInit_pkt_header = new byte[] { 0xAA, 0xCC };
        public readonly static byte[] CommInitResponse_pkt_header = new byte[] { 0xAA, 0xDD };
        public readonly static byte[] ModuleAlive_pkt_header = new byte[] { 0xAA, 0x44 };


        /* PacketTypeIDs for Communication */
        public readonly static byte[] CommInitAckPkt_TypeID = new byte[] {0x90 };
        public readonly static byte[] CommInitAckPkt_SNo = new byte[] {0x00,0x00 };
        public readonly static byte[] CommInitAckContinue_Sts = new byte[] { 0x00 };


        /* Packet TyPeIDs for Commands */
        public readonly static byte[] Command_KeyCodePkt_TypeID = new byte[] {0x06 };
        public readonly static byte[] Command_KeyCodePkt_Sno = new byte[] { 0x00, 0x01 };
        public readonly static byte[] Command_KeyCodePkt_ContSts = new byte[] { 0x00 };

        public const byte DefaultKeyCode = 0xFF;

        private static ReaderWriterLockSlim lock_ = new ReaderWriterLockSlim(); // lock to write on single file 

        public static void LoadDeviceIds()
        {
            DeviceIDClass DeviceId1 = new DeviceIDClass();

            DeviceId1.Device_Id = new byte[] { 0x04, 0x04,0x07, 0x16, 0x16, 0x61, 0x31, 0x48 }; //0404071616613148
            DeviceId1.Device_name = "Device 1";
            Allowed_DevicesList.Add(DeviceId1);

            DeviceIDClass DeviceId2 = new DeviceIDClass();

            DeviceId2.Device_Id = new byte[] { 0x04,0x05,0x85,0x40, 0x56,0x01,0x39,0x40 }; //0405854056013940
            DeviceId2.Device_name = "Device 2";
            Allowed_DevicesList.Add(DeviceId2);

            DeviceIDClass DeviceId3 = new DeviceIDClass();
            DeviceId3.Device_Id = new byte[] { 0x04,0x05,0x85, 0x40, 0x56, 0x01,0x38,0x86 }; //0405854056013886----
            DeviceId3.Device_name= "Device 3";
            Allowed_DevicesList.Add(DeviceId3);


            DeviceIDClass DeviceId4 = new DeviceIDClass();
            DeviceId4.Device_Id = new byte[] { 0x04,0x05,0x85, 0x04, 0x54,0x63,0x01,0x63 }; //0405854054630163
            DeviceId4.Device_name = "Device 4";
            Allowed_DevicesList.Add(DeviceId4);

            DeviceIDClass DeviceId5 = new DeviceIDClass();
            DeviceId5.Device_Id = new byte[] { 0x04,0x05,0x85, 0x40, 0x56, 0x01,0x39,0x02 }; //0405854056013902
            DeviceId5.Device_name = "Device 5";
            Allowed_DevicesList.Add(DeviceId5);

            DeviceIDClass DeviceId6 = new DeviceIDClass();
            DeviceId6.Device_Id = new byte[] { 0x04,0x04,0x07,0x16,0x00,0x49,0x75,0x55 }; //0404071600497555
            DeviceId6.Device_name = "Device 6";
            Allowed_DevicesList.Add(DeviceId6);

            /* Added 6 Device ID's Till Then */

        }

        public static void AppendTexttoFile(string filepath,string content)
        {
            lock_.EnterWriteLock(); // Locking the File
            try
            {
            if (!File.Exists(filepath))
            {
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(content);
                }

            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(content);
                }
            }
            }
            finally
            {
                lock_.ExitWriteLock(); // Un locking the file 
            }
        }
    }
}
