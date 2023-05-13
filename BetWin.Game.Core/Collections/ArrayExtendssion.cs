using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.Core.Collections
{
    /// <summary>
    /// 数组扩展
    /// </summary>
    public static class ArrayExtendssion
    {
        public static T Get<T>(this string[] args, string argName, T defaultValue)
        {
            int index = Array.IndexOf(args, argName);
            if (index == -1 || args.Length <= index + 1) return defaultValue;
            string value = args[index + 1];

            return value.GetValue<T>() ?? defaultValue;
        }

        public static string Get(this string[] args, string argName)
        {
            return args.Get(argName, string.Empty);
        }
    }
}
