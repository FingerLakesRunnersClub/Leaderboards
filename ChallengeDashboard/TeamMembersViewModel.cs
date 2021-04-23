namespace FLRC.ChallengeDashboard
{
    public class TeamMembersViewModel : OverallResultsViewModel<AgeGrade>
    {
        public override string Title => $"Team {Team.Display} Members";

        public Team Team { get; init; }
    }
}