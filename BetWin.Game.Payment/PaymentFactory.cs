using BetWin.Game.Payment.Handlers;
using BetWin.Game.Payment.Providers;
using Newtonsoft.Json;
using SP.StudioCore.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace BetWin.Game.Payment
{
    public static class PaymentFactory
    {
        internal static IPaymentHandler? handler => IocCollection.GetService<IPaymentHandler>();


        public static IPaymentProvider? GetPaymentProvider(int provderId, string setting, string providerSetting = "{}")
        {
            string? providerCode = handler?.GetProviderCode(provderId);
            if (providerCode == null) return null;
            return GetPaymentProvider(providerCode, setting, providerSetting);
        }

        public static IPaymentProvider? GetPaymentProvider(string provider, string setting, string providerSetting = "{}")
        {
            Assembly assembly = typeof(PaymentFactory).Assembly;
            Type type = assembly.GetType($"{typeof(IPaymentProvider).Namespace}.{provider}");
            if (type == null) return null;

            Dictionary<string, string> settingValue = JsonConvert.DeserializeObject<Dictionary<string, string>>(setting ?? "{}") ?? new Dictionary<string, string>();
            Dictionary<string, string> providerValue = JsonConvert.DeserializeObject<Dictionary<string, string>>(providerSetting) ?? new Dictionary<string, string>();

            foreach (var item in providerValue)
            {
                if (settingValue.ContainsKey(item.Key))
                {
                    settingValue[item.Key] = item.Value;
                }
                else
                {
                    settingValue.Add(item.Key, item.Value);
                }
            }

            return (IPaymentProvider)Activator.CreateInstance(type, new[] { JsonConvert.SerializeObject(settingValue) });
        }

        /// <summary>
        /// 获取系统中所有定义的支付供应商
        /// </summary>
        public static IEnumerable<Type> GetPaymentProviders()
        {
            var assembly = typeof(PaymentProviderBase).Assembly;
            return assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(PaymentProviderBase)) && !t.IsAbstract);
        }
    }
}
