using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace Core
{
	public static class RandomExtensions
	{
		public static bool NextBool([NotNull] this Random random)
		{
			return random.Next() % 2 == 0;
		}

		public static byte NextByte([NotNull] this Random random)
		{
			return (byte)random.Next();
		}

		public static uint NextUint([NotNull] this Random random)
		{
			return (uint)random.Next();
		}

		public static ushort NextUshort([NotNull] this Random random)
		{
			return (ushort)random.Next(ushort.MinValue, ushort.MaxValue);
		}

		public static long NextLong([NotNull] this Random random)
		{
			var highBits = ((long)random.Next()) << 32;
			var lowBits = (long)random.Next();
			return highBits + lowBits;
		}

		public static DateTime NextDateTime([NotNull] this Random random)
		{
			var ticks = (long)(random.NextDouble() * (DateTime.MaxValue.Ticks - DateTime.MinValue.Ticks) + DateTime.MinValue.Ticks);
			return new DateTime(ticks);
		}

		public static Guid NextGuid([NotNull] this Random random)
		{
			return new Guid(random.NextBytes(16));
		}

		[NotNull]
		public static string NextString([NotNull] this Random random)
		{
			var count = random.Next() % 32 + 2;
			var sb = new StringBuilder();
			for (var i = 0; i < count; i++)
				sb.Append((char)('a' + random.Next() % 26));
			return sb.ToString();
		}

		[NotNull]
		public static byte[] NextBytes([NotNull] this Random random, int length)
		{
			var buf = new byte[length];
			random.NextBytes(buf);
			return buf;
		}

		public static T NextItem<T>([NotNull] this Random random, [NotNull] IList<T> items)
		{
			return items[random.Next(items.Count)];
		}

		public static void Shuffle<T>([NotNull] this Random random, [NotNull] IList<T> items)
		{
			for (var i = 0; i < items.Count - 1; i++)
			{
				var j = random.Next(i + 1, items.Count);
				var tmp = items[i];
				items[i] = items[j];
				items[j] = tmp;
			}
		}
	}
}