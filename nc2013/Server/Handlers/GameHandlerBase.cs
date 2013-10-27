using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace Server.Handlers
{
	public abstract class GameHandlerBase
	{
		private readonly string path;

		protected GameHandlerBase(string path)
		{
			this.path = path;
		}

		public bool Handle(HttpListenerContext context)
		{
			if (context.Request.Url.AbsolutePath.Equals("/corewars/" + path, StringComparison.OrdinalIgnoreCase))
			{
				DoHandle(context);
				context.Response.Close();
				return true;
			}
			return false;
		}

		protected Guid GetGameId(HttpListenerContext context)
		{
			var gameIdString = context.Request.QueryString["gameId"];
			if (string.IsNullOrEmpty(gameIdString))
				throw new HttpException(HttpStatusCode.BadRequest, "Query parameter 'gameId' is not specified");
			Guid gameId;
			if (!Guid.TryParse(gameIdString, out gameId))
				throw new HttpException(HttpStatusCode.BadRequest, "Query parameter 'gameId' is invalid - Guid is expected");
			return gameId;
		}

		protected int GetIntParam(HttpListenerContext context, string paramName)
		{
			var result = GetOptionalIntParam(context, paramName);
			if (!result.HasValue)
				throw new HttpException(HttpStatusCode.BadRequest, string.Format("Query parameter '{0}' is not specified", paramName));
			return result.Value;
		}

		protected int? GetOptionalIntParam(HttpListenerContext context, string paramName)
		{
			var valueString = context.Request.QueryString[paramName];
			if (string.IsNullOrEmpty(valueString))
				return null;
			int value;
			if (!int.TryParse(valueString, out value))
				throw new HttpException(HttpStatusCode.BadRequest, string.Format("Query parameter '{0}' is invalid - int is expected", paramName));
			return value;
		}

		protected string GetStringParam(HttpListenerContext context, string paramName)
		{
			var valueString = context.Request.QueryString[paramName];
			if (string.IsNullOrEmpty(valueString))
				throw new HttpException(HttpStatusCode.BadRequest, string.Format("Query parameter '{0}' is not specified", paramName));
			return valueString;
		}

		protected T GetRequest<T>(HttpListenerContext context)
		{
			var reader = new StreamReader(context.Request.InputStream);
			var data = reader.ReadToEnd();
			var result = JsonConvert.DeserializeObject<T>(data);
			return result;
		}

		protected void SendResponse<T>(HttpListenerContext context, T value)
		{
			context.Response.ContentType = "application/json; charset=utf-8";
			var result = JsonConvert.SerializeObject(value);
			using (var writer = new StreamWriter(context.Response.OutputStream))
				writer.Write(result);
			context.Response.Close();
		}

		protected void SendResponseRaw(HttpListenerContext context, object value, string contentType = null)
		{
			if (!ReferenceEquals(value, null))
			{
				if (!string.IsNullOrEmpty(contentType))
					context.Response.ContentType = contentType;
				using (var writer = new StreamWriter(context.Response.OutputStream))
					writer.Write(value);
			}
			context.Response.Close();
		}

		protected abstract void DoHandle(HttpListenerContext context);
	}
}