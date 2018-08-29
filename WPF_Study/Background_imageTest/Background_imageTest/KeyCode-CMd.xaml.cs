using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for KeyCode_CMd.xaml
    /// </summary>
    public partial class KeyCode_CMd : Window
    {
        public KeyCode_CMd()
        {
            InitializeComponent();
            // Clear the Existed DeviceID's in combobox 
            Device_cmb.Items.Clear();
            // Fill the Items to ComboBox 
            List<byte[]> DeviceIdList = new List<byte[]>();
            for(int i=0;i<= ServerClass.Devices_Buffer.Count-1;i++)
            {
                if(ServerClass.Devices_Buffer[i].IsConnected)
                {
                    DeviceIdList.Add(ServerClass.Devices_Buffer[i].DeviceId);
                }
            }
            for(int i=0;i<= DeviceIdList.Count-1;i++)
            {
                for(int j=0;j<=Global.Allowed_DevicesList.Count-1;j++)
                {
                    if(DeviceIdList[i].SequenceEqual(Global.Allowed_DevicesList[j].Device_Id))
                    {
                        Device_cmb.Items.Add(Global.Allowed_DevicesList[j].Device_name);
                        break;
                    }

                }
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // to Close the KeyCode Dailogue
        }

        private void SendCmd_Click(object sender, RoutedEventArgs e)
        {
            // Validate KeyCode Length Should be 1 to 14 Characters
            try
            {
                string trimed_code = Code_txt.Text.Trim();
                string Sel_Devicename = Device_cmb.SelectedItem as string;
                /* Fetch the Device ID from Devices */
                byte[] Deviceid = new byte[8];

                for(int i=0;i<=Global.Allowed_DevicesList.Count-1;i++)
                {
                    if(Global.Allowed_DevicesList[i].Device_name.Equals(Sel_Devicename))
                    {
                        Global.Allowed_DevicesList[i].Device_Id.CopyTo(Deviceid, 0); 
                        break;
                    }
                }

                bool IsDevice_Connected = false;
                for(int i=0;i<=ServerClass.Devices_Buffer.Count-1;i++)
                {
                    if(ServerClass.Devices_Buffer[i].IsConnected && (ServerClass.Devices_Buffer[i].DeviceId.SequenceEqual(Deviceid))) // Device Has Connection
                    {
                        IsDevice_Connected = true;
                    }
                }
                if (trimed_code == "")
                {
                    // Error Related Port Number cannot be empty.
                    MessageBox.Show("Port number cannot be empty !!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if(!IsDevice_Connected)
                {
                    MessageBox.Show("Device Is not Connected !!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    /* Everything Is Ok I.e Valid port number, Device Has Connection */
                    CommandClass CmdObj = new CommandClass();
                    Deviceid.CopyTo(CmdObj.DeviceId, 0);
                    Global.Command_KeyCodePkt_TypeID.CopyTo(CmdObj.TypeId, 0); // .CopyTo is the Best Method to copy the bytes

                    CmdObj.Status = false;        // Command- Status

                    CmdObj.Senttime = DateTime.Now; // Command-Sentime

                    CmdObj.SentCnt = 0;           // COmmand - SentCnt

                    byte[] databytes=InsertKeyCodeStringasbytes(trimed_code);  // Prepared Command bytes

                    byte[] Final_bytes = GetFinalbytesOfCommand(Deviceid, Global.Command_KeyCodePkt_TypeID, databytes);

                    /* Initialize the CommandData Array */
                    CmdObj.CommandData = new byte[Final_bytes.Length];  // To Store Whole Command prepared byte[] array

                    Final_bytes.CopyTo(CmdObj.CommandData, 0); // Storing  Total Command bytes of CommandObj.

                    Global.Command_KeyCodePkt_TypeID.CopyTo(CmdObj.TypeId, 0);   //Command-TypeID

                    ServerClass.Command_Buffer.Add(CmdObj);
                    
                }

            }
            catch(Exception ex)
            {
                DateTime now_time = DateTime.Now;
                string time = Convert.ToString(now_time);
                Global.AppendTexttoFile(Global.exception_filepath, "Exception Ocuured On SendSmd_Click Click:  " + ex.Message + time); // Logging the Execptions Ocuured

            }

            // DeviceId Validation Whether It Is Connected Or Not. 
        }

        public byte[] GetFinalbytesOfCommand(byte[] Deviceid, byte[] Command_Pkt_TypeID, byte[] databytes)
        {
            /* Prepare PacketObject */
            PacketClass PktObj = FramePacketforCommand(Deviceid, Global.Command_KeyCodePkt_TypeID, databytes);
            /* Calculating Length of the PAcket */
            short length = (short)(Global.Packet_Default_length + PktObj.Data.Length);

            PktObj.Packet_Length = BitConverter.GetBytes(length); // Getting Bytes from checksum.. Length

            ushort Chksum = ServerClass.Calc_ChecksumForCommandObj(PktObj); // Calculates Checksum Of the Packet 

            PktObj.Checksum = BitConverter.GetBytes(Chksum);   //Checksum

            PktObj = ServerClass.Reverse_PacketBytes(PktObj);  // Reversing the Object byte[] arrays

            byte[] Final_bytes = ServerClass.PktObjectToByteArray(PktObj);  // Convering Object to byte[] array.

            return Final_bytes;
        }
        public byte[] InsertKeyCodeStringasbytes(string KeyCode)
        {
            try
            {

           
            byte[] databytes = new byte[Global.keycode_data_length]; //
            int i = 0;
            int j = i + 1;
            int string_len = KeyCode.Length - 1;
            string temp_str;
            int databytes_itr = 0;
            while (j<=string_len)
            {
                if(j<=string_len)
                {
                    temp_str = KeyCode.Substring(i, 2); // Making Two Digits as One Byte.
                    /* Convert String as Byte */
                    byte temp_byte = Convert.ToByte(temp_str);

                    /* Add Byte to Byte Array */
                    databytes[databytes_itr] = temp_byte;

                    databytes_itr = databytes_itr + 1;


                    i = j + 1;
                    j = i + 1;
                    if(j>string_len && i<= string_len)
                        {
                            temp_str = KeyCode.Substring(i, 1);
                            /* Convert String as Byte */
                            byte tmp_byte = Convert.ToByte(temp_str);
                            byte Default_byte = 0x0F;
                            /*Here You Need to Make Digit as 3F like that  Because here you have Single Digit only*/
                            tmp_byte = (byte)(tmp_byte << 4);
                            Default_byte = (byte)(Default_byte >> 4);
                            byte final_byte = (byte)(tmp_byte | Default_byte);
                            /* Add Byte to Byte Array   */
                            databytes[databytes_itr] = final_byte;
                            databytes_itr = databytes_itr + 1;
                            break;
                        }
                }
               
            }
            /* Fill All the Remaning bytes in databytes with FF */
            for(int itr= databytes_itr; itr < Global.keycode_data_length; itr++)
            {
                databytes[itr] = Global.DefaultKeyCode; 
            }

            return databytes;
            }
            catch(Exception ex)
            {
                DateTime now_time = DateTime.Now;
                string time = Convert.ToString(now_time);
                Global.AppendTexttoFile(Global.exception_filepath, "Exception Ocuured On InsertKeyCodeStringasbytes:  " + ex.Message + time); // Logging the Execptions Ocuured

                return null;
            }
        }
        public PacketClass FramePacketforCommand(byte[] Deviceid,byte[] Command_Pkt_TypeID, byte[] databytes)
        {
            PacketClass Obj = new PacketClass();

            Global.Command_pkt_header.CopyTo(Obj.packetHeader, 0); // Packet Header

            Deviceid.CopyTo(Obj.DeviceId, 0);   // Device Id

            Global.SystemPassword.CopyTo(Obj.Password, 0); // System Password

            Command_Pkt_TypeID.CopyTo(Obj.TypeId, 0);  // TypeID

            if (Command_Pkt_TypeID.SequenceEqual(Global.Command_KeyCodePkt_TypeID))
            {
                Global.Command_KeyCodePkt_Sno.CopyTo(Obj.Packet_sno, 0); // Packet Serial Numbr
                Global.Command_KeyCodePkt_ContSts.CopyTo(Obj.Continue_Sts, 0);// Packet Continue Sts

            }

            Obj.Data = new byte[databytes.Length];

            databytes.CopyTo(Obj.Data, 0);            // Data

            return Obj;
           
        }


        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
  
}
