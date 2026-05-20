using System.Text.RegularExpressions;

namespace FLRC.Leaderboards.Model;

public sealed record CommunityPost
{
	public ushort ID { get; init; }
	public string Name { get; init; } = null!;
	public DateTime Date { get; init; }
	public string Content { get; init; } = null!;

	public bool Matches(Result result)
		=> result.Athlete.LinkedAccounts.Any(l => l.Type == LinkedAccount.Keys.Discourse && l.Value == ID.ToString())
		   && Date.Date == result.StartTime.Date;

	public bool HasNarrative()
		=> HasHeader("## Story");

	private bool HasHeader(string text)
		=> Content.Contains(text) && NotQuoted(text);

	private bool NotQuoted(string text)
		=> !Regex.IsMatch(Content, $@"\[quote.*\].*{text}.*\[/quote.*\]", RegexOptions.None, TimeSpan.FromSeconds(1));
}