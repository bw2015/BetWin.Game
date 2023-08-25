using BetWin.Game.Payment.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.Payment.Providers
{
    public interface IPaymentProvider
    {
        PaymentResponse Payment(PaymentRequest request);

        
    }
}
