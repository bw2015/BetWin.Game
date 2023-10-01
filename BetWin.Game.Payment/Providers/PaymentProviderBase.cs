using BetWin.Game.Payment.Exceptions;
using BetWin.Game.Payment.Handlers;
using BetWin.Game.Payment.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SP.StudioCore.Ioc;
using SP.StudioCore.Net.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace BetWin.Game.Payment.Providers
{
    public abstract class PaymentProviderBase : IPaymentProvider
    {
        public PaymentProviderBase(string setting)
        {
            if (string.IsNullOrEmpty(setting) || !setting.StartsWith("{")) return;
            JsonConvert.PopulateObject(setting, this);
        }

        protected IPaymentHandler? handler
            => IocCollection.GetService<IPaymentHandler>();

        protected abstract PaymentResponse payment(PaymentRequest request, out string postData, out HttpClientResponse result);

        public PaymentResponse Payment(PaymentRequest request)
        {
            PaymentResponse? response = default;
            string postData = string.Empty;
            HttpClientResponse result = default;
            try
            {
                response = this.payment(request, out postData, out result);
            }
            finally
            {
                if (response != null)
                {
                    this.handler?.SaveLog(this.GetType(), request, response, postData, result);
                }
            }
            return response;
        }

        public override string ToString()
        {
            List<object> list = new List<object>();
            foreach (PropertyInfo property in this.GetType().GetProperties())
            {
                list.Add(new
                {
                    property.Name,
                    Value = property.GetValue(this, null),
                    Description = property.GetCustomAttribute<DescriptionAttribute>()?.Description ?? property.Name,
                    Type = property.PropertyType.FullName
                });
            }
            return JsonConvert.SerializeObject(list);
        }

        protected abstract CallbackResponse callback(HttpContext context);

        public CallbackResponse Callback(HttpContext context)
        {
            CallbackResponse? response = null;
            try
            {
                response = this.callback(context);
            }
            finally
            {
                if (response != null)
                {
                    this.handler?.SaveCallback(response.Value, context);
                }
            }
            return response ?? default;
        }
    }
}
