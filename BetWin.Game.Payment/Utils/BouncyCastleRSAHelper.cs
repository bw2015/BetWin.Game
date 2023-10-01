using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;


namespace BetWin.Game.Payment.Utils
{
    internal class BouncyCastleRSAHelper
    {
        /// <summary>
        /// 生成公钥和私钥对
        /// </summary>
        public static RSAKey GeneratePublicAndPrivateKeyInfo()
        {
            RSAKey rSAKey = new RSAKey();
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);
            rSAKey.PrivateKey = rsa.ToXmlString(true);
            rSAKey.PublicKey = rsa.ToXmlString(false);
            return rSAKey;
        }

        /// <summary>
        /// 用私钥给数据进行RSA加密
        /// </summary>
        /// <param name="xmlPrivateKey"> 私钥(XML格式字符串)</param>
        /// <param name="strEncryptString">要加密的数据</param>
        /// <returns> 加密后的数据 </returns>
        public static string PrivateKeyEncrypt(string xmlPrivateKey, string strEncryptString)
        {
            //加载私钥
            RSACryptoServiceProvider privateRsa = new RSACryptoServiceProvider();
            privateRsa.FromXmlString(xmlPrivateKey);

            //转换密钥
            AsymmetricCipherKeyPair keyPair = DotNetUtilities.GetKeyPair(privateRsa);
            IBufferedCipher c = CipherUtilities.GetCipher("RSA/ECB/PKCS1Padding"); //使用RSA/ECB/PKCS1Padding格式
            //第一个参数为true表示加密，为false表示解密；第二个参数表示密钥

            c.Init(true, keyPair.Private);
            byte[] dataToEncrypt = Encoding.UTF8.GetBytes(strEncryptString);
            #region 分段加密
            int bufferSize = (privateRsa.KeySize / 8) - 11;
            byte[] buffer = new byte[bufferSize];
            byte[] outBytes = null;
            //分段加密
            using (MemoryStream input = new MemoryStream(dataToEncrypt))
            using (MemoryStream ouput = new MemoryStream())
            {
                while (true)
                {
                    int readLine = input.Read(buffer, 0, bufferSize);
                    if (readLine <= 0)
                    {
                        break;
                    }
                    byte[] temp = new byte[readLine];
                    Array.Copy(buffer, 0, temp, 0, readLine);
                    byte[] encrypt = c.DoFinal(temp);
                    ouput.Write(encrypt, 0, encrypt.Length);
                }
                outBytes = ouput.ToArray();
            }
            #endregion
            //byte[] outBytes = c.DoFinal(DataToEncrypt);//加密
            string strBase64 = Convert.ToBase64String(outBytes);

            return strBase64;
        }

        /// <summary>
        /// 用公钥给数据进行RSA解密 
        /// </summary>
        /// <param name="xmlPublicKey"> 公钥(XML格式字符串) </param>
        /// <param name="strDecryptString"> 要解密数据 </param>
        /// <returns> 解密后的数据 </returns>
        public static string PublicKeyDecrypt(string xmlPublicKey, string strDecryptString)
        {
            //加载公钥
            RSACryptoServiceProvider publicRsa = new RSACryptoServiceProvider();
            publicRsa.FromXmlString(xmlPublicKey);
            RSAParameters rp = publicRsa.ExportParameters(false);

            //转换密钥
            AsymmetricKeyParameter pbk = DotNetUtilities.GetRsaPublicKey(rp);

            IBufferedCipher c = CipherUtilities.GetCipher("RSA/ECB/PKCS1Padding");
            //第一个参数为true表示加密，为false表示解密；第二个参数表示密钥
            c.Init(false, pbk);
            byte[] outBytes = null;
            byte[] dataToDecrypt = Convert.FromBase64String(strDecryptString);
            #region 分段解密
            int keySize = publicRsa.KeySize / 8;
            byte[] buffer = new byte[keySize];

            using (MemoryStream input = new MemoryStream(dataToDecrypt))
            using (MemoryStream output = new MemoryStream())
            {
                while (true)
                {
                    int readLine = input.Read(buffer, 0, keySize);
                    if (readLine <= 0)
                    {
                        break;
                    }
                    byte[] temp = new byte[readLine];
                    Array.Copy(buffer, 0, temp, 0, readLine);
                    byte[] decrypt = c.DoFinal(temp);
                    output.Write(decrypt, 0, decrypt.Length);
                }
                outBytes = output.ToArray();
            }
            #endregion
            //byte[] outBytes = c.DoFinal(DataToDecrypt);//解密

            string strDec = Encoding.UTF8.GetString(outBytes);
            return strDec;
        }

