using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace METCSV
{
    static class Global
    {
        public static bool downloadNow = true;

        public static bool ShowProfitsWindows { get; set; }

        public static string Encrypt(string textToDecrypt)
        {
            if (textToDecrypt == null) return "";

            string EncryptionKey = "SDLKAJWDOGOJEPIASHEKANDKJASDPOIJAPWDOKNASPFPAUJEHAKSDHNKLHAWDE";
            byte[] clearBytes = Encoding.Unicode.GetBytes(textToDecrypt);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    textToDecrypt = Convert.ToBase64String(ms.ToArray());
                }
            }
            return textToDecrypt;
        }

        public static string Decrypt(string textToDecrypt)
        {
            if (textToDecrypt == null) return "";

            //if (textToDecrypt.EndsWith("="))
            //    textToDecrypt.Remove(textToDecrypt.Length - 1);

            string EncryptionKey = "SDLKAJWDOGOJEPIASHEKANDKJASDPOIJAPWDOKNASPFPAUJEHAKSDHNKLHAWDE";
            byte[] cipherBytes = Convert.FromBase64String(textToDecrypt);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    textToDecrypt = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return textToDecrypt;
        }
    }
}
