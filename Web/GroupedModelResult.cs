using FLRC.Leaderboards.Core.Results;
using FLRC.Leaderboards.Model;
using Result = FLRC.Leaderboards.Model.Result;

namespace FLRC.Leaderboards.Web;

public sealed class GroupedModelResult(IGrouping<Model.Athlete, Model.Result> group)
	: Grouped<Model.Athlete, Model.Result>(group)
{
	public Model.Result Average(Model.Course course, ushort? threshold = null)
	{
		var timedResults = group
			.Where(r => r.Duration > TimeSpan.Zero)
			.OrderBy(r => r.StartTime)
			.ToArray();
		var average = timedResults.Length > 0
			? timedResults.OrderBy(r => r.Duration)
				.Take(threshold ?? timedResults.Length)
				.Average(r => r.Duration.TotalSeconds)
			: 0;

		return new Model.Result
		{
			Athlete = Key,
			Course = course,
			StartTime = timedResults.LastOrDefault()?.StartTime ?? DateTime.MinValue,
			Duration = !Key.IsPrivate && timedResults.Length > 0
				? TimeSpan.FromSeconds(average)
				: TimeSpan.Zero
		};
	}

	public override int CompareTo(Grouped<Model.Athlete, Model.Result> other)
		=> group.Key.ID.CompareTo(other.Key.ID);
}