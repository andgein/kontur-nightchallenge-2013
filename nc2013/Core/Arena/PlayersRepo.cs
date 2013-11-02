using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Core.Game;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Core.Arena
{
	public class PlayersRepo : IPlayersRepo
	{
		private const string nameValidationRegex = @"^[0-9A-Za-z_-]+$";
		private readonly DirectoryInfo playersDir;
		private readonly IWarriorProgramParser warriorProgramParser;

		public PlayersRepo([NotNull] DirectoryInfo playersDir, [NotNull] IWarriorProgramParser warriorProgramParser)
		{
			this.playersDir = playersDir;
			this.warriorProgramParser = warriorProgramParser;
			if (!playersDir.Exists)
				playersDir.Create();
		}

		public bool CreateOrUpdate([NotNull] ArenaPlayer request)
		{
			lock (playersDir)
			{
				var existingVersions = DeserializePlayerVersions(request.Name);
				var versionsToPersist = TryUpdatePlayer(existingVersions, request);
				if (versionsToPersist != null)
					SerializePlayerVersions(request.Name, versionsToPersist);
				return versionsToPersist != null;
			}
		}

		[NotNull]
		public ArenaPlayer[] LoadPlayerVersions([NotNull] string name)
		{
			lock (playersDir)
				return DeserializePlayerVersions(name);
		}

		[NotNull]
		public ArenaPlayer[] LoadLastVersions()
		{
			lock (playersDir)
			{
				return playersDir
					.GetFiles("*.json")
					.Select(file => DeserializePlayerVersions(Path.GetFileNameWithoutExtension(file.Name)).GetLastVersion())
					.ToArray();
			}
		}

		[CanBeNull]
		private ArenaPlayer[] TryUpdatePlayer([NotNull] ArenaPlayer[] existingVersions, [NotNull] ArenaPlayer request)
		{
			request.Timestamp = DateTime.UtcNow;
			request.Authors = (request.Authors ?? string.Empty).Trim();
			request.Program = (request.Program ?? string.Empty).Trim();
			if (!Regex.IsMatch(request.Name, nameValidationRegex))
				throw new BadBotException(string.Format("Имя должно подходить под шаблон: {0}", nameValidationRegex));
			if (string.IsNullOrEmpty(request.Password))
				throw new BadBotException("Пароль не может быть пустым");
			if (existingVersions.Any(p => p.Password != request.Password || p.Name != request.Name))
				throw new BadBotException("Неверный пароль");
			if (string.IsNullOrEmpty(request.Program))
				throw new BadBotException("Бот пуст?!? O_o");
			var parserErrors = warriorProgramParser.ValidateProgram(request.Program);
			if (!string.IsNullOrEmpty(parserErrors))
				throw new BadBotException(string.Format("В программе есть ошибки:\r\n{0}", parserErrors));
			var newVersions = new[] { request };
			ArenaPlayer[] versionsToPersist;
			if (existingVersions.Length == 0)
				versionsToPersist = newVersions;
			else
			{
				var last = existingVersions.GetLastVersion();
				if (last.Program != request.Program || !string.IsNullOrEmpty(request.Authors) && request.Authors != last.Authors)
					versionsToPersist = existingVersions.Concat(newVersions).ToArray();
				else
					throw new BadBotException("Бот идентичен последней версии");
			}
			if (versionsToPersist.All(v => string.IsNullOrEmpty(v.Authors)))
				throw new BadBotException("Не заполнен список авторов");
			return versionsToPersist;
		}

		private void SerializePlayerVersions([NotNull] string playerName, [NotNull] ArenaPlayer[] playerVersions)
		{
			var file = GetFile(playerName);
			File.WriteAllText(file.FullName, JsonConvert.SerializeObject(playerVersions, Formatting.Indented));
		}

		[NotNull]
		private ArenaPlayer[] DeserializePlayerVersions([NotNull] string playerName)
		{
			var file = GetFile(playerName);
			if (file.Exists)
			{
				var playerVersions = JsonConvert.DeserializeObject<ArenaPlayer[]>(File.ReadAllText(file.FullName));
				for (var i = 0; i < playerVersions.Length; i++)
					playerVersions[i].Version = i + 1;
				return playerVersions;
			}
			return new ArenaPlayer[0];
		}

		[NotNull]
		private FileInfo GetFile([NotNull] string playerName)
		{
			return new FileInfo(Path.Combine(playersDir.FullName, playerName + ".json"));
		}
	}
}