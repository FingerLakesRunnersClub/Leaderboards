namespace FLRC.Leaderboards.Core;

public abstract record Formatted<T>(T Value) : Formattable
{
	public abstract string Display { get; }
}