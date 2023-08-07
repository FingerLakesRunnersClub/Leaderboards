using FLRC.Leaderboards.Core.Teams;

namespace FLRC.Leaderboards.Core.Overall;

public sealed class GroupResultsViewModel : OverallResultsViewModel<TeamMember>
{
	public override string Title => $"{Name} Group Members";

	public string Name { get; init; }
}