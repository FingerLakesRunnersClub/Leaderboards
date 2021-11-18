namespace FLRC.ChallengeDashboard;

public class TeamResults
{
	public Rank Rank { get; set; }
	public Team Team { get; init; }

	public AgeGrade AverageAgeGrade { get; init; }
	public byte AgeGradePoints { get; set; }

	public ushort TotalRuns { get; init; }
	public byte MostRunsPoints { get; set; }

	public byte TotalPoints => (byte)(AgeGradePoints + MostRunsPoints);
}