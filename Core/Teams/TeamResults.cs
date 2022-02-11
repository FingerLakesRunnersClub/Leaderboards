using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Ranking;

namespace FLRC.Leaderboards.Core.Teams;

public class TeamResults : Ranked<TeamPoints>
{
	public new Rank Rank { get; set; }
	public Team Team { get; init; }

	public AgeGrade AverageAgeGrade { get; init; }
	public byte AgeGradePoints { get; set; }

	public ushort TotalRuns { get; init; }
	public byte MostRunsPoints { get; set; }

	public byte TotalPoints => (byte)(AgeGradePoints + MostRunsPoints);
}