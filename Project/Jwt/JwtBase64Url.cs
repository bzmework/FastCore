using System;
using System.Collections.Generic;
using System.Text;

namespace FastCore.Jwt
{
    /// <summary>
    /// JwtBase64Url编码/解码
    /// </summary>
    public static class JwtBase64Url
    {

        /// <summary>
        /// 编码
        /// </summary>
        /// <param name="input">待编码的数据</param>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <returns>编码后的字符串</returns>
        public static string Encode(byte[] input)
        {
            if (input is null)
                throw new ArgumentNullException(nameof(input));
            if (input.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(input));

            var output = Convert.ToBase64String(input);
            output = output.Split('=')[0]; // 删除任何尾随的'='
            output = output.Replace('+', '-'); // 将'+'替换成'-'
            output = output.Replace('/', '_'); // 将'/'替换成'_'
            return output;
        }

        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="input">待编码的数据</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="FormatException" />
        /// <returns>解码编码后的数据</returns>
        public static byte[] Decode(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException(nameof(input));

            var output = input;
            output = output.Replace('-', '+'); // 将'-'还原成'+'
            output = output.Replace('_', '/'); // 将'_'还原成'/'
            switch (output.Length % 4) // 尾部用'='填充
            {
                case 0:
                    break; // 不需要填充'='
                case 2:
                    output += "==";
                    break; // 填充两个'='
                case 3:
                    output += "=";
                    break; // 填充一个'='
                default:
                    throw new FormatException("非法的base64url字符串");
            }

            var converted = Convert.FromBase64String(output); 
            return converted;
        }
    }
}



