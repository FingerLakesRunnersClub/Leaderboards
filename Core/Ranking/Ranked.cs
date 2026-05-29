using System.Text.Json.Serialization;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;

namespace FLRC.Leaderboards.Core.Ranking;

public record Ranked<T,TR>
{
	[JsonIgnore]
	public RankedList<T,TR> All { get; init; }
	public Rank Rank { get; init; }
	public TR Result { get; init; }
	public T Value { get; init; }
	public uint Count { get; init; }

	public Date StartTime { get; init; }
	public AgeGrade AgeGrade { get; init; }

	public Time BehindLeader { get; init; }

	public Points Points { get; init; }
}