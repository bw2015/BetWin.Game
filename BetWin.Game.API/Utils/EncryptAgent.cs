using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace BetWin.Game.API.Utils
{
    /// <summary>
    /// 常用加密算法
    /// </summary>
    internal static class EncryptAgent
    {
        /// <summary>
        /// 对字符串进行MD5加密
        /// </summary>
        /// <param name="text"></param>
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
        /// AES加密
        /// </summary>
        internal static string AesEncrypt(string source, string key, string? iv = null, CipherMode mode = CipherMode.ECB)
        {
            if (string.IsNullOrEmpty(source)) return null;
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(source);
            byte[] keyData = Encoding.UTF8.GetBytes(key);
            using (RijndaelManaged rm = new RijndaelManaged()
            {
                Key = keyData,
                Mode = mode,
                Padding = PaddingMode.PKCS7
            })
            {
                if (iv != null)
                {
                    rm.IV = Encoding.UTF8.GetBytes(iv);
                }

                ICryptoTransform cTransform = rm.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                return Convert.ToBase64String(resultArray);
            }
        }

        /// <summary>
        ///  AES 解密（如果解密失败则返回null）
        /// </summary>
        /// <param name="str">明文（待解密）</param>
        /// <param name="key">密文</param>
        /// <returns></returns>
        public static string? AesDecrypt(string str, string key, string? iv = null, CipherMode mode = CipherMode.ECB)
        {
            if (string.IsNullOrEmpty(str)) return null;
            try
            {
                byte[] toEncryptArray = Convert.FromBase64String(str);

                using (RijndaelManaged rm = new RijndaelManaged()
                {
                    Key = Encoding.UTF8.GetBytes(key),
                    Mode = mode,
                    Padding = PaddingMode.PKCS7
                })
                {
                    if (iv != null)
                    {
                        rm.IV = Encoding.UTF8.GetBytes(iv);
                    }
                    ICryptoTransform cTransform = rm.CreateDecryptor();
                    byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                    return Encoding.UTF8.GetString(resultArray);
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
