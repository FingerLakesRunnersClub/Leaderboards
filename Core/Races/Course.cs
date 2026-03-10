using FLRC.AgeGradeCalculator;
using FLRC.Leaderboards.Core.Results;

namespace FLRC.Leaderboards.Core.Races;

public sealed class Course
{
	public Race Race { get; init; }
	public uint ID { get; init; }
	public string Name => Race.Name + (Race.Courses?.Length > 1 ? $" ({ShortName})" : string.Empty);
	public string ShortName => Distance?.Display ?? Race?.Name;
	public Distance Distance { get; init; }
	public bool ShowDecimals { get; init; }

	public bool IsFieldEvent => Enum.TryParse<FieldEvent>(Name.ToFieldEvent(), out _);
	public string EventMetric => IsFieldEvent ? FieldEventMetric : "Time";
	public string EventSuperlative => IsFieldEvent ? FieldEventSuperlative : "Fastest";

	private string FieldEventMetric => Name is "High Jump" or "Pole Vault" ? "Height" : "Distance";
	private string FieldEventSuperlative => Name is "High Jump" or "Pole Vault" ? "Highest" : "Farthest";

	public DateTime LastUpdated { get; set; }
	public int LastHash { get; set; }

	public Result[] Results { get; set; } = [];
}