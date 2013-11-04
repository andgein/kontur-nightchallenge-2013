using JetBrains.Annotations;

namespace Server.Handlers
{
	public class IndexHandler : StrictPathHttpHandlerBase
	{
		public IndexHandler() : base("")
		{
		}

		public override void Handle([NotNull] GameHttpContext context, bool godMode)
		{
			context.Redirect(context.BasePath + "index.html");
		}
	}
}