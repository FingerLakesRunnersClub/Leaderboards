using FLRC.Leaderboards.Core.Overall;
using FLRC.Leaderboards.Core.Teams;

namespace FLRC.Leaderboards.Core.Groups;

public class GroupResultsViewModel : OverallResultsViewModel<TeamMember>
{
	public override string Title => $"{Name} Group Members";

	public string Name { get; init; }
}