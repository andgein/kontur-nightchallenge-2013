using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using log4net;
using log4net.Config;

namespace Server
{
	public static class Program
	{
		private const string defaultPrefix = "http://*/corewar/";

		private static readonly ILog log = LogManager.GetLogger(typeof (Program));

		public static void Main(string [] args)
		{
			var prefix = GetPrefix(args);
			XmlConfigurator.ConfigureAndWatch(new FileInfo("log.config.xml"));
			Runtime.Init(log);
			var httpServer = new GameHttpServer(prefix);
			Runtime.SetConsoleCtrlHandler(() =>
			{
				log.InfoFormat("Stopping...");
				httpServer.Stop();
			});
			httpServer.Run();
			log.InfoFormat("Listening {0}", prefix);
			Process.Start(httpServer.DefaultUrl);
			httpServer.WaitForTermination();
			log.InfoFormat("Stopped");
		}

		private static string GetPrefix(IEnumerable<string> args)
		{
			var prefix = args.FirstOrDefault();
			return string.IsNullOrEmpty(prefix) ? defaultPrefix : prefix;
		}
	}
}