using System;
using System.IO;
using JetBrains.Annotations;

namespace Core
{
	public static class SettingsFileHelper
	{
		[NotNull]
		public static string PatchDirectoryName([NotNull] string dirName, [CanBeNull] string baseDirectoryPath = null)
		{
			return Path.IsPathRooted(dirName) ? dirName : WalkDirectoryTree(dirName, Directory.Exists, baseDirectoryPath);
		}

		[NotNull]
		private static string WalkDirectoryTree([NotNull] string filename, Func<string, bool> fileSystemObjectExists, [CanBeNull] string baseDirectoryPath = null)
		{
			if (baseDirectoryPath == null)
				baseDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
			var baseDirectory = new DirectoryInfo(baseDirectoryPath);
			while (baseDirectory != null)
			{
				var candidateFilename = Path.Combine(baseDirectory.FullName, filename);
				if (fileSystemObjectExists(candidateFilename))
					return candidateFilename;
				baseDirectory = baseDirectory.Parent;
			}
			return filename;
		}
	}
}