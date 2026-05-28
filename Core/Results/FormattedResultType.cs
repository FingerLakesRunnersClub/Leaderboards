using System.Text.RegularExpressions;

namespace FLRC.Leaderboards.Core.Results;

public sealed record FormattedResultType(ResultType Value) : Formatted<ResultType>(Value)
{
	private static readonly Regex RegexPattern = Patterns.CamelCase();

	public override string Display
		=> RegexPattern.Replace(Enum.GetName(Value) ?? string.Empty, "$1 $2");
}