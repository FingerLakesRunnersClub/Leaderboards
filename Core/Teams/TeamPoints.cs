namespace FLRC.Leaderboards.Core.Teams;

public record TeamPoints(byte Value) : Formatted<byte>(Value)
{
	public override string Display => Value.ToString();
}