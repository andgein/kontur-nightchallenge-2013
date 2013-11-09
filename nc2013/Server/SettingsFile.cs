using System;
using System.Globalization;
using System.IO;
using JetBrains.Annotations;
using log4net;

namespace Server
{
	public class SettingsFile
	{
		private const string defaultPrefix = "http://*:8080/corewar/";
		private const int defaultBattlesPerPair = 5;
		private const int defaultContestDurationInHours = 12;

		private static readonly ILog log = LogManager.GetLogger(typeof(SettingsFile));

		public SettingsFile([CanBeNull] string settingsFilename)
		{
			HttpListenerPrefix = defaultPrefix;
			BattlesPerPair = defaultBattlesPerPair;
			ProductionMode = false;
			GodAccessOnly = false;
			GodModeSecret = Guid.NewGuid().ToString();
			ContestStartTimestamp = null;
			ContestDurationInHours = defaultContestDurationInHours;
			if (string.IsNullOrEmpty(settingsFilename))
				log.Warn("Using default settings");
			else if (!File.Exists(settingsFilename))
				log.Warn(string.Format("Settings file {0} not found - using default settings", settingsFilename));
			else
			{
				var lines = File.ReadLines(settingsFilename);
				foreach (var line in lines)
				{
					if (string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith("#"))
						continue;
					var strings = line.Split(new[] { '=' }, 2);
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
					else if (string.Equals(settingName, "SubmitIsAllowed", StringComparison.OrdinalIgnoreCase))
						ParseBoolSetting(settingName, settingValue, v => SubmitIsAllowed = v);
					else if (string.Equals(settingName, "EnableDeepNavigation", StringComparison.OrdinalIgnoreCase))
						ParseBoolSetting(settingName, settingValue, v => EnableDeepNavigation = v);
					else if (string.Equals(settingName, "ContestStartTimestamp", StringComparison.OrdinalIgnoreCase))
						ParseTimestampSetting(settingName, settingValue, v => ContestStartTimestamp = v);
					else if (string.Equals(settingName, "ContestDurationInHours", StringComparison.OrdinalIgnoreCase))
						ParseIntSetting(settingName, settingValue, v => ContestDurationInHours = v);
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

		private static void ParseTimestampSetting([NotNull] string settingName, [NotNull] string settingValue, Action<DateTime> setter)
		{
			if (string.IsNullOrWhiteSpace(settingValue))
				log.Warn(string.Format("Empty value of setting '{0}'", settingName));
			else
			{
				DateTime settingIntValue;
				if (!DateTime.TryParseExact(settingValue, "u", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out settingIntValue))
					log.Warn(string.Format("Invalid timestamp value for setting '{0}' in settings file: {1}", settingName, settingValue));
				else
					setter(settingIntValue);
			}
		}

		[NotNull]
		public string HttpListenerPrefix { get; private set; }

		public int BattlesPerPair { get; private set; }
		public bool ProductionMode { get; private set; }
		public bool GodAccessOnly { get; private set; }
		public bool SubmitIsAllowed { get; private set; }
		public bool EnableDeepNavigation { get; private set; }

		[NotNull]
		public string GodModeSecret { get; private set; }

		public DateTime? ContestStartTimestamp { get; private set; }

		public int ContestDurationInHours { get; private set; }

		public override string ToString()
		{
			return string.Format("HttpListenerPrefix: {0}, BattlesPerPair: {1}, ProductionMode: {2}, GodAccessOnly: {3}, GodModeSecret: {4}, ContestStartTimestamp: {5} UTC, ContestDurationInHours: {6}, SubmitIsAllowed: {7}, EnableDeepNavigation: {8}",
				HttpListenerPrefix, BattlesPerPair, ProductionMode, GodAccessOnly, GodModeSecret, ContestStartTimestamp, ContestDurationInHours, SubmitIsAllowed, EnableDeepNavigation);
		}
	}
}