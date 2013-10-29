using System.IO;
using Core.Game;

namespace Core.Arenas
{
	public class GamesHistory
	{
		private readonly DirectoryInfo historyDir;

		public GamesHistory(DirectoryInfo historyDir)
		{
			this.historyDir = historyDir;
		}

	}
}