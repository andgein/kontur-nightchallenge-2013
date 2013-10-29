using System;
using System.IO;
using Core.Game;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Server.Debugging;

namespace Server.Sessions
{
	public class Session : ISession
	{
		private readonly string sessionStorageFolder;
		private readonly Lazy<IDebugger> lazyDebugger;

		public Session(Guid sessionId, [NotNull] string sessionStorageFolder, [NotNull] IGameServer gameServer)
		{
			SessionId = sessionId;
			this.sessionStorageFolder = sessionStorageFolder;
			lazyDebugger = new Lazy<IDebugger>(() => new Debugger(gameServer, this));
		}

		public Guid SessionId { get; private set; }

		[NotNull]
		public IDebugger Debugger
		{
			get { return lazyDebugger.Value; }
		}

		public void Save<T>([NotNull] string key, [CanBeNull] T value)
		{
			return;
			if (!Directory.Exists(sessionStorageFolder))
				Directory.CreateDirectory(sessionStorageFolder);
			var valueString = JsonConvert.SerializeObject(value, new JsonSerializerSettings {Formatting = Formatting.Indented, ContractResolver = new CamelCasePropertyNamesContractResolver()});
			File.WriteAllText(Path.Combine(sessionStorageFolder, key), valueString);
		}

		[CanBeNull]
		public T Load<T>([NotNull] string key)
		{
			return default(T);
			if (!File.Exists(Path.Combine(sessionStorageFolder, key)))
				return default(T);
			var valueString = File.ReadAllText(key);
			var result = JsonConvert.DeserializeObject<T>(valueString, new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});
			return result;
		}
	}
}