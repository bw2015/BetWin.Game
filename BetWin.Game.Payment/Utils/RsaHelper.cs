using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.Payment.Utils
{
    internal class RsaHelper
    {
        #region 初始化全局参数
        /// <summary>
        /// 公钥
        /// </summary>
        private string? _PublicKeyPem { get; set; } = null;

        /// <summary>
        /// 私钥
        /// </summary>
        public string? _PrivateKeyPem { get; set; } = null;

        /// <summary>
        /// 当前是否使用私钥进行加解密
        /// </summary>
        private bool _IsPrivateKey { get; set; } = false;
        #endregion 结束初始化全局参数

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public RsaHelper()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="publicKeyPem">公钥</param>
        /// <param name="privateKeyPem">私钥</param>
        /// <param name="isprivateKey">true为私钥进行加解密,false为公钥进行加解密</param>
        public RsaHelper(string publicKeyPem, string? privateKeyPem, bool isprivateKey = true)
        {
            _IsPrivateKey = isprivateKey;
            _PublicKeyPem = publicKeyPem;
            _PrivateKeyPem = privateKeyPem;
        }

        #endregion 结束构造函数

        #region 对外公开函数
        /// <summary>
        /// .net格式分钥转换为java公钥
        /// </summary>
        /// <returns></returns>
        public string RSAPublicKeyDotNet2Java(string publicKey)
        {
            return BouncyCastleRSAHelper.RSAPublicKeyDotNet2Java(publicKey);
        }

        /// <summary>
        /// java格式分钥转换为.net公钥
        /// </summary>
        /// <returns></returns>
        public string RSAPublicKeyJava2DotNet(string publicKey)
        {
            return BouncyCastleRSAHelper.RSAPublicKeyJava2DotNet(publicKey);
        }

        /// <summary>
        /// 将net私钥转换成java所用的私钥字符串
        /// </summary>
        /// <param name="privateKeyPath">私钥文件路径</param>
        /// <returns></returns>
        public string RSAPrivateKeyDotNet2Java(string privateKeyPath)
        {
            return BouncyCastleRSAHelper.RSAPrivateKeyDotNet2Java(privateKeyPath);
        }

        /// <summary>
        /// rsa加密
        /// </summary>
        /// <param name="data">需要加密的文本</param>
        /// <returns></returns>
        public string Encrypt(string data)
        {
            if (_IsPrivateKey)
            {
                return BouncyCastleRSAHelper.PrivateKeyEncrypt(_PrivateKeyPem, data);
            }
            else
            {
                return BouncyCastleRSAHelper.EncrytByPublic(_PublicKeyPem, data);
            }
        }

        /// <summary>
        /// rsa解密
        /// </summary>
        /// <param name="encryptText">需要解密的文本</param>
        /// <returns></returns>
        public string Decrypt(string encryptText)
        {
            if (_IsPrivateKey)
            {
                return BouncyCastleRSAHelper.DecryptByPrivate(_PrivateKeyPem, encryptText);
            }
            else
            {
                return BouncyCastleRSAHelper.PublicKeyDecrypt(_PublicKeyPem ?? string.Empty, encryptText);
            }
        }

        /// <summary>
        /// 获取RSA公司秘钥
        /// </summary>
        /// <returns></returns>
        public RSAKey GetNewRasKey()
        {
            return BouncyCastleRSAHelper.GeneratePublicAndPrivateKeyInfo();
        }

        /// <summary>
        /// 使用md5加密验证签名
        /// </summary>
        /// <param name="data">原始数据</param>
        /// <param name="sign">签名</param>
        /// <returns></returns>
        public string md5Signature(string data)
        {
            if (!string.IsNullOrWhiteSpace(data))
            {
                return MD5Helper.MD5Encrypt32(data);
            }
            return "";
        }

        /// <summary>
        /// 使用md5加密验证签名
        /// </summary>
        /// <param name="data">原始数据</param>
        /// <param name="sign">签名</param>
        /// <returns></returns>
        public bool md5Verify(string data, string sign)
        {
            if (sign.ToUpper() == MD5Helper.MD5Encrypt32(data))
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}
