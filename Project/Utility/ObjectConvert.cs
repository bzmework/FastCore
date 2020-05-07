using System;
using System.Globalization;
using System.Text;
using FastCore.Json;

namespace FastCore
{
    /// <summary>
    /// 对象转换
    /// </summary>
    public static class ObjectConvert
    {
		#region object转换成系统的基本类型(bool, sbyte, byte, short, int, long, float, double, decimal, datetime, string)

		/// <summary>
		/// 将object转换成bool(System.Boolean)
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>转换后的值</returns>
		public static bool C2Bool(object value)
		{
			if (value == null) return false;
			try
			{
				return System.Convert.ToBoolean(value, CultureInfo.InvariantCulture);
			}
			catch
			{
				return C2Bool(value.ToString());
			}
		}

		/// <summary>
		/// 将object转换成sbyte(System.SByte)
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>转换后的值</returns>
		public static sbyte C2SByte(object value)
		{
			if (value == null) return 0;
			try
			{
				return Convert.ToSByte(value, CultureInfo.InvariantCulture);
			}
			catch
			{
				return C2SByte(value.ToString());
			}
		}

		/// <summary>
		/// 将object转换成sbyte(System.Byte)
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>转换后的值</returns>
		public static byte C2Byte(object value)
		{
			if (value == null) return 0;
			try
			{
				return Convert.ToByte(value, CultureInfo.InvariantCulture);
			}
			catch
			{
				return C2Byte(value.ToString());
			}
		}

		/// <summary>
		/// 将object转换成short(System.Int16)
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>转换后的值</returns>
		public static short C2Short(object value)
		{
			if (value == null) return 0;
			try
			{
				return Convert.ToInt16(value, CultureInfo.InvariantCulture);
			}
			catch
			{
				return C2Short(value.ToString());
			}
		}

		/// <summary>
		/// 将object转换成ushort(System.UInt16)
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>转换后的值</returns>
		public static ushort C2UShort(object value)
		{
			if (value == null) return 0;
			try
			{
				return Convert.ToUInt16(value, CultureInfo.InvariantCulture);
			}
			catch
			{
				return C2UShort(value.ToString());
			}
		}

		/// <summary>
		/// 将object转换成int(System.Int32)
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>转换后的值</returns>
		public static int C2Int(object value)
		{
			if (value == null) return 0;
			try
			{
				return Convert.ToInt32(value, CultureInfo.InvariantCulture);
			}
			catch
			{
				return C2Int(value.ToString());
			}
		}

		/// <summary>
		/// 将object转换成uint(System.UInt32)
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>转换后的值</returns>
		public static uint C2UInt(object value)
		{
			if (value == null) return 0;
			try
			{
				return Convert.ToUInt32(value, CultureInfo.InvariantCulture);
			}
			catch
			{
				return C2UInt(value.ToString());
			}
		}

		/// <summary>
		/// 将object转换成long(System.Int64)
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>转换后的值</returns>
		public static long C2Lng(object value)
		{
			if (value == null) return 0;
			try
			{
				return Convert.ToInt64(value, CultureInfo.InvariantCulture);
			}
			catch
			{
				return C2Lng(value.ToString());
			}
		}

		/// <summary>
		/// 将object转换成long(System.UInt64)
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>转换后的值</returns>
		public static ulong C2ULng(object value)
		{
			if (value == null) return 0;
			try
			{
				return Convert.ToUInt64(value, CultureInfo.InvariantCulture);
			}
			catch
			{
				return C2ULng(value.ToString());
			}
		}

		/// <summary>
		/// 将object转换成double(System.Double)
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>转换后的值</returns>
		public static double C2Dbl(object value)
		{
			if (value == null) return 0;
			try
			{
				return Convert.ToDouble(value, CultureInfo.InvariantCulture);
			}
			catch
			{
				return C2Dbl(value.ToString());
			}
		}

		/// <summary>
		/// 将object转换成float(System.Single)
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>转换后的值</returns>
		public static float C2Flt(object value)
		{
			if (value == null) return 0;
			try
			{
				return System.Convert.ToSingle(value, CultureInfo.InvariantCulture);
			}
			catch
			{
				return C2Flt(value.ToString());
			}
		}

		/// <summary>
		/// 将object转换成decimal(System.Decimal)
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>转换后的值</returns>
		public static decimal C2Dec(object value)
		{
			if (value == null) return 0;
			try
			{
				return System.Convert.ToDecimal(value, CultureInfo.InvariantCulture);
			}
			catch
			{
				return C2Dec(value.ToString());
			}
		}

		/// <summary>
		/// 将object转换成DateTime
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>转换后的值</returns>
		public static DateTime C2Date(object value)
		{
			if (value == null) return default;
			try
			{
				return System.Convert.ToDateTime(value, CultureInfo.InvariantCulture);
			}
			catch
			{
				return C2Date(value.ToString());
			}
		}

