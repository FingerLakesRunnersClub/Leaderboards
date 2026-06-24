using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.ViewModels;

public sealed record ChallengeProgress
{
	public Athlete Athlete { get; init; }
    public Challenge Challenge { get; init; }

    public Course[] CompletedCourses { get; init; }
    public Course[] AllCourses { get; init; }

    public double CompletedMiles { get; init; }
    public double TotalMiles { get; init; }

    public int CoursePercent { get; init; }
    public double MileagePercent { get; set; }

    public bool ShowLinkButton { get; init; }
}