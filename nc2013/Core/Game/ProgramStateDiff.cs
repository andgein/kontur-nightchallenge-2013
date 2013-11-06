using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Core.Game
{
	[JsonObject]
	public class ProgramStateDiff : IEquatable<ProgramStateDiff>
	{
		[JsonProperty]
		public int Program { get; set; }

		[JsonProperty]
		public ProcessStateChangeType ChangeType { get; set; }

		[JsonProperty]
		public uint? NextPointer { get; set; }

		[NotNull]
		public override string ToString()
		{
			return string.Format("Program: {0}, ChangeType: {1}, NextPointer: {2}", Program, ChangeType, NextPointer);
		}

		public bool Equals([CanBeNull] ProgramStateDiff other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Program == other.Program && ChangeType == other.ChangeType && NextPointer == other.NextPointer;
		}

		public override bool Equals([CanBeNull] object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((ProgramStateDiff) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = Program;
				hashCode = (hashCode*397) ^ (int) ChangeType;
				hashCode = (hashCode*397) ^ NextPointer.GetHashCode();
				return hashCode;
			}
		}
	}
}