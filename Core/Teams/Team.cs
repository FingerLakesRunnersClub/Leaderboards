namespace FLRC.Leaderboards.Core.Teams;

public record Team(byte Value) : Formatted<byte>(Value)
{
	public override string Display => Value == 2 ? "1–29"
		: Value == 6 ? "60+"
		: $"{Value}0–{Value}9";
}