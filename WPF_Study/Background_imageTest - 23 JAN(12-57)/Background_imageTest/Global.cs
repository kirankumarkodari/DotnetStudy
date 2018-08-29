using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Background_imageTest
{
    public static class Global
    {
        public  const int Max_clients = 15;
        public const int Default_updateindx = 99;
        public const int Max_bytes_read = 1100;
        public const int Max_databuff_size= 500;
        public const string exception_filepath = "C:\\AAC\\Logs\\Exceptions.txt";
        public static bool Is_Manuallyclosed = true;
        public static bool Is_dataprocessing = false;
        public const int Max_bytes=1050;
        public const int Min_bytes = 22;
        public const int default_socket_loc = 999;
        public const long SystemPassword = 65859465;


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



        /*Packet Headres */
        public readonly static byte[] Command_pkt_header = new byte[] { 0xAA,0x66 };
        public readonly static byte[] CommandResponse_pkt_header = new byte[] { 0xAA, 0x33 };
        public readonly static byte[] Event_pkt_header = new byte[] { 0xAA, 0x55 };
        public readonly static byte[] EventResponse_pkt_header = new byte[] { 0xAA, 0x99 };
        public readonly static byte[] CommInit_pkt_header = new byte[] { 0xAA, 0xCC };
        public readonly static byte[] CommInitResponse_pkt_header = new byte[] { 0xAA, 0xDD };
        public readonly static byte[] ModuleAlive_pkt_header = new byte[] { 0xAA, 0x44 };


        /* PacketTypeIDs for Commands */
        public readonly static byte[] CommInitAckPkt_TypeID = new byte[] {0x90 };
        public readonly static byte[] CommInitAckPkt_SNo = new byte[] { 0x00 };
        public readonly static byte[] CommInitAckContinue_Sts = new byte[] { 0x00 };

        public static long[] Allowed_Devices=  /* This Array Should be loaded Dynamically When COnfiguration screnn has added */
        {
            0404071616613148,
            0405854056013940,
            0405854056013886,
            0405854054630163,
            0405854056013902,
            0404071600497555
        };
        
        public static void AppendTexttoFile(string filepath,string content)
        {
            if(!File.Exists(filepath))
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
    }
}
