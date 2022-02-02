using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Results;
using FLRC.Leaderboards.Core.Teams;

namespace FLRC.Leaderboards.Core.Overall;

public class OverallResults
{
	private readonly IEnumerable<Course> _courses;

	public OverallResults(IEnumerable<Course> courses) => _courses = courses;

	public RankedList<Points> MostPoints(Category category = null)
		=> RankedList(_courses.SelectMany(c => c.Fastest(category)).GroupBy(r => r.Result.Athlete), g => new Points(g.Sum(r => r.Points.Value)), g => g.Sum(r => r.Points.Value), _ => 0);

	public RankedList<double> MostMiles(Category category = null)
		=> RankedList(_courses.SelectMany(c => c.MostMiles(category)).GroupBy(r => r.Result.Athlete), g => g.Sum(r => r.Value), g => g.Sum(r => r.Value), _ => 0);

	public RankedList<AgeGrade> AgeGrade(Category category = null)
		=> RankedList(_courses.SelectMany(c => c.Fastest(category)).GroupBy(r => r.Result.Athlete), g => new AgeGrade(g.Average(r => r.AgeGrade.Value)), g => g.Count(), g => (uint)g.Count());

	public RankedList<Date> Completed(Category category = null)
		=> RankedList(_courses.SelectMany(c => c.Earliest(category)).GroupBy(r => r.Result.Athlete).Where(a => a.Count() == _courses.Count()), g => g.Max(r => r.Value), g => Date.CompetitionStart.Subtract(g.Max(r => r.Value)?.Value ?? Date.CompetitionStart), g => (uint)g.Count());

	public RankedList<TeamMember> TeamMembers(byte ag)
		=> RankedList(_courses.SelectMany(c => c.Fastest(null, ag)).GroupBy(r => r.Result.Athlete), g => new TeamMember(g.ToList()), g => g.Count(), g => (uint)g.Count());

	public RankedList<TeamMember> GroupMembers(IEnumerable<Athlete> athletes)
		=> RankedList(_courses.SelectMany(c => c.Fastest().Where(r => athletes.Contains(r.Result.Athlete))).GroupBy(r => r.Result.Athlete), g => new TeamMember(g.ToList()), g => g.Count(), g => (uint)g.Count());

	public IEnumerable<TeamResults> TeamPoints()
		=> _courses.SelectMany(c => c.TeamPoints())
			.GroupBy(r => r.Team)
			.Select(g => new TeamResults
			{
				Team = g.Key,
				AgeGradePoints = (byte)(g.Sum(r => r.AgeGradePoints) + Athlete.Teams.Count * (_courses.Count() - g.Count())),
				MostRunsPoints = (byte)(g.Sum(r => r.MostRunsPoints) + Athlete.Teams.Count * (_courses.Count() - g.Count()))
			})
			.Rank();

	private static RankedList<T1> RankedList<T1, T2, T3>(IEnumerable<IGrouping<Athlete, Ranked<T2>>> results, Func<IGrouping<Athlete, Ranked<T2>>, T1> getValue, Func<IGrouping<Athlete, Ranked<T2>>, T3> sort, Func<IGrouping<Athlete, Ranked<T2>>, uint> count)
	{
		var ranks = new RankedList<T1>();
		var list = results.OrderByDescending(sort).ThenByDescending(getValue).ToList();
		for (ushort rank = 1; rank <= list.Count; rank++)
		{
			var result = list[rank - 1];
			var value = getValue(result);
			ranks.Add(new Ranked<T1>
			{
				Rank = ranks.Any() && ranks.Last().Value.Equals(value) ? ranks.Last().Rank : new Rank(rank),
				Result = new Result { Athlete = result.Key },
				Count = count(result),
				AgeGrade = new AgeGrade(result.Average(r => r.AgeGrade.Value)),
				Value = value
			});
		}

		return ranks;
	}
}