		/// <summary>
		/// 将object转换成string(System.String)
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>转换后的值</returns>
		public static string C2Str(object value)
		{
			if (value == null) return "";
			try
			{
				return System.Convert.ToString(value, CultureInfo.InvariantCulture);
			}
			catch
			{
				return "";
			}
		}

		/// <summary>
		/// 将object转换成char(System.Char)
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>转换后的值</returns>
		public static char C2Chr(object value)
		{
			if (value == null) return default;
			try
			{
				return System.Convert.ToChar(value, CultureInfo.InvariantCulture);
			}
			catch
			{
				return C2Chr(value.ToString());
			}
		}

		/// <summary>
		/// 将object转换成数据库非空字符串
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>转换后的值</returns>
		public static string C2Dbs(object value)
		{
			if (value == null) return " ";
			try
			{
				string val = System.Convert.ToString(value, CultureInfo.InvariantCulture);
				return string.IsNullOrEmpty(val) ? " " : val;
			}
			catch
			{
				return " ";
			}
		}

		#endregion

		#region string转换成系统的基本类型(bool, sbyte, byte, short, int, long, float, double, decimal, datetime)

		/// <summary>
		/// 将string转换成bool(System.Boolean)
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>转换后的值</returns>
		public static bool C2Bool(string value)
		{
			if (bool.TryParse(value, out var result))
			{
				return result;
			}
			else
			{
				if (double.TryParse(value, out var dblVal))
				{
					return dblVal > 0 ? true : false;
				}
			}
			return false;
		}

		/// <summary>
		/// 将string转换成sbyte(System.SByte)
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>转换后的值</returns>
		public static sbyte C2SByte(string value)
		{
			if (sbyte.TryParse(value, out var result))
			{
				return result;
			}
			return 0;
		}

		/// <summary>
		/// 将string转换成byte(System.Byte)
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>转换后的值</returns>
		public static byte C2Byte(string value)
		{
			if (byte.TryParse(value, out var result))
			{
				return result;
			}
			return 0;
		}

		/// <summary>
		/// 将string转换成short(System.Int16)
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>转换后的值</returns>
		public static short C2Short(string value)
		{
			if (short.TryParse(value, out var result))
			{
				return result;
			}
			else
			{
				if (double.TryParse(value, out var dblVal))
				{
					return (short)dblVal;
				}
			}
			return 0;
		}

		/// <summary>
		/// 将string转换成ushort(System.UInt16)
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>转换后的值</returns>
		public static ushort C2UShort(string value)
		{
			if (ushort.TryParse(value, out var result))
			{
				return result;
			}
			else
			{
				if (double.TryParse(value, out var dblVal))
				{
					return (ushort)dblVal;
				}
			}
			return 0;
		}

		/// <summary>
		/// 将string转换成int(System.Int32)
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>转换后的值</returns>
		public static int C2Int(string value)
		{
			if (int.TryParse(value, out var result))
			{
				return result;
			}
			else
			{
				// 如果字符串中包含"."或","等分隔符号时可尝试转换成double，然后强制转换成int
				if (double.TryParse(value, out var dblVal))
				{
					return (int)dblVal;
				}
			}
			return 0;
		}

		/// <summary>
		/// 将string转换成uint(System.UInt32)
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>转换后的值</returns>
		public static uint C2UInt(string value)
		{
			if (uint.TryParse(value, out var result))
			{
				return result;
			}
			else
			{
				if (double.TryParse(value, out var dblVal))
				{
					return (uint)dblVal;
				}
			}
			return 0;
		}

		/// <summary>
		/// 将string转换成long(System.Int64)
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>转换后的值</returns>
		public static long C2Lng(string value)
		{
			if (long.TryParse(value, out var result))
			{
				return result;
			}
			else
			{
				if (double.TryParse(value, out var dblVal))
				{
					return (long)dblVal;
				}
			}
			return 0;
		}

		/// <summary>
		/// 将string转换成ulong(System.UInt64)
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>转换后的值</returns>
		public static ulong C2ULng(string value)
		{
			if (ulong.TryParse(value, out var result))
			{
				return result;
			}
			else
			{
				if (double.TryParse(value, out var dblVal))
				{
					return (ulong)dblVal;
				}
			}
			return 0;
		}

		/// <summary>
		/// 将string转换成double(System.Double)
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>转换后的值</returns>
		public static double C2Dbl(string value)
		{
			if (double.TryParse(value, out var result))
			{
				return result;
			}
			return 0;
		}

		/// <summary>
		/// 将string转换成float(System.Single)
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>转换后的值</returns>
		public static float C2Flt(string value)
		{
			if (float.TryParse(value, out var result))
			{
				return result;
			}
			return 0;
		}

