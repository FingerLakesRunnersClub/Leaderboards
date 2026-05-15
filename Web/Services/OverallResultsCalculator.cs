using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Teams;
using FLRC.Leaderboards.Model;
using Course = FLRC.Leaderboards.Model.Course;
using TeamMember = FLRC.Leaderboards.Web.ViewModels.TeamMember;

namespace FLRC.Leaderboards.Web.Services;

public sealed class OverallResultsCalculator(ICommunityStarCalculator starCalculator) : IOverallResultsCalculator
{
	public RankedList<Points, Result> MostPoints(Iteration iteration, Filter filter = null)
		=> RankedList(iteration, OfficialCourses(iteration).SelectMany(c => c.Results.For(iteration).Fastest(filter)).GroupBy(r => r.Result.Athlete).Where(g => !g.Key.IsPrivate), g => !g.Key.IsPrivate ? new Points(g.Sum(r => r.Points?.Value ?? 0)) : null, g => g.Sum(r => r.Points?.Value));

	public RankedList<Points, Result> MostPoints(Iteration iteration, byte limit, Filter filter = null)
		=> RankedList(iteration, OfficialCourses(iteration).SelectMany(c => c.Results.For(iteration).Fastest(filter)).GroupBy(r => r.Result.Athlete).Where(g => !g.Key.IsPrivate), g => !g.Key.IsPrivate ? new Points(g.OrderByDescending(r => r.Points).Take(limit).Sum(r => r.Points?.Value ?? 0)) : null, g => g.OrderByDescending(r => r.Points).Take(limit).Sum(r => r.Points?.Value ?? 0), g => g.Where(r => r.Rank.Value == 1).Sum(r => r.All.Count > 1 ? r.All[1].BehindLeader.Value.TotalSeconds : 0), g => uint.Min((uint)g.Count(), limit));

	public RankedList<AgeGrade, Result> AgeGrade(Iteration iteration, Filter filter = null)
		=> RankedList(iteration, OfficialCourses(iteration).SelectMany(c => c.Results.For(iteration).Fastest(filter)).GroupBy(r => r.Result.Athlete).Where(g => !g.Key.IsPrivate), g => !g.Key.IsPrivate ? new AgeGrade(g.AvgAgeGrade()) : null, g => g.Count(), g => (uint)g.Count());

	public RankedList<Date, Result> Completed(Iteration iteration, Filter filter = null)
		=> RankedList(iteration, OfficialCourses(iteration).SelectMany(c => c.Results.For(iteration).Earliest(filter)).GroupBy(r => r.Result.Athlete).Where(a => a.Key.Challenge(iteration) == iteration.OfficialChallenge && a.Count() == OfficialCourses(iteration).Length), g => g.Max(r => r.Value), g => Start(iteration).Subtract(g.Max(r => r.Value)?.Value ?? Start(iteration)), g => (uint)g.Count());

	public RankedList<Date, Result> CompletedPersonal(Iteration iteration, Filter filter = null)
		=> RankedList(iteration, AllCourses(iteration).SelectMany(c => c.Results.For(iteration).Earliest(filter)).GroupBy(r => r.Result.Athlete).Where(a => a.Key.Challenge(iteration) != iteration.OfficialChallenge && (a.Key.Challenge(iteration)?.Courses.All(c => a.Any(r => r.Result.Course == c)) ?? false)), g => g.Max(r => r.Value), g => Start(iteration).Subtract(g.Max(r => r.Value)?.Value ?? Start(iteration)), g => (uint)g.Count());

	public RankedList<Miles, Result> MostMiles(Iteration iteration, Filter filter = null)
		=> RankedList(iteration, AllCourses(iteration).SelectMany(c => c.Results.For(iteration).MostMiles(filter)).GroupBy(r => r.Result.Athlete), g => new Miles(g.Sum(r => r.Value.Value)), g => new Points(g.Sum(r => r.Value.Value)), g => (uint)g.Sum(r => r.Count));

	public RankedList<int, Result> MostCourses(Iteration iteration, Filter filter = null)
		=> RankedList(iteration, AllCourses(iteration).SelectMany(c => c.Results.For(iteration).Fastest(filter)).GroupBy(r => r.Result.Athlete), g => g.Count(), g => g.Count(), g => (uint)g.Count());

	public RankedList<Stars, Result> Community(Iteration iteration, Filter filter = null)
		=> RankedList(iteration, AllCourses(iteration).SelectMany(c => c.Results.For(iteration).CommunityStars(starCalculator, filter)).GroupBy(g => g.Result.Athlete), g => new Stars((ushort)g.Sum(r => r.Value.Value)), g => g.Sum(s => s.Value.Value), g => (uint)g.Count());

