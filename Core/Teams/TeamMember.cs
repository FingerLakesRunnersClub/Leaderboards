using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Ranking;

namespace FLRC.Leaderboards.Core.Teams;

public class TeamMember : IComparable<TeamMember>
{
	public AgeGrade AgeGrade { get; }
	public ushort Runs { get; }
	public double Miles { get; }

	public TeamMember(IReadOnlyCollection<Ranked<Time>> results)
	{
		AgeGrade = new AgeGrade(results.Average(r => r.AgeGrade.Value));
		Runs = (ushort)results.Sum(r => r.Count);
		Miles = Math.Round(results.Sum(r => r.Count * r.Result.Course.Distance.Miles), 1);
	}

	public int CompareTo(TeamMember other) => AgeGrade.CompareTo(other.AgeGrade);
}