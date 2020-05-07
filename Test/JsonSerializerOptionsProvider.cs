using System.Text.Json;

namespace Test
{
	/// <summary>
	/// Json���к�ѡ��
	/// </summary>
	internal static class JsonSerializerOptionsProvider
	{
		/// <summary>
		/// ѡ��
		/// </summary>
		public static readonly JsonSerializerOptions Options;

		/// <summary>
		/// ʵ����
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
