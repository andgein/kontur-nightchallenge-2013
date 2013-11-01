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
		public string Program;

		[JsonIgnore]
		public Warrior Warrior;

		private bool Equals(TournamentPlayer other)
		{
			return string.Equals(Name, other.Name) && Version == other.Version;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((TournamentPlayer) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((Name != null ? Name.GetHashCode() : 0)*397) ^ Version;
			}
		}

		public override string ToString()
		{
			return string.Format("Name: {0}, Version: {1}, Program: {2}", Name, Version, Program);
		}
	}
}