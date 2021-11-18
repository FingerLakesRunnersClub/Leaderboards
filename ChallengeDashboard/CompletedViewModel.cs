namespace FLRC.ChallengeDashboard;

public class CompletedViewModel : OverallResultsViewModel<Date>
{
	public override string Title => "Completions";
	public override string ResponsiveBreakpoint => "lg";
}