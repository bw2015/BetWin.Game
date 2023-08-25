using BetWin.Game.Payment.Exceptions;
using BetWin.Game.Payment.Handlers;
using BetWin.Game.Payment.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SP.StudioCore.Ioc;
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

        public abstract PaymentResponse Payment(PaymentRequest request);

        /// <summary>
        /// 回调
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual bool Callback(HttpContext context)
        {
            throw new NotImplementedException();
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
    }
}
