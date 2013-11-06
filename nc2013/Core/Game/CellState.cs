using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Core.Game
{
	[JsonObject]
	public class CellState : IEquatable<CellState>
	{
		[NotNull]
		[JsonProperty]
		public string Instruction { get; set; }

		[JsonProperty]
		public CellType CellType { get; set; }

		[JsonProperty]
		public int? LastModifiedByProgram { get; set; }

		public bool Equals([CanBeNull] CellState other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(Instruction, other.Instruction) && CellType == other.CellType && LastModifiedByProgram == other.LastModifiedByProgram;
		}

		public override bool Equals([CanBeNull] object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((CellState) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = Instruction.GetHashCode();
				hashCode = (hashCode*397) ^ (int) CellType;
				hashCode = (hashCode*397) ^ LastModifiedByProgram.GetHashCode();
				return hashCode;
			}
		}

		[NotNull]
		public override string ToString()
		{
			return string.Format("Instruction: {0}, CellType: {1}, LastModifiedByProgram: {2}", Instruction, CellType, LastModifiedByProgram);
		}
	}
}