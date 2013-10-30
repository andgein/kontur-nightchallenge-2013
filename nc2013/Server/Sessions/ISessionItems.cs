using JetBrains.Annotations;

namespace Server.Sessions
{
	public interface ISessionItems
	{
		[CanBeNull]
		object this[object key] { get; set; }
	}
}