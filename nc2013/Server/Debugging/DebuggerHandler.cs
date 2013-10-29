using System.Net;
using JetBrains.Annotations;
using Server.Handlers;
using Server.Sessions;

namespace Server.Debugging
{
	[RequireAuthorization]
	public class DebuggerHandler : StrictPathHttpHandlerBase
	{
		public DebuggerHandler() : base("debugger") {}

		public override void DoHandle([NotNull] HttpListenerContext context)
		{
			context.SendStaticFile("debugger.html");
		}
	}
}