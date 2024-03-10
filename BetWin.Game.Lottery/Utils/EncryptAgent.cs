using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BetWin.Game.Lottery.Utils
{
    internal static class EncryptAgent
    {
        /// <summary>
        /// 对字符串进行MD5加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns>32位 大写</returns>
        public static string toMD5(this string input, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = encoding.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
        public static string AESDecrypt(this string base64String, string keyString, string ivString, CipherMode mode = CipherMode.CBC, PaddingMode padding = PaddingMode.PKCS7)
        {
            byte[] cipherText = Convert.FromBase64String(base64String);
            byte[] key = Encoding.UTF8.GetBytes(keyString);
            byte[] iv = Encoding.UTF8.GetBytes(ivString);
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;
                aesAlg.Mode = mode;
                aesAlg.Padding = padding;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using MemoryStream msDecrypt = new MemoryStream(cipherText);
                using CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                using StreamReader srDecrypt = new StreamReader(csDecrypt);
                return srDecrypt.ReadToEnd();
            }
        }
    }
}