        /// <summary>
        /// 使用公钥加密，分段加密
        /// </summary>
        /// <param name="content"></param>
        /// <param name="privateKeyPath"></param>
        /// <returns></returns>
        public static string EncrytByPublic(string publicKey, string strEncryptString)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publicKey);
            byte[] originalData = Encoding.UTF8.GetBytes(strEncryptString);
            if (originalData == null || originalData.Length <= 0)
            {
                throw new NotSupportedException();
            }
            if (rsa == null)
            {
                throw new ArgumentNullException();
            }
            byte[] encryContent = null;
            #region 分段加密
            int bufferSize = (rsa.KeySize / 8) - 11;
            byte[] buffer = new byte[bufferSize];
            //分段加密
            using (MemoryStream input = new MemoryStream(originalData))
            using (MemoryStream ouput = new MemoryStream())
            {
                while (true)
                {
                    int readLine = input.Read(buffer, 0, bufferSize);
                    if (readLine <= 0)
                    {
                        break;
                    }
                    byte[] temp = new byte[readLine];
                    Array.Copy(buffer, 0, temp, 0, readLine);
                    byte[] encrypt = rsa.Encrypt(temp, false);
                    ouput.Write(encrypt, 0, encrypt.Length);
                }
                encryContent = ouput.ToArray();
            }
            #endregion
            return Convert.ToBase64String(encryContent);
        }

        /// <summary>
        /// 通过私钥解密，分段解密
        /// </summary>
        /// <param name="content"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public static string DecryptByPrivate(string privateKey, string strDecryptString)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(privateKey);
            byte[] encryptData = Convert.FromBase64String(strDecryptString);
            //byte[] dencryContent = rsa.Decrypt(encryptData, false);
            byte[] dencryContent = null;
            #region 分段解密
            if (encryptData == null || encryptData.Length <= 0)
            {
                throw new NotSupportedException();
            }

            int keySize = rsa.KeySize / 8;
            byte[] buffer = new byte[keySize];

            using (MemoryStream input = new MemoryStream(encryptData))
            using (MemoryStream output = new MemoryStream())
            {
                while (true)
                {
                    int readLine = input.Read(buffer, 0, keySize);
                    if (readLine <= 0)
                    {
                        break;
                    }
                    byte[] temp = new byte[readLine];
                    Array.Copy(buffer, 0, temp, 0, readLine);
                    byte[] decrypt = rsa.Decrypt(temp, false);
                    output.Write(decrypt, 0, decrypt.Length);
                }
                dencryContent = output.ToArray();
            }
            #endregion
            return Encoding.UTF8.GetString(dencryContent);
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string ReadFile(string filePath)
        {
            string content = "";
            if (File.Exists(filePath))
            {
                content = File.ReadAllText(filePath);
                byte[] mybyte = Encoding.UTF8.GetBytes(content);
                content = Encoding.UTF8.GetString(mybyte);
            }
            return content;
        }

        /// <summary>
        /// 将net私钥转换成java所用的私钥字符串
        /// </summary>
        /// <param name="privateKeyPath">私钥文件路径</param>
        /// <returns></returns>
        public static string RSAPrivateKeyDotNet2Java(string privateKeyPath)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(ReadFile(privateKeyPath));
            BigInteger m = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Modulus")[0].InnerText));
            BigInteger exp = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Exponent")[0].InnerText));
            BigInteger d = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("D")[0].InnerText));
            BigInteger p = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("P")[0].InnerText));
            BigInteger q = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Q")[0].InnerText));
            BigInteger dp = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("DP")[0].InnerText));
            BigInteger dq = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("DQ")[0].InnerText));
            BigInteger qinv = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("InverseQ")[0].InnerText));

            RsaPrivateCrtKeyParameters privateKeyParam = new RsaPrivateCrtKeyParameters(m, exp, d, p, q, dp, dq, qinv);

            PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKeyParam);
            byte[] serializedPrivateBytes = privateKeyInfo.ToAsn1Object().GetEncoded();
            return Convert.ToBase64String(serializedPrivateBytes);
        }

        /// <summary>
        /// 将net 公钥转换成java所用的公钥字符串
        /// </summary>
        /// <param name="publicKeyPath">公钥路径</param>
        /// <returns></returns>
        public static string RSAPublicKeyDotNet2Java(string publicKey)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(publicKey);
            BigInteger m = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Modulus")[0].InnerText));
            BigInteger p = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Exponent")[0].InnerText));
            RsaKeyParameters pub = new RsaKeyParameters(false, m, p);

            SubjectPublicKeyInfo publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(pub);
            byte[] serializedPublicBytes = publicKeyInfo.ToAsn1Object().GetDerEncoded();
            return Convert.ToBase64String(serializedPublicBytes);
        }

        /// <summary>
        /// RSA公钥格式转换，java->.net
        /// </summary>
        /// <param name="publicKey">java生成的公钥</param>
        /// <returns></returns>
        public static string RSAPublicKeyJava2DotNet(string publicKey)
        {
            RsaKeyParameters publicKeyParam = (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(publicKey));
            return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent></RSAKeyValue>",
            Convert.ToBase64String(publicKeyParam.Modulus.ToByteArrayUnsigned()),
            Convert.ToBase64String(publicKeyParam.Exponent.ToByteArrayUnsigned()));
        }

        /// <summary>
        /// RSA私钥格式转换，java->.net
        /// </summary>
        /// <param name="privateKey">java生成的RSA私钥</param>
        /// <returns></returns>
        public static string RSAPrivateKeyJava2DotNet(string privateKey)
        {
            RsaPrivateCrtKeyParameters privateKeyParam = (RsaPrivateCrtKeyParameters)PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKey));

            return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
                Convert.ToBase64String(privateKeyParam.Modulus.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.PublicExponent.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.P.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.Q.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.DP.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.DQ.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.QInv.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.Exponent.ToByteArrayUnsigned()));
        }
    }
}
