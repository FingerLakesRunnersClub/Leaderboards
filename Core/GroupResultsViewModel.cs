namespace FLRC.Leaderboards.Core;

public class GroupResultsViewModel : OverallResultsViewModel<TeamMember>
{
	public override string Title => $"{Name} Group Members";

	public string Name { get; init; }
}