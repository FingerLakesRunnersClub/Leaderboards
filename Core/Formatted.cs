namespace FLRC.Leaderboards.Core;

public abstract record Formatted<T>
{
	protected Formatted(T value) => Value = value;

	public T Value { get; }
	public abstract string Display { get; }
}