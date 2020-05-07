using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FastCore
{
    /// <summary>
    /// 各种对象校验
    /// </summary>
    public static class ObjectCheck
    {
		/// <summary>
		/// 是否为整型
		/// </summary>
		/// <param name="value">要判定的值</param>
		/// <returns></returns>
		public static bool IsInteger(object value)
		{
			if(value == null)
				return false;

			switch (Type.GetTypeCode(value.GetType()))
			{
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
					return true;
				default:
					return int.TryParse(value.ToString(), out _);
			}
		}

		/// <summary>
		/// 是否为数字
		/// </summary>
		/// <param name="value">要判定的值</param>
		/// <returns></returns>
		public static bool IsNumeric(object value)
		{
			if (value == null)
				return false;

			switch (Type.GetTypeCode(value.GetType()))
			{
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
					return true;
				default:
					return double.TryParse(value.ToString(), out _);
			}
		}

		/// <summary>
		/// 是否为日期
		/// </summary>
		/// <param name="value">要判定的值</param>
		/// <returns></returns>
		public static bool IsDate(object value)
		{
			if(value == null)
			{
				return false;
			}

			if (Type.GetTypeCode(value.GetType()) == TypeCode.DateTime)
			{
				return true;
			}
			else
			{
				try
				{
					string text = string.Format(CultureInfo.InvariantCulture, value.ToString(), "yyyy-MM-dd");
					return !string.IsNullOrEmpty(text);
				}
				catch
				{
					return false;
				}
			}
		}

		/// <summary>判断字符串是否为String.Empty</summary>
		/// <param name="value">字符串</param>
		/// <returns></returns>
		public static bool IsEmpty(string value)
        {
            return value != null && value.Length <= 0;
        }

		/// <summary>判断字符串是否为null或String.Empty</summary>
		/// <param name="value">字符串</param>
		/// <returns></returns>
		public static bool IsNullOrEmpty(string value)
		{
			return value == null || value.Length <= 0;
		}

		/// <summary>字符串是否空或者空白字符串</summary>
		/// <param name="value">字符串</param>
		/// <returns></returns>
		public static bool IsNullOrWhiteSpace(string value)
        {
            if (value != null)
            {
                for (var i = 0; i < value.Length; i++)
                {
                    if (!char.IsWhiteSpace(value[i])) return false;
                }
            }
            return true;
        }

		/// <summary>字符串是否采用Base64编码</summary>
		/// <remarks>https://stackoverflow.com/questions/8571501/how-to-check-whether-a-string-is-base64-encoded-or-not</remarks>
		/// <param name="value">字符串</param>
		/// <returns></returns>
		public static bool IsBase64(string value)
		{
			// Base64编码后的特点：
			// 符串只可能包含A-Z，a-z，0-9，+，/，= 字符
			// 字符串长度是4的倍数
			// =只会出现在字符串最后，可能没有或者一个等号或者两个等号

			string pattern = "^([A-Za-z0-9+/]{4})*([A-Za-z0-9+/]{4}|[A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)$";
			//string pattern = @"^[a-zA-Z0-9\+/]*={0,3}$";

			if (string.IsNullOrEmpty(value))
			{
				return false;
			}
			else
			{
				if (value.Length % 4 != 0)
				{
					return false;
				}
				else if (!Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase))
				{
					return false;
				}
			}

			return true;
		}
	}
}
