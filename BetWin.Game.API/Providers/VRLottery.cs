using BetWin.Game.API.Enums;
using BetWin.Game.API.Models;
using BetWin.Game.API.Requests;
using BetWin.Game.API.Responses;
using BetWin.Game.API.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BetWin.Game.API.Providers
{
    /// <summary>
    /// VR彩票
    /// </summary>
    public class VRLottery : GameBase
    {
        #region ========  配置参数  ========

        [Description("API请求网关")]
        public string gateway { get; set; }

        [Description("商户ID")]
        public string merId { get; set; }

        [Description("版本号")]
        public string version { get; set; } = "1.0";

        [Description("加解密金鑰")]
        public string key { get; set; }

        [Description("登录网关")]
        public string loginUrl { get; set; }

        #endregion

        public VRLottery(string jsonString) : base(jsonString)
        {
        }

        public override Dictionary<GameLanguage, string> Languages => new Dictionary<GameLanguage, string>();

        public override Dictionary<GameCurrency, string> Currencies => new Dictionary<GameCurrency, string>();


        public override BalanceResponse Balance(BalanceModel request)
        {
            throw new NotImplementedException();
        }

        public override CheckTransferResponse CheckTransfer(CheckTransferModel request)
        {
            throw new NotImplementedException();
        }

        public override OrderResult GetOrder(QueryOrderModel request)
        {
            throw new NotImplementedException();
        }

        public override LoginResponse Login(LoginModel request)
        {
            string data = $"playerName={request.PlayerName}&loginTime={DateTime.Now.GetTimeZone(0):yyyy-MM-ddTHH:mm:ssZ}";
            string sign = this.encrypt(data);

            return new LoginResponse(GameResultCode.Success)
            {
                Method = LoginMethod.Redirect,
                Url = $"{this.loginUrl}?version={this.version}&id={this.merId}&data={HttpUtility.UrlEncode(sign)}"
            };
        }

        public override LogoutResponse Logout(LogoutModel request)
        {
            throw new NotImplementedException();
        }

        public override RegisterResponse Register(RegisterModel request)
        {
            throw new NotImplementedException();
        }

        public override TransferResponse Transfer(TransferModel request)
        {
            throw new NotImplementedException();
        }

        protected override GameResultCode GetResultCode(string result, out string message)
        {
            JObject info = JObject.Parse(result);
            int errorCode = info["errorCode"]?.Value<int>() ?? 0;

            message = errorCode switch
            {
                0 => "成功",
                1 => "版本错误",
                2 => "找不到对应的界面",
                3 => "系统错误",
                4 => "网络传送失败",
                5 => "加密失败",
                6 => "解密失败",
                7 => "没有此商户",
                8 => "没有此玩家",
                9 => "新增玩家失败",
                10 => "余额不足",
                11 => "URL 错误",
                12 => "此玩家被禁用",
                13 => "玩家钱包转点 - 种类错误",
                14 => "玩家钱包转点 - 订单号重复",
                15 => "玩家钱包转点 - 订单号内容错误",
                16 => "玩家钱包转点 - 订单建立时间过太久",
                17 => "玩家钱包转点 - 金额错误",
                18 => "新增玩家失败，失败原因是此玩家已经存在。",
                999 => "未知错误",
                _ => errorCode.ToString()
            };

            return errorCode switch
            {
                0 => GameResultCode.Success,
                5 => GameResultCode.SignInvalid,
                6 => GameResultCode.SignInvalid,
                7 => GameResultCode.NoMerchant,
                8 => GameResultCode.NoPlayer,
                9 => GameResultCode.PlayerNameInvalid,
                10 => GameResultCode.NoBalance,
                12 => GameResultCode.PlayerLocked,
                13 => GameResultCode.TransferInvalid,
                14 => GameResultCode.TransferDuplicate,
                15 => GameResultCode.TransferInvalid,
                16 => GameResultCode.TransferInvalid,
                17 => GameResultCode.TransferInvalid,
                18 => GameResultCode.DuplicatePlayerName,
                _ => GameResultCode.Error
            };

        }

        internal override HttpResult RequestAPI(GameRequest request)
        {
            string url = $"{this.gateway}{request.Url}";
            HttpResult result = NetAgent.PostAsync(url, (string)request.Data).Result;
            throw new NotImplementedException();
        }

        #region ========  工具方法  ========

        /// <summary>
        /// 加密数据
        /// </summary>
        private string getPostData(Dictionary<string, object> data)
        {
            if (!data.Any()) return string.Empty;
            Dictionary<string, string> result = new Dictionary<string, string>()
            {
                {"version",this.version },
                {"id",this.merId },
                { "data", HttpUtility.UrlEncode(this.encrypt(data.ToJson())) }
            };
            return string.Join("&", result.Select(t => $"{t.Key}={t.Value}"));
        }

        /// <summary>
        /// 加密
        /// </summary>
        private string encrypt(string data)
        {
            byte[] keyArray = Encoding.ASCII.GetBytes(this.key);
            byte[] toEncryptArray = Encoding.ASCII.GetBytes(data);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            //rDel.BlockSize = 128;
            //rDel.KeySize = 256;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray);
        }

        /// <summary>
        /// 解密
        /// </summary>
        public string? decrypt(string data)
        {
            if (string.IsNullOrEmpty(data)) return null;
            byte[] toEncryptArray = Convert.FromBase64String(data);

            RijndaelManaged rm = new RijndaelManaged
            {
                Key = Encoding.ASCII.GetBytes(this.key),
                Mode = CipherMode.ECB,
                //KeySize = 256,
                //BlockSize = 128,
                Padding = PaddingMode.PKCS7
            };
            ICryptoTransform cTransform = rm.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Encoding.UTF8.GetString(resultArray);
        }

        #endregion
    }
}
