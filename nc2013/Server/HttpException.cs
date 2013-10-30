using System;
using System.IO;
using System.Net;
using JetBrains.Annotations;

namespace Server
{
	public class HttpException : Exception
	{
		private readonly HttpStatusCode httpStatusCode;

		public HttpException(HttpStatusCode httpStatusCode, [NotNull] string message, Exception innerException = null) : base(message, innerException)
		{
			this.httpStatusCode = httpStatusCode;
		}

		public void WriteToResponse(HttpListenerResponse response)
		{
			response.StatusCode = (int) httpStatusCode;
			using (var writer = new StreamWriter(response.OutputStream))
			{
				writer.WriteLine("[[" + Message + "]]");
				if (InnerException != null)
					writer.Write(InnerException);
			}
			response.Close();
		}
	}
}