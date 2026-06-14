using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.ViewModels;

public sealed record ChallengeProgress
{
	public Athlete Athlete { get; init; }
    public Challenge Challenge { get; init; }
    public Course[] CompletedCourses { get; init; }
    public Course[] AllCourses { get; init; }
    public int PercentComplete { get; init; }
    public bool ShowLinkButton { get; init; }
}