using FLRC.Leaderboards.Core.Community;

namespace FLRC.Leaderboards.Core.Races;

public sealed class Race
{
	public uint ID { get; init; }
	public string Name { get; init; }
	public string Type { get; init; }
	public DateTime Date { get; init; }
	public string Source { get; init; }
	public string URL { get; init; }

	public Course[] Courses { get; set; } = [];
}