using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CN.SMSSDK.Sharp
{
    public static class AESHelper
    {
        public const string DefaultKey = "sdk.commonap.sdk";
        public const string DefaultIV = "sdk.commonap.sdk";
        public static string EncryptString(string str, string key = DefaultKey, string iv = DefaultIV)
        {
            Aes aes = Aes.Create();
            byte[] inputByteArray = Encoding.Default.GetBytes(str);
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = ASCIIEncoding.UTF8.GetBytes(key);// 密匙
            aes.IV = ASCIIEncoding.UTF8.GetBytes(iv);// 初始化向量
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            var retB = Convert.ToBase64String(ms.ToArray());
            return retB;
        }

        //解密
        public static string DecryptString(string str, string key = DefaultKey, string iv = DefaultIV)
        {
            Aes aes = Aes.Create();
            byte[] inputByteArray = Convert.FromBase64String(str);
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = ASCIIEncoding.UTF8.GetBytes(key);
            aes.IV = ASCIIEncoding.UTF8.GetBytes(iv);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            // 如果两次密匙不一样，这一步可能会引发异常
            cs.FlushFinalBlock();
            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}
