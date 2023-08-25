using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.Payment.Exceptions
{
    public class PaymentException : Exception
    {
        public PaymentError Error { get; private set; }

        public PaymentException(PaymentError error) : base(error.ToString())
        {
            this.Error = error;
        }
    }
}
