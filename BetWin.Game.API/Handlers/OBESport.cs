using BetWin.Game.API.Enums;
using BetWin.Game.API.Models;
using BetWin.Game.API.Requests;
using BetWin.Game.API.Responses;
using BetWin.Game.API.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BetWin.Game.API.Handlers
{
    [Description("OB电竞")]
    public class OBESport : GameBase
    {
        public OBESport(string jsonString) : base(jsonString)
        {
        }

        public override Dictionary<LanguageType, string> Languages => throw new NotImplementedException();

        public override Dictionary<GameCurrency, string> Currencies => throw new NotImplementedException();

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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        internal override HttpResult RequestAPI(GameRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
