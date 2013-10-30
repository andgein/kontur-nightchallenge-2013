using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Core.Parser;
using Newtonsoft.Json;

namespace Core.Arena
{
	public class PlayersRepo
	{
		private const string nameValidationRegex = @"^[0-9A-Za-z_-]+$";
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
				ArenaPlayer[] versions = DeserializePlayerVersions(request.Name);
				versions = UpdatePlayer(versions, request);
				SerializePlayerVersions(request.Name, versions);
			}
		}

		public ArenaPlayer LoadPlayer(string name, int? version)
		{
			lock (playersDir)
			{
				ArenaPlayer[] versions = DeserializePlayerVersions(name);
				return GetVersion(versions, version);
			}
		}

		public ArenaPlayer[] LoadLastVersions()
		{
			lock (playersDir)
			{
				return playersDir
					.GetFiles("*.json")
					.Select(file => GetVersion(DeserializePlayerVersions(Path.GetFileNameWithoutExtension(file.Name))))
					.ToArray();
			}
		}

		private ArenaPlayer[] UpdatePlayer(ArenaPlayer[] versions, ArenaPlayer request)
		{
			request.Timestamp = DateTime.Now;
			if (!Regex.IsMatch(request.Name, nameValidationRegex))
				throw new Exception(string.Format("Имя должно быть {0}! ;-)", nameValidationRegex));
			if (string.IsNullOrEmpty(request.Password))
				throw new Exception("Пароль не может быть пустым");
			if (versions.Any(p => p.Password != request.Password || p.Name != request.Name))
				throw new Exception("Неверный пароль");
			if (string.IsNullOrEmpty(request.Program))
				throw new Exception("Бот пуст?!? O_o");
//			new WarriorParser().Parse(request.Program);
			if (versions.Length == 0)
				versions = new[] {request};
			else
			{
				ArenaPlayer last = GetVersion(versions);
				if (last.Program != request.Program || !string.IsNullOrWhiteSpace(request.Authors) && request.Authors != last.Authors)
					versions = versions.Concat(new[] {request}).ToArray();
			}
			if (versions.All(v => string.IsNullOrWhiteSpace(v.Authors)))
				throw new Exception("Не заполнен список авторов");
			return versions;
		}

		private void SerializePlayerVersions(string playerName, ArenaPlayer[] playerVersions)
		{
			FileInfo file = GetFile(playerName);
			File.WriteAllText(file.FullName, JsonConvert.SerializeObject(playerVersions, Formatting.Indented));
		}

		private ArenaPlayer[] DeserializePlayerVersions(string playerName)
		{
			FileInfo file = GetFile(playerName);
			if (file.Exists)
				return JsonConvert.DeserializeObject<ArenaPlayer[]>(File.ReadAllText(file.FullName));
			return new ArenaPlayer[0];
		}

		private FileInfo GetFile(string playerName)
		{
			return new FileInfo(Path.Combine(playersDir.FullName, playerName + ".json"));
		}

		private static ArenaPlayer GetVersion(ArenaPlayer[] versions, int? version = null)
		{
			int index = (version ?? versions.Length) - 1;
			ArenaPlayer result = versions[index];
			return
				new ArenaPlayer
				{
					Authors = versions.Last(v => !string.IsNullOrWhiteSpace(v.Authors)).Authors,
					Name = result.Name,
					Program = result.Program,
					Timestamp = result.Timestamp,
					Version = index + 1
				};
		}
	}
}