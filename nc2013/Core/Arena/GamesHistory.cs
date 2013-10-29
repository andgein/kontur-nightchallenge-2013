using System.IO;

namespace Core.Arena
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