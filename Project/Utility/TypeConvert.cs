using System;
using System.ComponentModel;
using System.Globalization;

namespace FastCore
{
	/// <summary>
	/// 类型转换
	/// </summary>
	public static class TypeConvert
    {
		/// <summary>
		/// 将object类型转换成指定的类型
		/// </summary>
		/// <typeparam name="T">目标类型</typeparam>
		/// <param name="value">转换的值</param>
		/// <returns>转换后的目标类型值，转换失败则返回目标类型的默认值</returns>
		public static T C2Type<T>(object value)
		{
			// null返回默认值
			if (value == null)
			{
				return default(T);
			}

			Type destType = typeof(T); // 目标类型
			Type valueType = value.GetType(); // 值类型

			// 类型相同直接转换
			if (valueType == destType)
			{
				return (T)value;
			}

			// 接口、抽象、泛型不支持类型转换
			if (destType.IsInterface || destType.IsAbstract || destType.IsGenericTypeDefinition)
			{
				return default(T);
			}

			// 开始类型转换
			object outValue; 
			TypeCode typeCode = Type.GetTypeCode(destType);
			try
			{
				// 字符串->任意类型
				if (value is string str)
				{
					if (destType == typeof(byte[]))
					{
						outValue = ObjectConvert.C2Bytes(str);
					}
					else
					{
						switch (typeCode)
						{
							case TypeCode.Object: outValue = ObjectConvert.C2Obj<T>(str); break;
							case TypeCode.DateTime: outValue = ObjectConvert.C2Date(str); break;
							case TypeCode.Decimal: outValue = ObjectConvert.C2Dec(str); break;
							case TypeCode.Double: outValue = ObjectConvert.C2Dbl(str); break;
							case TypeCode.Single: outValue = ObjectConvert.C2Flt(str); break;
							case TypeCode.Int64: outValue = ObjectConvert.C2Lng(str); break;
							case TypeCode.Int32: outValue = ObjectConvert.C2Int(str); break;
							case TypeCode.Int16: outValue = ObjectConvert.C2Short(str); break;
							case TypeCode.UInt64: outValue = ObjectConvert.C2Lng(str); break;
							case TypeCode.UInt32: outValue = ObjectConvert.C2Int(str); break;
							case TypeCode.UInt16: outValue = ObjectConvert.C2Short(str); break;
							case TypeCode.Byte: outValue = ObjectConvert.C2Byte(str); break;
							case TypeCode.SByte: outValue = ObjectConvert.C2SByte(str); break;
							case TypeCode.Char: outValue = ObjectConvert.C2Chr(str); break;
							case TypeCode.Boolean: outValue = ObjectConvert.C2Bool(str); break;
							default: outValue = null; break;
						}
					}
					if (outValue != null)
					{
						return (T)outValue;
					}
				}

				// 字节数组->任意类型
				if (value is byte[] byts)
				{
					switch (typeCode)
					{
						case TypeCode.Object: outValue = ObjectConvert.C2Obj<T>(byts); break;
						case TypeCode.String: outValue = ObjectConvert.C2Str(byts); break;
						case TypeCode.DateTime: outValue = ObjectConvert.C2Date(byts); break;
						case TypeCode.Decimal: outValue = ObjectConvert.C2Dec(byts); break;
						case TypeCode.Double: outValue = ObjectConvert.C2Dbl(byts); break;
						case TypeCode.Single: outValue = ObjectConvert.C2Flt(byts); break;
						case TypeCode.Int64: outValue = ObjectConvert.C2Lng(byts); break;
						case TypeCode.Int32: outValue = ObjectConvert.C2Int(byts); break;
						case TypeCode.Int16: outValue = ObjectConvert.C2Short(byts); break;
						case TypeCode.UInt64: outValue = ObjectConvert.C2Lng(byts); break;
						case TypeCode.UInt32: outValue = ObjectConvert.C2Int(byts); break;
						case TypeCode.UInt16: outValue = ObjectConvert.C2Short(byts); break;
						case TypeCode.Byte: outValue = ObjectConvert.C2Byte(byts); break;
						case TypeCode.SByte: outValue = ObjectConvert.C2SByte(byts); break;
						case TypeCode.Char: outValue = ObjectConvert.C2Chr(byts); break;
						case TypeCode.Boolean: outValue = ObjectConvert.C2Bool(byts); break;
						default: outValue = null; break;
					}
					if (outValue != null)
					{
						return (T)outValue;
					}
				}

				// 字符串或数值->枚举型
				destType = Nullable.GetUnderlyingType(destType) ?? destType;
				if (destType.IsEnum)
				{
					// 转换成枚举型
					if (value is string)
					{
						outValue = Enum.Parse(destType, (string)value, true);
						return (T)outValue;
					}
					if (ObjectCheck.IsInteger(value))
					{
						outValue = (T)Enum.ToObject(destType, value);
						return (T)outValue;
					}
				}
			}
			catch
			{
				outValue = null;
			}

			try
			{
				// 尝试转换value到T
				var tc1 = TypeDescriptor.GetConverter(typeof(T));
				if (tc1.CanConvertFrom(value.GetType()))
				{
					return (T)tc1.ConvertFrom(value);
				}

				// 尝试转换value到T
				var tc2 = TypeDescriptor.GetConverter(value.GetType());
				if (tc2.CanConvertTo(typeof(T)))
				{
					return (T)tc2.ConvertTo(value, typeof(T));
				}

				// 尝试改变成目标类型
				outValue = Convert.ChangeType(value, destType, CultureInfo.InvariantCulture);
				return (T)outValue;
			}
			catch
			{
				// 再次尝试转换成目标类型
				if (destType.IsAssignableFrom(valueType))
				{
					return (T)value;
				}
			}

			// 转换失败返回默认值
			return default(T);
		}

	}
}
