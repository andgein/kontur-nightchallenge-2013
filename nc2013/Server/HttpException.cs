using System;
using System.IO;
using System.Net;

namespace Server
{
	public class HttpException : Exception
	{
		private readonly HttpStatusCode httpStatusCode;

		public HttpException(HttpStatusCode httpStatusCode, string message) : base(message)
		{
			this.httpStatusCode = httpStatusCode;
		}

		public void WriteToResponse(HttpListenerResponse response)
		{
			response.StatusCode = (int)httpStatusCode;
			using (var writer = new StreamWriter(response.OutputStream))
				writer.Write(Message);
			response.Close();
		}
	}
}