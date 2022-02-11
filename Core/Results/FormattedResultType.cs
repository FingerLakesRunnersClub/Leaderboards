using System.Text.RegularExpressions;

namespace FLRC.Leaderboards.Core.Results;

public record FormattedResultType(ResultType Value) : Formatted<ResultType>(Value)
{
	public override string Display
		=> Regex.Replace(Enum.GetName(Value) ?? string.Empty, "([a-z])([A-Z])", "$1 $2");
}