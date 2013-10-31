using System;
using System.IO;
using System.Net;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Server
{
	public static class GameHttpContextExtensions
	{
		public static Guid GetGuidParam([NotNull] this GameHttpContext context, [NotNull] string paramName)
		{
			var value = context.GetOptionalGuidParam(paramName);
			if (!value.HasValue)
				throw new HttpException(HttpStatusCode.BadRequest, String.Format("Query parameter '{0}' is not specified", paramName));
			return value.Value;
		}

		public static Guid? GetOptionalGuidParam([NotNull] this GameHttpContext context, [NotNull] string paramName)
		{
			var guidString = context.Request.QueryString[paramName];
			if (String.IsNullOrEmpty(guidString))
				return null;
			Guid values;
			if (!Guid.TryParse(guidString, out values))
				throw new HttpException(HttpStatusCode.BadRequest, String.Format("Query parameter '{0}' is invalid - Guid is expected", paramName));
			return values;
		}

		public static int GetIntParam([NotNull] this GameHttpContext context, string paramName)
		{
			var result = context.GetOptionalIntParam(paramName);
			if (!result.HasValue)
				throw new HttpException(HttpStatusCode.BadRequest, String.Format("Query parameter '{0}' is not specified", paramName));
			return result.Value;
		}

		public static int? GetOptionalIntParam([NotNull] this GameHttpContext context, string paramName)
		{
			var valueString = context.Request.QueryString[paramName];
			if (String.IsNullOrEmpty(valueString))
				return null;
			int value;
			if (!Int32.TryParse(valueString, out value))
				throw new HttpException(HttpStatusCode.BadRequest, String.Format("Query parameter '{0}' is invalid - int is expected", paramName));
			return value;
		}

		public static string GetStringParam([NotNull] this GameHttpContext context, string paramName)
		{
			var valueString = context.Request.QueryString[paramName];
			if (String.IsNullOrEmpty(valueString))
				throw new HttpException(HttpStatusCode.BadRequest, String.Format("Query parameter '{0}' is not specified", paramName));
			return valueString;
		}

		public static string GetOptionalStringParam([NotNull] this GameHttpContext context, string paramName)
		{
			var valueString = context.Request.QueryString[paramName];
			return String.IsNullOrEmpty(valueString) ? null : valueString;
		}

		public static T GetRequest<T>([NotNull] this GameHttpContext context)
		{
			var reader = new StreamReader(context.Request.InputStream);
			var data = reader.ReadToEnd();
			var result = JsonConvert.DeserializeObject<T>(data, new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});
			return result;
		}

		public static void SendResponse<T>([NotNull] this GameHttpContext context, T value, HttpStatusCode statusCode = HttpStatusCode.OK)
		{
			context.Response.StatusCode = (int) statusCode;
			context.Response.ContentType = "application/json; charset=utf-8";
			var result = JsonConvert.SerializeObject(value, new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver(), Formatting = Formatting.Indented});
			using (var writer = new StreamWriter(context.Response.OutputStream))
				writer.Write(result);
		}

		public static void SendResponseRaw([NotNull] this GameHttpContext context, object value, string contentType = null)
		{
			if (!ReferenceEquals(value, null))
			{
				if (!String.IsNullOrEmpty(contentType))
					context.Response.ContentType = contentType;
				using (var writer = new StreamWriter(context.Response.OutputStream))
					writer.Write(value);
			}
		}

		public static void SendResponseRaw([NotNull] this GameHttpContext context, [CanBeNull] byte[] value, string contentType = null)
		{
			if (value != null)
			{
				if (!String.IsNullOrEmpty(contentType))
					context.Response.ContentType = contentType;
				context.Response.OutputStream.Write(value, 0, value.Length);
			}
		}

		public static void Redirect([NotNull] this GameHttpContext context, [NotNull] string url)
		{
			context.Response.StatusCode = (int) HttpStatusCode.Redirect;
			context.Response.RedirectLocation = url;
		}

		public static bool IsAjax([NotNull] this GameHttpContext context)
		{
			return context.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
		}

		public static void SetCookie([NotNull] this GameHttpContext context, [NotNull] string cookieName, [NotNull] string cookieValue, bool httpOnly, bool persistent)
		{
			var header = string.Format("{0}={1}; path={2}", cookieName, cookieValue, context.BasePath);
			if (persistent)
				header += "; expires=" + DateTime.Now.AddYears(1).ToString("R");
			if (httpOnly)
				header += "; httponly";
			context.Response.AppendHeader("Set-Cookie", header);
		}

		[CanBeNull]
		public static string TryGetCookie([NotNull] this GameHttpContext context, [NotNull] string cookieName)
		{
			var cookie = context.Request.Cookies[cookieName];
			if (cookie == null || string.IsNullOrEmpty(cookie.Value))
				return null;
			return cookie.Value;
		}

		public delegate bool TryParseDelegate<T>([NotNull] string source, out T result);

		[CanBeNull]
		public static T? TryGetCookie<T>([NotNull] this GameHttpContext context, [NotNull] string cookieName, [NotNull] TryParseDelegate<T> tryParse) where T : struct
		{
			var cookieValue = TryGetCookie(context, cookieName);
			T result;
			if (string.IsNullOrEmpty(cookieValue) || !tryParse(cookieValue, out result))
				return null;
			return result;
		}

		[CanBeNull]
		public static string TryGetContentType([NotNull] string file)
		{
			file = file.ToLower();
			if (Path.GetExtension(file) == ".js")
				return "text/javascript; encoding=utf-8";
			if (Path.GetExtension(file) == ".html" || Path.GetExtension(file) == ".htm")
				return "text/html; encoding=utf-8";
			if (Path.GetExtension(file) == ".css")
				return "text/css; encoding=utf-8";
			if (Path.GetExtension(file) == ".txt")
				return "text/plain; encoding=utf-8";
			if (Path.GetExtension(file) == ".gif")
				return "image/gif";
			return null;
		}

		private static bool TryHandleStatic([NotNull] this GameHttpContext context, [NotNull] string path)
		{
			if (!File.Exists(path)) return false;
			var contentType = TryGetContentType(path);
			context.SendResponseRaw(File.ReadAllBytes(path), contentType);
			return true;
		}

		public static void SendStaticFile([NotNull] this GameHttpContext context, [NotNull] string localPath)
		{
			if (!context.TryHandleStatic(Path.Combine("StaticContent", localPath)))
				throw new HttpException(HttpStatusCode.NotFound, String.Format("Static resource '{0}' is not found", context.Request.RawUrl));
		}
	}
}