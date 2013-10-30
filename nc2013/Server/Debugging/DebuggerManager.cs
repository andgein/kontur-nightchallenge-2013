using Core.Game;
using JetBrains.Annotations;
using Server.Sessions;

namespace Server.Debugging
{
	public class DebuggerManager : IDebuggerManager
	{
		private readonly IGameServer gameServer;

		public DebuggerManager([NotNull] IGameServer gameServer)
		{
			this.gameServer = gameServer;
		}

		[NotNull]
		public IDebugger GetDebugger([NotNull] ISession session)
		{
			var debugger = (IDebugger) session.Items["debugger"];
			if (debugger == null)
			{
				debugger = new Debugger(gameServer, session);
				session.Items["debugger"] = debugger;
			}
			return debugger;
		}
	}
}