using System;
using System.Globalization;
using NUnit.Framework;

namespace Server
{
	[TestFixture]
	public class Timestamp_Test
	{
		[Test]
		public void Parse()
		{
			var ts = new DateTime(2013, 11, 08, 15, 0, 0, DateTimeKind.Utc);
			var tsStr = ts.ToString("u");
			Console.Out.WriteLine(tsStr);
			DateTime parsedTs;
			DateTime.TryParseExact(tsStr, "u", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out parsedTs);
			Assert.That(parsedTs, Is.EqualTo(ts));
			Assert.That(parsedTs.Kind, Is.EqualTo(DateTimeKind.Utc));
		}

		[Test]
		public void Formatting()
		{
			var ts = TimeSpan.FromHours(12);
			Console.Out.WriteLine(ts.ToString("c"));
		}
	}
}