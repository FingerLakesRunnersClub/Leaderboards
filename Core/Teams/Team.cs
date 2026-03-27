using System.Collections.Immutable;

namespace FLRC.Leaderboards.Core.Teams;

public sealed record Team(byte Value) : Formatted<byte>(Value)
{
	public static readonly IImmutableDictionary<byte, Team> Teams = Enumerable.Range(2, 5).ToImmutableDictionary(t => (byte) t, t => new Team((byte) t));

	public byte MinAge => (byte) (Value >= 3 ? Value * 10 : 1);
	public byte? MaxAge => Value <= 5 ? (byte) (Value * 10 + 9) : null;

	public override string Display => MaxAge == null
		? $"{MinAge}+"
		: $"{MinAge}–{MaxAge}";
}