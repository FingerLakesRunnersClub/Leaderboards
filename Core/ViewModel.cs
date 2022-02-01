namespace FLRC.Leaderboards.Core;

public abstract class ViewModel
{
	public abstract string Title { get; }
	public IDictionary<uint, string> CourseNames { get; init; }
	public IDictionary<string, string> Links { get; init; }
}