using System;
using System.Security.Cryptography;
using System.Text;

namespace RTLSWebService.Common
{
    public class AES
    {
        /*
    * Reference Links
    * http://stackoverflow.com/questions/2090765/encryption-compatable-between-android-and-c-sharp
    * http://lamahashim.blogspot.in/2009/08/encyptiondecryption-in-c-and-java.html
    * 
    */

        private static string passphrase = "Eff&TataRakshak@1985";
        private ICryptoTransform rijndaelDecryptor;
        private ICryptoTransform rijndaelEncryptor;
        // Replace me with a 16-byte key, share between Java and C#       (vEmploy@VLNAPP16)
        private static byte[] rawSecretKey =  {0x46, 0x45, 0x6d, 0x70, 0x6c, 0x6f, 0x69, 0x50,
                                               0x45, 0x4c, 0x4e, 0x41, 0x50, 0x50, 0x31, 0x36};

        public AES()
        {
            byte[] passwordKey = encodeDigest(passphrase);
            RijndaelManaged rijndael = new RijndaelManaged();
            rijndaelDecryptor = rijndael.CreateDecryptor(passwordKey, rawSecretKey);
            rijndaelEncryptor = rijndael.CreateEncryptor(passwordKey, rawSecretKey);
        }

        public string Encrypt(string inputdata)
        {
            byte[] plaindataBytes = Encoding.UTF8.GetBytes(inputdata);
            return Convert.ToBase64String(rijndaelEncryptor.TransformFinalBlock(plaindataBytes, 0, plaindataBytes.Length));
        }

        private string Decrypt(byte[] encryptedData)
        {
            byte[] newClearData = rijndaelDecryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
            return Encoding.ASCII.GetString(newClearData);
        }

        public string DecryptFromBase64(string encryptedBase64)
        {
            try
            {
                return Decrypt(Convert.FromBase64String(encryptedBase64));
            }
            catch (Exception convertExp)
            {
                return convertExp.Message;
            }
        }

        private byte[] encodeDigest(string text)
        {
            MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] data = Encoding.ASCII.GetBytes(text);
            return x.ComputeHash(data);
        }

    }

}
