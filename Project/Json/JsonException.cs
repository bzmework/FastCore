using System;
using System.Collections.Generic;
using System.Text;

namespace FastCore.Json
{
	/// <summary>
	/// Exception thrown for invalid json string
	/// </summary>
	public sealed class JsonInvalidException : Exception
	{
		public JsonInvalidException()
			: base("Input is not a valid JSON.")
		{
		}
	}

	/// <summary>
	/// Exception thrown for invalid json string
	/// </summary>
	public sealed class JsonTypeMismatchException : Exception
	{
		public JsonTypeMismatchException()
			: base("Unexpected type was encountered in JSON")
		{
		}
	}

	/// <summary>
	/// Exception thrown for invalid json property attribute
	/// </summary>
	public sealed class JsonInvalidPropertyException : Exception
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public JsonInvalidPropertyException()
			: base("Class cannot contain any NetJSONProperty with null or blank space character")
		{
		}
	}

	/// <summary>
	/// Exception thrown for invalid assembly generation when adding all assembly into a specified assembly file
	/// </summary>
	public sealed class JsonInvalidAssemblyGeneration : Exception
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="asmName"></param>
		public JsonInvalidAssemblyGeneration(string asmName) : base(String.Format("Could not generate assembly with name [{0}] due to empty list of types to include", asmName)) { }
	}

}
