using FLRC.Leaderboards.Core.Ranking;

namespace FLRC.Leaderboards.Core.Series;

public sealed record Series
{
	public string ID { get; init; }
	public string Name { get; init; }
	public uint[] Races { get; init; }
	public byte HourLimit { get; init; }

	public RankedList<SeriesResult> Results { get; init; }
}
