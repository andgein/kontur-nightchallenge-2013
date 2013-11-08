using JetBrains.Annotations;
using Server.Handlers;

namespace Server.Debugging
{
	public abstract class DebuggerHandlerBase : StrictPathHttpHandlerBase
	{
		private readonly IDebuggerManager debuggerManager;

		protected DebuggerHandlerBase([NotNull] string path, [NotNull] IDebuggerManager debuggerManager) : base(path)
		{
			this.debuggerManager = debuggerManager;
		}

		public override sealed void Handle([NotNull] GameHttpContext context)
		{
			var debugger = debuggerManager.GetDebugger(context.Session);
			DoHandle(context, debugger, context.GodMode);
		}

		protected abstract void DoHandle([NotNull] GameHttpContext context, [NotNull] IDebugger debugger, bool godMode);
	}
}