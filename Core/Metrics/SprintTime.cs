namespace FLRC.Leaderboards.Core.Metrics;

public sealed record SprintTime(TimeSpan Value) : Time(Value)
{
	public override string Display => Value.ToString(Value.TotalMinutes >= 1 ? @"m\:ss\.ff" : @"s\.ff");
}