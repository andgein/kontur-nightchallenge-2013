using System;
using System.Collections.Generic;
using System.IO;

namespace Tests
{
	public static class TestWarriors
	{
		public const string basePath = @"..\..\..\..\";

		public static IEnumerable<string> GetBotFiles(string folder)
		{
			return Directory.EnumerateFiles(basePath + folder, "*.red", SearchOption.AllDirectories);
		}
		
		public static string GetBotFile(string name)
		{
			return basePath + name;
		}
	}
}