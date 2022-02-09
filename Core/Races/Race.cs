namespace FLRC.Leaderboards.Core.Races;

public class Race
{
	public uint ID { get; init; }
	public string Name { get; init; }
	public string Type { get; init; }
	public DateTime Date { get; set; }
	public string Source { get; init; }
	public string URL { get; init; }
	public IReadOnlyCollection<Course> Courses { get; set; }
}