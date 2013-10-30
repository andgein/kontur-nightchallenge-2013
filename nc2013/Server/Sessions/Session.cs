using System;
using System.Collections;
using System.IO;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Server.Sessions
{
	public class Session : ISession, ISessionItems
	{
		private readonly string sessionStorageFolder;
		private readonly Hashtable items = new Hashtable();

		public Session(Guid sessionId, [NotNull] string sessionStorageFolder)
		{
			SessionId = sessionId;
			this.sessionStorageFolder = sessionStorageFolder;
		}

		public Guid SessionId { get; private set; }

		[NotNull]
		public ISessionItems Items
		{
			get { return this; }
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

		[CanBeNull]
		object ISessionItems.this[object key]
		{
			get { return items[key]; }
			set { items[key] = value; }
		}
	}
}