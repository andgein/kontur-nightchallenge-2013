using System;
using System.IO;
using System.Linq;
using Core.Parser;
using Newtonsoft.Json;

namespace Core.Arena
{
	public class PlayersRepo {

		private readonly DirectoryInfo playersDir;

		public PlayersRepo(DirectoryInfo playersDir)
		{
			this.playersDir = playersDir;
			if (!playersDir.Exists)
				playersDir.Create();
		}

		public void CreateOrUpdate(ArenaPlayer request)
		{
			lock (playersDir)
			{
				var versions = DeserializePlayerVersions(request.Name);
				versions = UpdatePlayer(versions, request);
				SerializePlayerVersions(request.Name, versions);
			}
		}

		private ArenaPlayer[] UpdatePlayer(ArenaPlayer[] versions, ArenaPlayer request)
		{
			request.Timestamp = DateTime.Now;
			if (versions.All(p => p.Password != request.Password && p.Name != request.Name))
				throw new Exception("Неверный пароль");
			if (string.IsNullOrEmpty(request.Program))
				throw new Exception("Бот пуст?!? O_o");
			new WarriorParser().Parse(request.Program);
			versions = versions.Concat(new[] {request}).ToArray();
			if (versions.All(v => string.IsNullOrWhiteSpace(v.Authors)))
				throw new Exception("Не заполнен список авторов");
			return versions;
		}

		private void SerializePlayerVersions(string playerName, ArenaPlayer[] playerVersions)
		{
			var file = GetFile(playerName);
			File.WriteAllText(file.FullName, JsonConvert.SerializeObject(playerVersions, Formatting.Indented));
		}

		private ArenaPlayer[] DeserializePlayerVersions(string playerName)
		{
			var file = GetFile(playerName);
			if (file.Exists)
				return JsonConvert.DeserializeObject<ArenaPlayer[]>(File.ReadAllText(file.FullName));
			return new ArenaPlayer[0];
		}

		private FileInfo GetFile(string playerName)
		{
			return new FileInfo(Path.Combine(playersDir.FullName, playerName + ".json"));
		}

		public ArenaPlayer LoadPlayer(string name, int? version)
		{
			lock (playersDir)
			{
				var versions = DeserializePlayerVersions(name);
				var index = (version ?? versions.Length) - 1;
				var result = versions[index];
				result.Authors = versions.Last(v => !string.IsNullOrWhiteSpace(v.Authors)).Authors;
				result.Version = index + 1;
				return result;
			}
		}
	}
}