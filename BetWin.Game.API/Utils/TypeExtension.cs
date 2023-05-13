using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BetWin.Game.API.Utils
{
    internal static class TypeExtension
    {
        public static bool HasAttribute<T>(this Object obj) where T : Attribute
        {
            ICustomAttributeProvider custom = obj is ICustomAttributeProvider ? (ICustomAttributeProvider)obj : (ICustomAttributeProvider)obj.GetType();
            foreach (var t in custom.GetCustomAttributes(false))
            {
                if (t.GetType().Equals(typeof(T))) return true;
            }
            return false;
        }
    }
}
