using System.Diagnostics;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Server
{
	public static class JsonSerializer
	{
		private static bool? isDebug;

		[CanBeNull]
		public static T Deserialize<T>([NotNull] string data)
		{
			return JsonConvert.DeserializeObject<T>(data, new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});
		}

		[NotNull]
		public static string Serialize<T>([CanBeNull] T data)
		{
			return JsonConvert.SerializeObject(data, new JsonSerializerSettings
			{
				Formatting = IsDebug ? Formatting.Indented : Formatting.None,
				DateFormatString = "yyyy-MM-dd HH:mm:ss",
				ContractResolver = new CamelCasePropertyNamesContractResolver()
			});
		}

		private static bool IsDebug
		{
			get { return isDebug ?? (isDebug = Debugger.IsAttached).Value; }
		}
	}
}