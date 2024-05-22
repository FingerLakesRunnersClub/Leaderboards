using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Ranking;

namespace FLRC.Leaderboards.Core.Athletes;

public sealed class AthleteLogViewModel : DataTableViewModel
{
	public AthleteHeader Header { get; init; }
	public Athlete Athlete => Header.Athlete;
	public RankedList<Time> Results { get; init; }
	public override string Title => $"{Athlete.Name} — Activity Log";
	public override string ResponsiveBreakpoint => "lg";
}