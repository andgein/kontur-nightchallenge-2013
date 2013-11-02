using System;
using System.Collections;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Core;
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
			this.sessionStorageFolder = Path.Combine(sessionStorageFolder, sessionId.ToString());
		}

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
						var valueString = JsonConvert.SerializeObject(value, new JsonSerializerSettings {Formatting = Formatting.Indented, ContractResolver = new CamelCasePropertyNamesContractResolver()});
						File.WriteAllText(GetKeyFilename(key), valueString);
					}
				}
			});
		}

		[CanBeNull]
		public T Load<T>([NotNull] string key)
		{
			var filename = GetKeyFilename(key);
			if (!File.Exists(filename))
				return default(T);
			var valueString = File.ReadAllText(filename);
			try
			{
				var result = JsonConvert.DeserializeObject<T>(valueString, new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});
				return result;
			}
			catch (Exception e)
			{
				Log.For(this).Error(string.Format("Failed to deserialize session key {0} from file {1}", key, filename), e);
				return default(T);
			}
		}

		[NotNull]
		private string GetKeyFilename([NotNull] string key)
		{
			return Path.Combine(sessionStorageFolder, key) + ".json";
		}

		[CanBeNull]
		object ISessionItems.this[object key]
		{
			get { return items[key]; }
			set { items[key] = value; }
		}
	}
}