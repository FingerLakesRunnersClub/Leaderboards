using FLRC.Leaderboards.Core.Metrics;

namespace FLRC.Leaderboards.Core.Teams;

public record TeamResults : Formattable
{
	public Team Team { get; init; }
	public AgeGrade AverageAgeGrade { get; init; }
	public byte AgeGradePoints { get; set; }

	public ushort TotalRuns { get; init; }
	public byte MostRunsPoints { get; set; }

	public byte TotalPoints => (byte) (AgeGradePoints + MostRunsPoints);

	public string Display => TotalPoints.ToString();
}