using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Core.Game
{
	[JsonObject]
	public class MemoryDiff : IEquatable<MemoryDiff>
	{
		[JsonProperty]
		public uint Address { get; set; }

		[JsonProperty]
		[NotNull]
		public CellState CellState { get; set; }

		public bool Equals([CanBeNull] MemoryDiff other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Address == other.Address && CellState.Equals(other.CellState);
		}

		public override bool Equals([CanBeNull] object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((MemoryDiff) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((int) Address*397) ^ CellState.GetHashCode();
			}
		}

		[NotNull]
		public override string ToString()
		{
			return string.Format("Address: {0}, CellState: {1}", Address, CellState);
		}
	}
}