using System.Text.RegularExpressions;
using FLRC.Leaderboards.Core.Races;

namespace FLRC.Leaderboards.Core.Metrics;

public sealed record Performance(string Value) : Distance(Value)
{
	public override double Meters { get; } = ParsePerformance(Value) ?? ParseDistance(Value);

	private const double InchesPerMeter = 1 / 0.0254;
	private static readonly Regex RegexPattern = Patterns.ImperialMeasurement();

	private static double? ParsePerformance(string value)
	{
		var split = RegexPattern.Match(value).Groups;
		if (split.Count < 2)
			return null;

		var feet = double.Parse(split[1].Value.Trim());
		var inches = double.Parse(split[2].Value.Trim());
		var totalInches = feet * 12 + inches;

		return totalInches / InchesPerMeter;
	}
}