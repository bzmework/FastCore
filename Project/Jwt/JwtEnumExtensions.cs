using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace FastCore.Jwt
{
    /// <summary>
    /// JWT枚举扩展
    /// </summary>
    internal static class JwtEnumExtensions
    {
        /// <summary>
        /// 获得JwtSecurityAlgorithms当前枚举值
        /// </summary>
        public static string ToStr(this JwtSecurityAlgorithms value)
        {
            return GetDescription(value);
        }

        /// <summary>
        /// 获得JwtClaimNames当前枚举值
        /// </summary>
        public static string ToStr(this JwtClaimNames value)
        {
            return GetDescription(value);
        }

        /// <summary>
        /// 获得描述特性
        /// </summary>
        private static string GetDescription(object value)
        {
            return value.GetType()
                 .GetField(value.ToString())
                 .GetCustomAttribute<DescriptionAttribute>()?.Description ?? value.ToString();
        }
    }
}
