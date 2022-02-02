namespace FLRC.Leaderboards.Core.Reports;

public class Statistics
{
	public IDictionary<string, int> Participants { get; init; }
	public IDictionary<string, int> Runs { get; init; }
	public IDictionary<string, double> Miles { get; init; }
	public IDictionary<string, double> Average { get; init; }
}