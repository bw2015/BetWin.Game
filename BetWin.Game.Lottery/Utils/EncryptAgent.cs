using System;
using System.Collections.Generic;
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
    }
}
