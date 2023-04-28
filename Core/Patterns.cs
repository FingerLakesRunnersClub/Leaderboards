using System.Text.RegularExpressions;

namespace FLRC.Leaderboards.Core;

public static partial class Patterns
{
	[GeneratedRegex("([\\d\\.]+)(.*)")]
	public static partial Regex Distance();

	[GeneratedRegex("([a-z])([A-Z])")]
	public static partial Regex CamelCase();
}