namespace FLRC.ChallengeDashboard;

public class AthleteSummaryViewModel : ViewModel
{
	public override string Title => Summary.Athlete.Name;

	public AthleteSummary Summary { get; init; }
}
