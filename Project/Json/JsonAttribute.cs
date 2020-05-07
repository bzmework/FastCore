using System;
using System.Collections.Generic;
using System.Text;

namespace FastCore.Json
{
	/// <summary>
	/// 属性重命名字段/属性名，以便用于序列化和反序列化
	/// Attribute for renaming field/property name to use for serialization and deserialization
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Enum)]
	public sealed class JsonPropertyAttribute : Attribute
	{
		/// <summary>
		/// Name of property/field
		/// 属性/字段的名称
		/// </summary>
		public string Name { get; private set; }
		/// <summary>
		/// 默认构造函数
		/// Default constructor
		/// </summary>
		/// <param name="name"></param>
		public JsonPropertyAttribute(string name)
		{
			Name = name;
		}
	}

	/// <summary>
	/// Attribute for configuration of Class that requires type information for serialization and deserialization
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = true)]
	public sealed class JsonKnownTypeAttribute : Attribute
	{
		/// <summary>
		/// Type
		/// </summary>
		public Type Type { private set; get; }
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="type"></param>
		public JsonKnownTypeAttribute(Type type)
		{
			Type = type;
		}
	}

}
