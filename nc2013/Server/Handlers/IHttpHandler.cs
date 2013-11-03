using JetBrains.Annotations;

namespace Server.Handlers
{
	public interface IHttpHandler
	{
		bool CanHandle([NotNull] GameHttpContext context);
		void Handle([NotNull] GameHttpContext context, bool godMode);
	}
}