using Core.Parser;
using Server.Handlers;

namespace Server
{
	public class CommandDescribeHandler : StrictPathHttpHandlerBase
	{
		public CommandDescribeHandler() : base("describe") { }

		public override void Handle(GameHttpContext context, bool godMode)
		{
			var cmd = context.GetOptionalStringParam("cmd");
			var description = new CommandDescriber().Describe(cmd);
			context.SendResponseRaw(description, "text/plain; charset=utf-8");
		}
	}
}