namespace FLRC.Leaderboards.Core.Teams;

public record Team(byte Value) : Formatted<byte>(Value)
{
	public byte MinAge => (byte) (Value >= 3 ? Value * 10 : 1);
	public byte? MaxAge => Value <= 5 ? (byte) (Value * 10 + 9) : null;

	public override string Display => MaxAge == null
		? $"{MinAge}+"
		: $"{MinAge}–{MaxAge}";
}