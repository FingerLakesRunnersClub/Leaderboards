using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Results;
using FLRC.Leaderboards.Core.Teams;

namespace FLRC.Leaderboards.Core.Overall;

public class OverallResults
{
	private readonly IReadOnlyCollection<Course> _courses;

	public OverallResults(IReadOnlyCollection<Course> courses) => _courses = courses;

	public RankedList<Points> MostPoints(Filter filter = null)
		=> RankedList(_courses.SelectMany(c => c.Fastest(filter)).GroupBy(r => r.Result.Athlete).Where(g => !g.Key.Private), g => !g.Key.Private ? new Points(g.Sum(r => r.Points?.Value ?? 0)) : null, g => g.Sum(r => r.Points?.Value));

	public RankedList<Points> MostPoints(byte limit, Filter filter = null)
		=> RankedList(_courses.SelectMany(c => c.Fastest(filter)).GroupBy(r => r.Result.Athlete).Where(g => !g.Key.Private), g => !g.Key.Private ? new Points(g.OrderByDescending(r => r.Points).Take(limit).Sum(r => r.Points?.Value ?? 0)) : null, g => g.OrderByDescending(r => r.Points).Take(limit).Sum(r => r.Points?.Value ?? 0), g => g.Where(r => r.Rank.Value == 1).Sum(r => r.All.Count > 1 ? r.All[1].BehindLeader.Value.TotalSeconds : 0));

	public RankedList<Miles> MostMiles(Filter filter = null)
		=> RankedList(_courses.SelectMany(c => c.MostMiles(filter)).GroupBy(r => r.Result.Athlete), g => new Miles(g.Sum(r => r.Value.Value)), g => new Points(g.Sum(r => r.Value.Value)));

	public RankedList<AgeGrade> AgeGrade(Filter filter = null)
		=> RankedList(_courses.SelectMany(c => c.Fastest(filter)).GroupBy(r => r.Result.Athlete).Where(g => !g.Key.Private), g => !g.Key.Private ? new AgeGrade(g.Average(r => r.AgeGrade.Value)) : null, g => g.Count(), g => (uint) g.Count());

	public RankedList<Stars> CommunityStars(Filter filter = null)
		=> RankedList(_courses.SelectMany(c => c.CommunityStars(filter).Where(p => p.Value.Value > 0)).GroupBy(r => r.Result.Athlete), g => new Stars((ushort) g.Sum(c => c.Value.Value)), g => g.Sum(c => c.Value.Value));

	public RankedList<Date> Completed(Filter filter = null)
		=> RankedList(_courses.SelectMany(c => c.Earliest(filter)).GroupBy(r => r.Result.Athlete).Where(a => a.Count() == _courses.Count), g => g.Max(r => r.Value), g => Date.CompetitionStart.Subtract(g.Max(r => r.Value)?.Value ?? Date.CompetitionStart), g => (uint) g.Count());

	public RankedList<TeamMember> TeamMembers(Team team, Filter filter = null)
		=> RankTeam(_courses.SelectMany(c => c.Fastest(filter).Where(r => r.Result.Athlete.Team == team)).ToArray());

	private static RankedList<TeamMember> RankTeam(IReadOnlyCollection<Ranked<Time>> results)
	{
		var ranked = results
			.GroupBy(r => r.Result.Athlete)
			.Select(g => new TeamMember(g.ToArray()) { Athlete = g.Key })
			.OrderByDescending(m => m.Score)
			.ToArray();

		var ranks = new RankedList<TeamMember>();
		for (byte rank = 1; rank <= ranked.Length; rank++)
		{
			var value = ranked[rank - 1];
			ranks.Add(new Ranked<TeamMember>
			{
				All = ranks,
				Rank = ranks.Any() && ranks.Last().Value.Score.Equals(value.Score) ? ranks.Last().Rank : new Rank(rank),
				Result = new Result { Athlete = value.Athlete },
				Count = value.Courses,
				AgeGrade = value.AgeGrade,
				Value = value
			});
		}

		return ranks;
	}

	public RankedList<TeamMember> GroupMembers(IReadOnlyCollection<Athlete> athletes)
		=> RankTeam(_courses.SelectMany(c => c.Fastest().Where(r => athletes.Contains(r.Result.Athlete))).ToArray());

	public RankedList<TeamResults> TeamPoints(Filter filter = null)
		=> _courses.SelectMany(c => c.TeamPoints(filter))
			.GroupBy(r => r.Value.Team)
			.Select(g => new TeamResults
			{
				Team = g.Key,
				AgeGradePoints = (byte) (g.Sum(r => r.Value.AgeGradePoints) + Athlete.Teams.Count * (_courses.Count - g.Count())),
				MostRunsPoints = (byte) (g.Sum(r => r.Value.MostRunsPoints) + Athlete.Teams.Count * (_courses.Count - g.Count()))
			})
			.Rank();

	private static RankedList<T1> RankedList<T1, T2, T3>(IEnumerable<IGrouping<Athlete, Ranked<T2>>> results, Func<IGrouping<Athlete, Ranked<T2>>, T1> getValue, Func<IGrouping<Athlete, Ranked<T2>>, T3> sort)
		=> RankedList(results, getValue, sort, getValue, _ => 0);

	private static RankedList<T1> RankedList<T1, T2, T3>(IEnumerable<IGrouping<Athlete, Ranked<T2>>> results, Func<IGrouping<Athlete, Ranked<T2>>, T1> getValue, Func<IGrouping<Athlete, Ranked<T2>>, T3> sort, Func<IGrouping<Athlete, Ranked<T2>>, uint> count)
		=> RankedList(results, getValue, sort, getValue, count);

	private static RankedList<T1> RankedList<T1, T2, T3, T4>(IEnumerable<IGrouping<Athlete, Ranked<T2>>> results, Func<IGrouping<Athlete, Ranked<T2>>, T1> getValue, Func<IGrouping<Athlete, Ranked<T2>>, T3> sort, Func<IGrouping<Athlete, Ranked<T2>>, T4> tiebreaker)
		=> RankedList(results, getValue, sort, tiebreaker, _ => 0);

	private static RankedList<T1> RankedList<T1, T2, T3, T4>(IEnumerable<IGrouping<Athlete, Ranked<T2>>> results, Func<IGrouping<Athlete, Ranked<T2>>, T1> getValue, Func<IGrouping<Athlete, Ranked<T2>>, T3> sort, Func<IGrouping<Athlete, Ranked<T2>>, T4> tiebreaker, Func<IGrouping<Athlete, Ranked<T2>>, uint> count)
	{
		var ranks = new RankedList<T1>();
		var list = results.OrderByDescending(sort).ThenByDescending(tiebreaker).ToArray();
		for (ushort rank = 1; rank <= list.Length; rank++)
		{
			var result = list[rank - 1];
			var value = getValue(result);
			var notInFirstPlace = ranks.Any();
			var lastPlace = notInFirstPlace
				? ranks.Last()
				: null;

			ranks.Add(new Ranked<T1>
			{
				All = ranks,
				Rank = notInFirstPlace && lastPlace.Value.Equals(value) ? lastPlace.Rank : new Rank(rank),
				Result = new Result { Athlete = result.Key },
				Count = count(result),
				AgeGrade = !result.Key.Private
					? new AgeGrade(result.Average(r => r.AgeGrade.Value))
					: null,
				Value = value
			});
		}

		return ranks;
	}
}