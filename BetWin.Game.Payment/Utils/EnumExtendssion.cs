using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace BetWin.Game.Payment.Utils
{
    internal static class EnumExtendssion
    {
        public static string GetDescription(this Enum @enum)
        {
            string value = @enum.ToString();
            FieldInfo field = @enum.GetType().GetField(value);
            object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);    //获取描述属性

            if (objs == null || objs.Length == 0) return value;
            DescriptionAttribute descriptionAttribute = (DescriptionAttribute)objs[0];
            return descriptionAttribute.Description;
        }
    }
}
