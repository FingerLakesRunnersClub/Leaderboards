using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.ViewModels;

public sealed record TeamMember : IComparable<TeamMember>
{
	public Athlete Athlete { get; init; }

	public byte Courses { get; }
	public AgeGrade AgeGrade { get; }
	public ushort Runs { get; }
	public Miles Miles { get; }

	public Points Score => new(50 * Courses + AgeGrade.Value + Runs / 50.0 + Miles.Value / Distance.MilesPerMarathon);

	public TeamMember(Ranked<Time, Result>[] results)
	{
		Courses = (byte)results.Length;
		AgeGrade = new AgeGrade(results.Average(r => r.AgeGrade?.Value ?? 0));
		Runs = (ushort) results.Sum(r => r.Count);
		Miles = new Miles(results.Sum(r => r.Count * new Distance(r.Result.Course.DistanceDisplay).Miles));
	}

	public int CompareTo(TeamMember other)
		=> Score.CompareTo(other.Score);
}