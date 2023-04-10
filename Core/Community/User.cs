namespace FLRC.Leaderboards.Core.Community;

public record User
{
	public ushort ID { get; init; }
	public string Name { get; init; }
	public string Username { get; init; }
}