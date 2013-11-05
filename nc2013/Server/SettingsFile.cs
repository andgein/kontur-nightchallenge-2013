using System;
using System.IO;
using JetBrains.Annotations;
using log4net;

namespace Server
{
	public class SettingsFile
	{
		private const string defaultPrefix = "http://*/corewar/";
		private const int defaultBattlesPerPair = 5;

		private static readonly ILog log = LogManager.GetLogger(typeof (SettingsFile));

		public SettingsFile([NotNull] string fileName)
		{
			HttpListenerPrefix = defaultPrefix;
			BattlesPerPair = defaultBattlesPerPair;
			ProductionMode = false;
			GodAccessOnly = false;
			GodModeSecret = Guid.NewGuid().ToString();
			if (File.Exists(fileName))
			{
				var lines = File.ReadLines(fileName);
				foreach (var line in lines)
				{
					if (string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith("#"))
						continue;
					var strings = line.Split(new[] {'='}, 2);
					if (strings.Length != 2)
					{
						log.Warn("Invalid line in settings file: " + line);
						continue;
					}
					var settingName = strings[0].Trim();
					var settingValue = strings[1].Trim();
					if (string.Equals(settingName, "HttpListenerPrefix", StringComparison.OrdinalIgnoreCase))
						ParseStringSetting(settingName, settingValue, v => HttpListenerPrefix = v);
					else if (string.Equals(settingName, "GodModeSecret", StringComparison.OrdinalIgnoreCase))
						ParseStringSetting(settingName, settingValue, v => GodModeSecret = v);
					else if (string.Equals(settingName, "BattlesPerPair", StringComparison.OrdinalIgnoreCase))
						ParseIntSetting(settingName, settingValue, v => BattlesPerPair = v);
					else if (string.Equals(settingName, "ProductionMode", StringComparison.OrdinalIgnoreCase))
						ParseBoolSetting(settingName, settingValue, v => ProductionMode = v);
					else if (string.Equals(settingName, "GodAccessOnly", StringComparison.OrdinalIgnoreCase))
						ParseBoolSetting(settingName, settingValue, v => GodAccessOnly = v);
					else
						log.Warn(string.Format("Unknown setting '{0}' in settings file", settingName));
				}
			}
		}

		private static void ParseStringSetting([NotNull] string settingName, [NotNull] string settingValue, Action<string> setter)
		{
			if (string.IsNullOrWhiteSpace(settingValue))
				log.Warn(string.Format("Empty value of setting '{0}'", settingName));
			else
				setter(settingValue);
		}

		private static void ParseBoolSetting([NotNull] string settingName, [NotNull] string settingValue, Action<bool> setter)
		{
			if (string.IsNullOrWhiteSpace(settingValue))
				log.Warn(string.Format("Empty value of setting '{0}'", settingName));
			else
			{
				bool settingBoolValue;
				if (!bool.TryParse(settingValue, out settingBoolValue))
					log.Warn(string.Format("Invalid boolean value for setting '{0}' in settings file: {1}", settingName, settingValue));
				else
					setter(settingBoolValue);
			}
		}

		private static void ParseIntSetting([NotNull] string settingName, [NotNull] string settingValue, Action<int> setter)
		{
			if (string.IsNullOrWhiteSpace(settingValue))
				log.Warn(string.Format("Empty value of setting '{0}'", settingName));
			else
			{
				int settingIntValue;
				if (!int.TryParse(settingValue, out settingIntValue))
					log.Warn(string.Format("Invalid integer value for setting '{0}' in settings file: {1}", settingName, settingValue));
				else
					setter(settingIntValue);
			}
		}

		[NotNull]
		public string HttpListenerPrefix { get; private set; }

		public int BattlesPerPair { get; private set; }
		public bool ProductionMode { get; private set; }
		public bool GodAccessOnly { get; private set; }

		[NotNull]
		public string GodModeSecret { get; private set; }

		[NotNull]
		public override string ToString()
		{
			return string.Format("HttpListenerPrefix: {0}, BattlesPerPair: {1}, ProductionMode: {2}, GodAccessOnly: {3}, GodModeSecret: {4}", HttpListenerPrefix, BattlesPerPair, ProductionMode, GodAccessOnly, GodModeSecret);
		}
	}
}