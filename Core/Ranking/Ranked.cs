using System.Text.Json.Serialization;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Results;

namespace FLRC.Leaderboards.Core.Ranking;

public class Ranked<T>
{
	[JsonIgnore]
	public RankedList<T> All { get; init; }
	public Rank Rank { get; init; }
	public Result Result { get; init; }
	public T Value { get; init; }
	public uint Count { get; init; }

	public AgeGrade AgeGrade { get; init; }

	public Time BehindLeader { get; init; }

	public Points Points { get; init; }
}