	private static RankedList<T1, Result> RankedList<T1, T2, T3>(Iteration iteration, IEnumerable<IGrouping<Athlete, Ranked<T2, Result>>> results, Func<IGrouping<Athlete, Ranked<T2, Result>>, T1> getValue, Func<IGrouping<Athlete, Ranked<T2, Result>>, T3> sort)
		=> RankedList(iteration, results, getValue, sort, getValue, g => (uint)g.Count());

	private static RankedList<T1, Result> RankedList<T1, T2, T3>(Iteration iteration, IEnumerable<IGrouping<Athlete, Ranked<T2, Result>>> results, Func<IGrouping<Athlete, Ranked<T2, Result>>, T1> getValue, Func<IGrouping<Athlete, Ranked<T2, Result>>, T3> sort, Func<IGrouping<Athlete, Ranked<T2, Result>>, uint> count)
		=> RankedList(iteration, results, getValue, sort, getValue, count);

	private static RankedList<T1, Result> RankedList<T1, T2, T3, T4>(Iteration iteration, IEnumerable<IGrouping<Athlete, Ranked<T2, Result>>> results, Func<IGrouping<Athlete, Ranked<T2, Result>>, T1> getValue, Func<IGrouping<Athlete, Ranked<T2, Result>>, T3> sort, Func<IGrouping<Athlete, Ranked<T2, Result>>, T4> tiebreaker, Func<IGrouping<Athlete, Ranked<T2, Result>>, uint> count)
	{
		var ranks = new RankedList<T1, Result>();
		var list = results.OrderByDescending(sort).ThenByDescending(tiebreaker).ToArray();
		for (ushort rank = 1; rank <= list.Length; rank++)
		{
			var result = list[rank - 1];
			var value = getValue(result);
			var notInFirstPlace = ranks.Any();
			var lastPlace = notInFirstPlace
				? ranks[^1]
				: null;

			var avgAgeGrade = result.AvgAgeGrade();
			var overallResult = new Result
			{
				Athlete = result.Key,
				StartTime = iteration.StartDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue
			};
			ranks.Add(new Ranked<T1, Result>
			{
				All = ranks,
				Rank = notInFirstPlace && lastPlace.Value.Equals(value) ? lastPlace.Rank : new Rank(rank),
				Result = overallResult,
				Count = count(result),
				AgeGrade = !result.Key.IsPrivate
					? new AgeGrade(avgAgeGrade)
					: null,
				Value = value
			});
		}

		return ranks;
	}

	public RankedList<TeamResults, Result> TeamPoints(Iteration iteration, Filter filter = null)
	{
		var officialCourses = OfficialCourses(iteration);
		return officialCourses.SelectMany(c => c.Results.For(iteration).TeamPoints(iteration, filter))
			.GroupBy(r => r.Value.Team)
			.Select(g => new TeamResults
			{
				Team = g.Key,
				AgeGradePoints = (byte)(g.Sum(r => r.Value.AgeGradePoints) + Team.Teams.Count * (officialCourses.Length - g.Count())),
				MostRunsPoints = (byte)(g.Sum(r => r.Value.MostRunsPoints) + Team.Teams.Count * (officialCourses.Length - g.Count()))
			})
			.Rank();
	}

	public RankedList<TeamMember, Result> TeamMembers(Iteration iteration, Team team, Filter filter = null)
	{
		var results = OfficialCourses(iteration).SelectMany(c => c.Results.For(iteration).Fastest(filter).Where(r => r.Result.Athlete.Team(iteration) == team)).ToArray();
		var ranked = results
			.GroupBy(r => r.Result.Athlete)
			.Select(g => new TeamMember(g.ToArray()) { Athlete = g.Key })
			.OrderByDescending(m => m.Score)
			.ToArray();

		var ranks = new RankedList<TeamMember, Result>();
		for (byte rank = 1; rank <= ranked.Length; rank++)
		{
			var value = ranked[rank - 1];
			ranks.Add(new Ranked<TeamMember, Result>
			{
				All = ranks,
				Rank = ranks.Any() && ranks[^1].Value.Score.Equals(value.Score) ? ranks[^1].Rank : new Rank(rank),
				Result = new Result { Athlete = value.Athlete, StartTime = iteration.StartDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.Now },
				Count = value.Courses,
				AgeGrade = value.AgeGrade,
				Value = value
			});
		}

		return ranks;
	}

	private static Course[] OfficialCourses(Iteration iteration)
		=> iteration.OfficialChallenge?.Courses.ToArray() ?? [];

	private static Course[] AllCourses(Iteration iteration)
		=> iteration.Races.SelectMany(r => r.Courses).ToArray();

	private static DateTime Start(Iteration iteration)
	=> iteration.StartDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue;
}