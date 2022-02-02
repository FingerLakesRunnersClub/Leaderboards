namespace FLRC.Leaderboards.Core.Teams;

public record Team : Formatted<byte>
{
	public Team(byte value) : base(value)
	{
	}

	public override string Display => Value == 2 ? "1–29"
		: Value == 6 ? "60+"
		: $"{Value}0–{Value}9";
}