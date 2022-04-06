using FLRC.Leaderboards.Core.Results;

namespace FLRC.Leaderboards.Core.Community;

public class Post
{
	public string Name { get; init; }
	public DateTime Date { get; init; }
	public string Content { get; init; }

	public bool HasNarrative()
		=> Content.Contains("## Narrative");

	public bool HasLocalBusiness()
		=> Content.Contains("## Local Business");

	public bool Matches(Result result)
		=> Name == result.Athlete.Name && Date.Date == result.StartTime.Value.Date;
}