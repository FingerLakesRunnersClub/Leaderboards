namespace FLRC.ChallengeDashboard
{
    public class TeamMembersViewModel : OverallResultsViewModel<double>
    {
        public override string Title => $"Team {Team.Display} Runners";

        public Team Team { get; init; }
    }
}