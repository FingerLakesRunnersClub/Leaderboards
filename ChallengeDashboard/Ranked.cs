namespace FLRC.ChallengeDashboard;

public class Ranked<T>
{
	public Rank Rank { get; init; }
	public Result Result { get; init; }
	public T Value { get; init; }
	public uint Count { get; init; }

	public AgeGrade AgeGrade { get; init; }

	public Time BehindLeader { get; init; }

	public Points Points { get; init; }
}