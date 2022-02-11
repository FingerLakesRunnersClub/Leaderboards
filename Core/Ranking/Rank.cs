namespace FLRC.Leaderboards.Core.Ranking;

public record Rank(ushort Value) : Formatted<ushort>(Value), IComparable<Rank>
{
	public override string Display =>
		Value > 0
			? (Value % 100) switch
			{
				11 => Value + "th",
				12 => Value + "th",
				13 => Value + "th",
				_ => (Value % 10) switch
				{
					1 => Value + "st",
					2 => Value + "nd",
					3 => Value + "rd",
					_ => Value + "th"
				}
			}
			: Value.ToString();

	public int CompareTo(Rank other) => Value.CompareTo(other.Value);
}