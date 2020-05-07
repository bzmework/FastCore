using System;
using System.Collections.Generic;
using System.Text;

namespace FastCore.Json
{
	/// <summary>
	/// Json设置
	/// </summary>
	public sealed class JsonSettings
	{
		private bool _hasDateStringFormat = false;
		private string _dateStringFormat;
		private bool _caseSensitive;
		private StringComparison _caseComparison = StringComparison.Ordinal;
		private JsonQuote _quoteType;
		private char _quoteChar;
		private string _quoteCharString;

		[ThreadStatic]
		private static JsonSettings _currentSettings;

		/// <summary>
		/// 日期格式，默认：Default
		/// </summary>
		public JsonDateFormat DateFormat { get; set; }

		/// <summary>
		/// 提供日期格式化时使用的字符串格式
		/// </summary>
		public string DateStringFormat
		{
			get
			{
				return _dateStringFormat;
			}
			set
			{
				_dateStringFormat = value;
				_hasDateStringFormat = !string.IsNullOrEmpty(value);
			}
		}

		/// <summary>
		/// 是否有日期字符串格式
		/// </summary>
		public bool HasDateStringFormat
		{
			get
			{
				return _hasDateStringFormat;
			}
		}

		/// <summary>
		/// 时区格式，默认：Unspecified
		/// </summary>
		public JsonTimeZoneFormat TimeZoneFormat { get; set; }

		/// <summary>
		/// 输出json的格式，默认：Default
		/// </summary>
		public JsonFormat Format { get; set; }

		/// <summary>
		/// Enum应该序列化为字符串(string)还是整型值(int)。默认值: True
		/// </summary>
		public bool UseEnumString { get; set; }

		/// <summary>
		/// 是否应该跳过默认值，默认: True
		/// </summary>
		public bool SkipDefaultValue { get; set; }

		/// <summary>
		/// 属性/字段名是否区分大小写，默认: True
		/// </summary>
		public bool CaseSensitive
		{
			get
			{
				return _caseSensitive;
			}
			set
			{
				_caseSensitive = value;
				_caseComparison = _caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
			}
		}

		/// <summary>
		/// 引用类型(Quote Type), 默认: 双引号
		/// </summary>
		public JsonQuote QuoteType
		{
			get
			{
				return _quoteType;
			}
			set
			{
				_quoteType = value;
				_quoteChar = _quoteType == JsonQuote.Single ? '\'' : '"';
				_quoteCharString = _quoteType == JsonQuote.Single ? "'" : "\"";
			}
		}

		/// <summary>
		/// 引用字符
		/// </summary>
		public char QuoteChar
		{
			get
			{
				return _quoteChar;
			}
			set
			{
				_quoteChar = value;
			}
		}

		/// <summary>
		/// 引用字符串
		/// </summary>
		public string QuoteCharString
		{
			get
			{
				return _quoteCharString;
			}
			set
			{
				_quoteCharString = value;
			}
		}

		/// <summary>
		/// 是否重写引用字符
		/// </summary>
		public bool HasOverrideQuoteChar { get; internal set; }

		/// <summary>
		/// 是否使用字符串优化
		/// </summary>
		public bool UseStringOptimization { get; set; }

		/// <summary>
		///  启用包含用于序列化和反序列化的类型信息 
		/// </summary>
		public bool IncludeTypeInformation { get; set; }

		private StringComparison CaseComparison
		{
			get
			{
				return CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
			}
		}

		/// <summary>
		///  为属性/字段名启用camelCasing 
		/// </summary>
		public bool CamelCase { get; set; }

		/// <summary>
		/// 默认构造函数
		/// </summary>
		public JsonSettings()
		{
			IncludeTypeInformation = Json.IncludeTypeInformation;
			DateFormat = JsonDateFormat.ISO;
			TimeZoneFormat = JsonTimeZoneFormat.Unspecified;
			UseEnumString = Json.UseEnumString;
			SkipDefaultValue = true;
			CaseSensitive = true;
			QuoteType = JsonQuote.Default;
			UseStringOptimization = true;
			Format = JsonFormat.Default;
			CamelCase = false;
		}

		/// <summary>
		/// Clone settings
		/// </summary>
		/// <returns></returns>
		public JsonSettings Clone()
		{
			return new JsonSettings
			{
				IncludeTypeInformation = IncludeTypeInformation,
				DateFormat = DateFormat,
				TimeZoneFormat = TimeZoneFormat,
				UseEnumString = UseEnumString,
				SkipDefaultValue = SkipDefaultValue,
				CaseSensitive = CaseSensitive,
				QuoteType = QuoteType,
				UseStringOptimization = UseStringOptimization,
				Format = Format
			};
		}

		/// <summary>
		/// Returns current JsonSettings that correspond to old use of settings
		/// </summary>
		public static JsonSettings CurrentSettings
		{
			get
			{
				return _currentSettings ?? (_currentSettings = new JsonSettings());
			}
		}
	}
}
