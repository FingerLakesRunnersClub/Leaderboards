namespace FLRC.Leaderboards.Core.Athletes;

public class AthleteSummaryViewModel : ViewModel
{
	public override string Title => Summary.Athlete.Name;

	public AthleteSummary Summary { get; init; }
}