		/// <summary>
		/// 将string转换成decimal(System.Decimal)
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>转换后的值</returns>
		public static decimal C2Dec(string value)
		{
			if (decimal.TryParse(value, out var result))
			{
				return result;
			}
			return 0;
		}

		/// <summary>
		/// 将string转换成DateTime
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>转换后的值</returns>
		public static DateTime C2Date(string value)
		{
			if (DateTime.TryParse(value, out var result))
			{
				return result;
			}
			return default(DateTime);
		}

		/// <summary>
		/// 将string转换成char(System.Char)
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>转换后的值</returns>
		public static char C2Chr(string value)
		{
			if (char.TryParse(value, out var result))
			{
				return result;
			}
			return '\0';
		}

		/// <summary>
		/// 将string转换成数据库非空字符串
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>转换后的值</returns>
		public static string C2Dbs(string value)
		{
			return (string.IsNullOrEmpty(value)) ? " " : value;
		}

		#endregion

		#region 任意类型(bool, short, short, int, long, float, double, decimal, datetime, string, object)转换成字节数组

		/// <summary>
		/// 将bool(System.Boolean)数值转换成字节数组
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static byte[] C2Bytes(bool value)
		{
			return FastToBytes(value.ToString(CultureInfo.InvariantCulture).ToUpperInvariant());
		}

