using System;
using System.Collections.Generic;
using System.Text;

namespace BetWin.Game.API.Utils
{
    internal static class EnumExtension
    {
        public static T ToEnum<T>(this string value) where T : struct, IComparable, IFormattable, IConvertible
        {
            if (string.IsNullOrEmpty(value) || !typeof(T).IsEnum) return default;
            Type type = typeof(T);
            return Enum.IsDefined(type, value) ? (T)Enum.Parse(type, value) : default;
        }
    }
}
