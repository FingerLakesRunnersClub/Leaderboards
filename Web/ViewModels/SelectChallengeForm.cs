using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.ViewModels;

public sealed record SelectChallengeForm
{
    public Iteration Iteration { get; init; }
    public Challenge Official { get; init; }
    public Course[] Courses { get; init; }

    public string Selection { get; init; }
    public Guid[] Selected { get; init; }
}