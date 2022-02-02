using System.Text.RegularExpressions;

namespace FLRC.Leaderboards.Core.Results;

public record FormattedResultType : Formatted<ResultType>
{
	public FormattedResultType(ResultType value) : base(value)
	{
	}

	public override string Display => Regex.Replace(Enum.GetName(Value) ?? string.Empty, "([a-z])([A-Z])", "$1 $2");
}