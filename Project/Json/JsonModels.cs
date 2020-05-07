using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FastCore.Json
{
	

	/// <summary>
	/// Json字符串阅读器
	/// </summary>
	public unsafe sealed class JsonStringReader
	{
		private char* ptr;
		private int index;
		internal int counter;

		/// <summary>
		/// 默认构造函数
		/// </summary>
		public JsonStringReader() 
		{ 
			//
		}

		internal JsonStringReader(char* ptr, int index)
		{
			this.ptr = ptr;
			this.index = index;
			this.counter++;
			this.index++;
		}

		/// <summary>
		/// 下一个字符
		/// </summary>
		/// <returns></returns>
		public char Next()
		{
			counter++;
			return ptr[index++];
		}
	}

	internal abstract class JsonSerializer<T>
	{

		public abstract string Serialize(T value);
		public abstract T Deserialize(string value);

		public abstract void Serialize(T value, TextWriter writer);
		public abstract T Deserialize(TextReader reader);

		//With Settings
		public abstract string Serialize(T value, JsonSettings settings);
		public abstract T Deserialize(string value, JsonSettings settings);
		public abstract void Serialize(T value, TextWriter writer, JsonSettings settings);
		public abstract T Deserialize(TextReader reader, JsonSettings settings);
	}

	/// <summary>
	/// Option for determining date formatting
	/// </summary>
	public enum JsonDateFormat
	{
		/// <summary>
		/// Default /Date(...)/
		/// </summary>
		Default = 0,
		/// <summary>
		/// ISO Format
		/// </summary>
		ISO = 2,
		/// <summary>
		/// Unix Epoch Milliseconds
		/// </summary>
		EpochTime = 4,
		/// <summary>
		/// JSON.NET Format for backward compatibility
		/// </summary>
		JsonNetISO = 6,
		/// <summary>
		/// .NET System.Web.Script.Serialization.JavaScriptSerializer backward compatibility
		/// </summary>
		JavascriptSerializer = 8
	}


	/// <summary>
	/// Option for determining timezone formatting
	/// </summary>
	public enum JsonTimeZoneFormat
	{
		/// <summary>
		/// Default unspecified
		/// </summary>
		Unspecified = 0,
		/// <summary>
		/// Utc
		/// </summary>
		Utc = 2,
		/// <summary>
		/// Local time
		/// </summary>
		Local = 4
	}

	/// <summary>
	/// Option for determine what type of quote to use for serialization and deserialization
	/// </summary>
	public enum JsonQuote
	{
		/// <summary>
		/// Default: double quote
		/// </summary>
		Default = 0,
		/// <summary>
		/// Use double quote
		/// </summary>
		Double = Default,
		/// <summary>
		/// Use single quote
		/// </summary>
		Single = 2
	}

	/// <summary>
	/// Options for controlling serialize json format
	/// </summary>
	public enum JsonFormat
	{
		/// <summary>
		/// Default
		/// </summary>
		Default = 0,
		/// <summary>
		/// Prettify string
		/// </summary>
		Prettify = 2
	}
}