		/// <summary>
		/// 将short(System.Int16)数值转换成字节
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static byte[] C2Bytes(short value)
		{
			return FastToBytes(value.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// 将ushort(System.UInt16)数值转换成字节
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static byte[] C2Bytes(ushort value)
		{
			return FastToBytes(value.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// 将int(System.Int32)数值转换成字节
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static byte[] C2Bytes(int value)
		{
			return FastToBytes(value.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// 将uint(System.UInt32)数值转换成字节
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static byte[] C2Bytes(uint value)
		{
			return FastToBytes(value.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// 将long(System.Int64)数值转换成字节
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static byte[] C2Bytes(long value)
		{
			return FastToBytes(value.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// 将ulong(System.UInt64)数值转换成字节
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static byte[] C2Bytes(ulong value)
		{
			return FastToBytes(value.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// 将double(System.Double)数值转换成字节
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static byte[] C2Bytes(double value)
		{
			var dblStr = value.ToString(CultureInfo.InvariantCulture.NumberFormat);

			if (dblStr.IndexOf('E') != -1 || dblStr.IndexOf('e') != -1)
				dblStr = DoubleConverter.ToExactString(value);

			return FastToBytes(dblStr);
		}

		/// <summary>
		/// 将float(System.Single)数值转换成字节
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static byte[] C2Bytes(float value)
		{
			return FastToBytes(value.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// 将decimal(System.Decimal)数值转换成字节
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static byte[] C2Bytes(decimal value)
		{
			return FastToBytes(value.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// 将DateTime值转换成字节
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static byte[] C2Bytes(DateTime value)
		{
			return FastToBytes(value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// 将字符串快速转换成字节
		/// 跳过'安全字符串'的编码过程
		/// Skip the encoding process for 'safe strings' 
		/// </summary>
		/// <param name="strVal"></param>
		/// <returns></returns>
		private static byte[] FastToBytes(string strVal)
		{
			var bytes = new byte[strVal.Length];
			for (var i = 0; i < strVal.Length; i++)
				bytes[i] = (byte)strVal[i];

			return bytes;
		}

		/// <summary>
		/// 将string(System.String)转换成字节。
		/// 说明：string转换成字节时会进行UTF8编码。
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static byte[] C2Bytes(string value)
		{
			return Encoding.UTF8.GetBytes(value);
		}

		/// <summary>
		/// 将object值转换成字节。
		/// 说明：如果value的类型是object，在转换成字节时, 首先序列化成json字符串，再UTF8编码成字节数组。
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static byte[] C2Bytes(object value)
		{
			if (value == null)
				return Array.Empty<byte>();

			if (value is byte[] buf)
				return buf;

			TypeCode typeCode = Type.GetTypeCode(value.GetType());
			switch(typeCode)
			{
				case TypeCode.Object: return C2Bytes(Json.Json.Serialize(value));
				case TypeCode.String: return C2Bytes(C2Str(value));
				case TypeCode.DateTime: return C2Bytes(C2Date(value));
				case TypeCode.Decimal: return C2Bytes(C2Dec(value));
				case TypeCode.Double: return C2Bytes(C2Dbl(value));
				case TypeCode.Single: return C2Bytes(C2Flt(value));
				case TypeCode.Int64: return C2Bytes(C2Lng(value));
				case TypeCode.Int32: return C2Bytes(C2Int(value));
				case TypeCode.Int16: return C2Bytes(C2Short(value));
				case TypeCode.UInt64: return C2Bytes(C2Lng(value));
				case TypeCode.UInt32: return C2Bytes(C2Int(value));
				case TypeCode.UInt16: return C2Bytes(C2Short(value));
				case TypeCode.Byte: return C2Bytes(C2Byte(value));
				case TypeCode.SByte: return C2Bytes(C2SByte(value));
				case TypeCode.Char: return C2Bytes(C2Chr(value));
				case TypeCode.Boolean: return C2Bytes(C2Bool(value));
				default: return C2Bytes(Json.Json.Serialize(value));
			}
		}

		#endregion

		#region 字节数组转换回任意类型(bool, short, short, int, long, float, double, decimal, datetime, string, object)

		/// <summary>
		/// 将字节数组转换成bool(System.Boolean)数值
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool C2Bool(byte[] value)
		{
			return C2Bool(Encoding.ASCII.GetString(value));
		}

		/// <summary>
		/// 将字节数组转换成sbyte(System.SByte)有符号字节
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>转换后的值</returns>
		public static sbyte C2SByte(byte[] value)
		{
			return C2SByte(Encoding.ASCII.GetString(value));
		}

		/// <summary>
		/// 将字节数组转换成sbyte(System.Byte)无符号字节
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>转换后的值</returns>
		public static byte C2Byte(byte[] value)
		{
			return C2Byte(Encoding.ASCII.GetString(value));
		}

		/// <summary>
		/// 将字节数组转换成short(System.Int16)数值
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static short C2Short(byte[] value)
		{
			return C2Short(Encoding.ASCII.GetString(value));
		}

		/// <summary>
		/// 将字节数组转换成ushort(System.UInt16)数值
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static ushort C2Bytes(byte[] value)
		{
			return C2UShort(Encoding.ASCII.GetString(value));
		}

		/// <summary>
		/// 将字节数组转换成int(System.Int32)数值
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static int C2Int(byte[] value)
		{
			return C2Int(Encoding.ASCII.GetString(value));
		}

		/// <summary>
		/// 将字节数组转换成uint(System.UInt32)数值
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static uint C2UInt(byte[] value)
		{
			return C2UInt(Encoding.ASCII.GetString(value));
		}

		/// <summary>
		/// 将字节数组转换成long(System.Int64)数值
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static long C2Lng(byte[] value)
		{
			return C2Lng(Encoding.ASCII.GetString(value));
		}

		/// <summary>
		/// 将字节数组转换成ulong(System.UInt64)数值
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static ulong C2ULng(byte[] value)
		{
			return C2ULng(Encoding.ASCII.GetString(value));
		}

		/// <summary>
		/// 将字节数组转换成double(System.Double)数值
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static double C2Dbl(byte[] value)
		{
			return C2Dbl(Encoding.ASCII.GetString(value));
		}

		/// <summary>
		/// 将字节数组转换成float(System.Single)数值
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static float C2Flt(byte[] value)
		{
			return C2Flt(Encoding.ASCII.GetString(value));
		}

		/// <summary>
		/// 将字节数组转换成decimal(System.Decimal)数值
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static decimal C2Dec(byte[] value)
		{
			return C2Dec(Encoding.ASCII.GetString(value));
		}

		/// <summary>
		/// 将字节数组转换成DateTime值
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static DateTime C2Date(byte[] value)
		{
			return C2Date(Encoding.ASCII.GetString(value));
		}

		/// <summary>
		/// 将字节数组转换成字符串string(System.String)
		/// 说明：value必须是已经UTF8编码后的字节数组。
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>转换后的值</returns>
		public static string C2Str(byte[] value)
		{
			return Encoding.UTF8.GetString(value);
		}

		/// <summary>
		/// 将字节数组转换成char(System.Char)字符
		/// </summary>
		/// <param name="value">要转换的值</param>
		/// <returns>转换后的值</returns>
		public static char C2Chr(byte[] value)
		{
			return C2Chr(Encoding.ASCII.GetString(value));
		}

		/// <summary>
		/// 将byte[]值转成T。
		/// 说明：value必须是已经序列化成json字符串再UTF8编码后的字节数组。
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static T C2Obj<T>(byte[] value)
		{
			var json = Encoding.UTF8.GetString(value);
			try
			{
				if (json.Contains("[") || json.Contains("{")) // 简单校验是否是Json字符串
				{
					return Json.Json.Deserialize<T>(json);
				}
				else
				{
					return (T)(json as object); // 无效的Json则原样返回
				}
			}
			catch 
			{
				return (T)(json as object);
			}
		}

		/// <summary>
		/// 将string值转成T。
		/// 说明：value必须是已经序列化成json的字符串。
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static T C2Obj<T>(string value)
		{
			return Json.Json.Deserialize<T>(value);
		}

		#endregion

	}
}
