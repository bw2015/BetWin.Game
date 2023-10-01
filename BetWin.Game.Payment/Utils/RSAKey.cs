using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.Payment.Utils
{
    /// <summary>
    /// RSA加密的密匙结构  公钥和私匙
    /// </summary>
    internal struct RSAKey
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
    }
}
