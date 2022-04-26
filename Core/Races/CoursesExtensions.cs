namespace FLRC.Leaderboards.Core.Races;

public static class CoursesExtensions
{
	public static IReadOnlyCollection<DateOnly> DistinctMonths(this IEnumerable<Course> courses)
		=> courses.SelectMany(c => c.DistinctMonths())
			.Distinct()
			.OrderBy(d => d)
			.ToArray();
}