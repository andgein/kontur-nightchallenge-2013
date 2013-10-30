using JetBrains.Annotations;
using Server.Handlers;

namespace Server.Debugging
{
	public class DebuggerHandler : StrictPathHttpHandlerBase
	{
		public DebuggerHandler() : base("debugger") {}

		public override void Handle([NotNull] GameHttpContext context)
		{
			context.SendStaticFile("debugger.html");
		}
	}
}