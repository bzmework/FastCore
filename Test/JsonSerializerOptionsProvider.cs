using System.Text.Json;

namespace Test
{
	/// <summary>
	/// Json序列号选项
	/// </summary>
	internal static class JsonSerializerOptionsProvider
	{
		/// <summary>
		/// 选项
		/// </summary>
		public static readonly JsonSerializerOptions Options;

		/// <summary>
		/// 实例化
		/// </summary>
		static JsonSerializerOptionsProvider()
		{
			JsonSerializerOptions val = new JsonSerializerOptions();
			val.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
			val.PropertyNameCaseInsensitive = true;
			Options = (JsonSerializerOptions)(object)val;
		}
	}
}
