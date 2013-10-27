using System.Net;

namespace Server.Handlers
{
	public abstract class GameFuncHandlerBase<TIn, TOut> : GameHandlerBase
	{
		protected GameFuncHandlerBase(string path) : base(path) {}

		protected override sealed void DoHandle(HttpListenerContext context)
		{
			var request = GetRequest<TIn>(context);
			var response = Handle(request);
			SendResponse(context, response);
		}

		protected abstract TOut Handle(TIn request);
	}
}