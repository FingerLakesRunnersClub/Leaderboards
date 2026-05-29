using FLRC.Leaderboards.Core.Results;
using FLRC.Leaderboards.Model;
using Result = FLRC.Leaderboards.Model.Result;

namespace FLRC.Leaderboards.Web;

public sealed record GroupedModelResult(IGrouping<Athlete, Result> Group) : Grouped<Athlete, Result>(Group)
{
	public Result Average(Course course, ushort? threshold = null)
	{
		var timedResults = Group
			.Where(r => r.Duration > TimeSpan.Zero)
			.OrderBy(r => r.StartTime)
			.ToArray();
		var average = timedResults.Length > 0
			? timedResults.OrderBy(r => r.Duration)
				.Take(threshold ?? timedResults.Length)
				.Average(r => r.Duration.TotalSeconds)
			: 0;

		return new Result
		{
			Athlete = Key,
			Course = course,
			StartTime = timedResults.LastOrDefault()?.StartTime ?? DateTime.MinValue,
			Duration = !Key.IsPrivate && timedResults.Length > 0
				? TimeSpan.FromSeconds(average)
				: TimeSpan.Zero
		};
	}

	public override int CompareTo(Grouped<Athlete, Result> other)
		=> Group.Key.ID.CompareTo(other.Key.ID);
}