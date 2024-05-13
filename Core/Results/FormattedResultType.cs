namespace FLRC.Leaderboards.Core.Results;

public sealed record FormattedResultType(ResultType Value) : Formatted<ResultType>(Value)
{
	public override string Display
		=> Patterns.CamelCase().Replace(Enum.GetName(Value) ?? string.Empty, "$1 $2");
}