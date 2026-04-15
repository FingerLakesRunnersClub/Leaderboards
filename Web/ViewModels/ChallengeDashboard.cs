using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.ViewModels;

public sealed record ChallengeDashboard
{
	public Athlete Athlete { get; init; }
	public Challenge Challenge { get; init; }
	public Course[] CoursesCompleted { get; set; }
}