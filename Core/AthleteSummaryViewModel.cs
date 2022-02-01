namespace FLRC.Leaderboards.Core;

public class AthleteSummaryViewModel : ViewModel
{
	public override string Title => Summary.Athlete.Name;

	public AthleteSummary Summary { get; init; }
}