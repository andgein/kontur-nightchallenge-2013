using System.Collections.Generic;
using Core.Parser;
using Newtonsoft.Json;

namespace Core.Arena
{
	[JsonObject]
	public class TournamentPlayer
	{
		[JsonProperty]
		public string Name;
		[JsonProperty]
		public int Version;
		[JsonIgnore]
		public Warrior Warrior;

		private sealed class NameVersionEqualityComparer : IEqualityComparer<TournamentPlayer>
		{
			public bool Equals(TournamentPlayer x, TournamentPlayer y)
			{
				if (ReferenceEquals(x, y)) return true;
				if (ReferenceEquals(x, null)) return false;
				if (ReferenceEquals(y, null)) return false;
				if (x.GetType() != y.GetType()) return false;
				return string.Equals(x.Name, y.Name) && x.Version == y.Version;
			}

			public int GetHashCode(TournamentPlayer obj)
			{
				unchecked
				{
					return ((obj.Name != null ? obj.Name.GetHashCode() : 0)*397) ^ obj.Version;
				}
			}
		}

		private static readonly IEqualityComparer<TournamentPlayer> nameVersionComparerInstance = new NameVersionEqualityComparer();

		public static IEqualityComparer<TournamentPlayer> NameVersionComparer
		{
			get { return nameVersionComparerInstance; }
		}
	}
}