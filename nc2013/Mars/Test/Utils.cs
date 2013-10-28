// This file is part of nMars - Core War MARS for .NET 
// Whole solution including it's license could be found at
// http://sourceforge.net/projects/nmars/
// 2006 Pavel Savara

using System;
using System.IO;
using Mars.Test;

namespace nMars.Test
{
	public class Utils
    {
        public static string CleanProblems(string basePath, string name)
        {
            string problemsPath = Path.Combine(basePath, name);
            if (Directory.Exists(problemsPath))
            {
                try
                {
                    Directory.Delete(problemsPath, true);
                }
                catch (Exception)
                {
                    //swalow
                }
            }
            Directory.CreateDirectory(problemsPath);
            return problemsPath;
        }

        public static string GetWarrirorsDirectory()
        {
	        return SettingsFileHelper.PatchDirectoryName("warriors", Directory.GetCurrentDirectory());
        }
    }
}