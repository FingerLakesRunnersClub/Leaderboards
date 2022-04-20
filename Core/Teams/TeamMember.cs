using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Ranking;

namespace FLRC.Leaderboards.Core.Teams;

public record TeamMember : IComparable<TeamMember>
{
	public Athlete Athlete { get; init; }

	public byte Courses { get; }
	public AgeGrade AgeGrade { get; }
	public ushort Runs { get; }
	public Miles Miles { get; }

	public Points Score => new(50 * Courses + AgeGrade.Value + Runs / 50.0 + Miles.Value / Distance.MilesPerMarathon);

	public TeamMember(IReadOnlyCollection<Ranked<Time>> results)
	{
		Courses = (byte) results.Count;
		AgeGrade = new AgeGrade(results.Average(r => r.AgeGrade.Value));
		Runs = (ushort) results.Sum(r => r.Count);
		Miles = new Miles(results.Sum(r => r.Count * r.Result.Course.Distance.Miles));
	}

	public int CompareTo(TeamMember other)
		=> Score.CompareTo(other.Score);
}