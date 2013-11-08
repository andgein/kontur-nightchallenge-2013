using JetBrains.Annotations;

namespace Server.Handlers
{
	public class RootHandler : StrictPathHttpHandlerBase
	{
		public RootHandler()
			: base(string.Empty)
		{
		}

		public override void Handle([NotNull] GameHttpContext context)
		{
			context.Redirect(context.BasePath + "index.html");
		}
	}
}