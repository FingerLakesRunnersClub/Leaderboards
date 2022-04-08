using System.Text.RegularExpressions;
using FLRC.Leaderboards.Core.Results;

namespace FLRC.Leaderboards.Core.Community;

public class Post
{
	public string Name { get; init; }
	public DateTime Date { get; init; }
	public string Content { get; init; }

	public bool Matches(Result result)
		=> Name == result.Athlete.Name && Date.Date == result.StartTime.Value.Date;

	public bool HasNarrative()
		=> HasHeader("## Narrative");

	public bool HasLocalBusiness()
		=> HasHeader("## Local Business");

	private bool HasHeader(string text)
		=> Content.Contains(text) && NotQuoted(text);

	private bool NotQuoted(string text)
		=> !Regex.IsMatch(Content, $@"\[quote.*\].*{text}.*\[/quote.*\]");
}