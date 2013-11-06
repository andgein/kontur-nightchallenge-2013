using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Core.Game
{
	[JsonObject]
	public class Breakpoint : IEquatable<Breakpoint>
	{
		public Breakpoint(uint address, int program, BreakpointType breakpointType)
		{
			Address = address;
			Program = program;
			BreakpointType = breakpointType;
		}

		[JsonProperty]
		public uint Address { get; private set; }

		[JsonProperty]
		public int Program { get; private set; }

		[JsonProperty]
		public BreakpointType BreakpointType { get; private set; }

		public bool Equals([CanBeNull] Breakpoint other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Address == other.Address && Program == other.Program && BreakpointType == other.BreakpointType;
		}

		public override bool Equals([CanBeNull] object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((Breakpoint) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (int) Address;
				hashCode = (hashCode*397) ^ Program;
				hashCode = (hashCode*397) ^ (int) BreakpointType;
				return hashCode;
			}
		}

		[NotNull]
		public override string ToString()
		{
			return string.Format("Address: {0}, Program: {1}, BreakpointType: {2}", Address, Program, BreakpointType);
		}
	}
}