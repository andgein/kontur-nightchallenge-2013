using System;
using System.Collections;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Server.Sessions
{
	public class Session : ISession, ISessionItems
	{
		private readonly string sessionStorageFolder;
		private readonly Hashtable items = new Hashtable();
		private int saveCounter;
		private readonly object locker = new object();

		public Session(Guid sessionId, [NotNull] string sessionStorageFolder)
		{
			SessionId = sessionId;
			this.sessionStorageFolder = Path.Combine(sessionStorageFolder, sessionId.ToString());
		}

		public Guid SessionId { get; private set; }

		[NotNull]
		public ISessionItems Items
		{
			get { return this; }
		}

		public void Save<T>([NotNull] string key, [CanBeNull] T value)
		{
			var newSaveCounter = Interlocked.Increment(ref saveCounter);
			Task.Factory.StartNew(() =>
			{
				if (Interlocked.CompareExchange(ref saveCounter, newSaveCounter, newSaveCounter) == newSaveCounter)
				{
					lock (locker)
					{
						if (!Directory.Exists(sessionStorageFolder))
							Directory.CreateDirectory(sessionStorageFolder);
						var valueString = JsonConvert.SerializeObject(value, new JsonSerializerSettings {Formatting = Formatting.None, ContractResolver = new CamelCasePropertyNamesContractResolver()});
						File.WriteAllText(Path.Combine(sessionStorageFolder, key), valueString);
					}
				}
			});
		}

		[CanBeNull]
		public T Load<T>([NotNull] string key)
		{
			var filename = Path.Combine(sessionStorageFolder, key);
			if (!File.Exists(filename))
				return default(T);
			var valueString = File.ReadAllText(filename);
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