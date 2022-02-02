using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Ranking;

namespace FLRC.Leaderboards.Core.Athletes;

public class AthleteLogViewModel : DataTableViewModel
{
	public Athlete Athlete { get; init; }
	public RankedList<Time> Results { get; init; }
	public override string Title => $"{Athlete.Name} — Activity Log";
	public override string ResponsiveBreakpoint => "md";
}