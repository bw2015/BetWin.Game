using BetWin.Game.Payment.Handlers;
using BetWin.Game.Payment.Providers;
using BetWin.Game.Payment.Withdraws;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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


        #region ========  支付供应商  ========


        public static IPaymentProvider? GetPaymentProvider(string provider, string setting)
        {
            Assembly assembly = typeof(PaymentFactory).Assembly;
            Type type = assembly.GetType($"{typeof(IPaymentProvider).Namespace}.{provider}");
            if (type == null) return null;
            return (IPaymentProvider)Activator.CreateInstance(type, new[] { setting });
        }

        /// <summary>
        /// 获取系统中所有定义的支付供应商
        /// </summary>
        public static IEnumerable<Type> GetPaymentProviders()
        {
            var assembly = typeof(PaymentProviderBase).Assembly;
            return assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(PaymentProviderBase)) && !t.IsAbstract);
        }

        #endregion

        #region ========  代付供应商  ========

        /// <summary>
        /// 获取系统中所有定义的代付供应商
        /// </summary>
        public static IEnumerable<Type> GetWithdrawProviders()
        {
            var assembly = typeof(PaymentProviderBase).Assembly;
            return assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(WithdrawProviderBase)) && !t.IsAbstract);
        }


        public static IWithdrawProvider? GetWithdrawProvider(string provider, string setting)
        {
            Assembly assembly = typeof(PaymentFactory).Assembly;
            Type type = assembly.GetType($"{typeof(IWithdrawProvider).Namespace}.{provider}");
            if (type == null) return null;
            return (IWithdrawProvider)Activator.CreateInstance(type, new[] { setting });
        }

        #endregion
    }
}
