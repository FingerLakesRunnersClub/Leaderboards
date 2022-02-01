using System;
using System.Collections.Generic;
using System.Linq;

namespace FLRC.Leaderboards.Core;

public class LeaderboardViewModel : ViewModel
{
	public override string Title => "Leaderboard";

	public LeaderboardResultType LeaderboardResultType { get; }

	private readonly IEnumerable<Course> _courses;
	private readonly Func<LeaderboardTable, bool> _filter;

	public LeaderboardViewModel(IEnumerable<Course> courses, LeaderboardResultType type)
	{
		var courseList = courses.ToList();
		_courses = courseList;
		_filter = GetFilter(type);
		CourseNames = courseList.ToDictionary(c => c.ID, c => c.Name);
		LeaderboardResultType = type;
	}

	public IEnumerable<LeaderboardTable> OverallResults
	{
		get
		{
			var vm = new OverallResults(_courses);
			return new List<LeaderboardTable>
				{
					new()
					{
						Title = "Most Points (F)",
						Link = "/Overall/Points/F",
						ResultType = new FormattedResultType(ResultType.Fastest),
						Category = Category.F,
						Rows = vm.MostPoints(Category.F).Take(3)
							.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
					},
					new()
					{
						Title = "Most Points (M)",
						Link = "/Overall/Points/M",
						ResultType = new FormattedResultType(ResultType.Fastest),
						Category = Category.M,
						Rows = vm.MostPoints(Category.M).Take(3)
							.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display})
					},
					new()
					{
						Title = "Age Grade",
						Link = "/Overall/AgeGrade",
						ResultType = new FormattedResultType(ResultType.BestAverage),
						Rows = vm.AgeGrade().Take(3)
							.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.AgeGrade.Display })
					},
					new()
					{
						Title = "Most Miles",
						Link = "/Overall/Miles",
						ResultType = new FormattedResultType(ResultType.MostRuns),
						Rows = vm.MostMiles().Take(3)
							.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.ToString("F1")})
					},
					new()
					{
						Title = "Top Teams",
						Link = "/Overall/Team",
						ResultType = new FormattedResultType(ResultType.Team),
						Rows = vm.TeamPoints().Take(3)
							.Select(t => new LeaderboardRow { Rank = t.Rank, Name = t.Team.Display, Link = $"/Team/Index/{t.Team.Value}", Value = t.TotalPoints.ToString() })
					}
				};
		}
	}

	public IDictionary<Course, IEnumerable<LeaderboardTable>> CourseResults
		=> _courses.ToDictionary(c => c, c => LeaderboardTables(c).Where(_filter));

	private static IEnumerable<LeaderboardTable> LeaderboardTables(Course course)
		=> new List<LeaderboardTable>
		{
				new()
				{
					Title = "Fastest (F)",
					Course = course,
					ResultType = new FormattedResultType(ResultType.Fastest),
					Category = Category.F,
					Link = $"/Course/{course.ID}/{ResultType.Fastest}/F",
					Rows = course.Fastest(Category.F).Take(3)
						.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display})
				},
				new()
				{
					Title = "Fastest (M)",
					Course = course,
					ResultType = new FormattedResultType(ResultType.Fastest),
					Category = Category.M,
					Link = $"/Course/{course.ID}/{ResultType.Fastest}/M",
					Rows = course.Fastest(Category.M).Take(3)
						.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display})
				},
				new()
				{
					Title = "Best Average (F)",
					Course = course,
					ResultType = new FormattedResultType(ResultType.BestAverage),
					Category = Category.F,
					Link = $"/Course/{course.ID}/{ResultType.BestAverage}/F",
					Rows = course.BestAverage(Category.F).Take(3)
						.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display})
				},
				new()
				{
					Title = "Best Average (M)",
					Course = course,
					ResultType = new FormattedResultType(ResultType.BestAverage),
					Category = Category.M,
					Link = $"/Course/{course.ID}/{ResultType.BestAverage}/M",
					Rows = course.BestAverage(Category.M).Take(3)
						.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display})
				},
				new()
				{
					Title = "Most Runs",
					Course = course,
					ResultType = new FormattedResultType(ResultType.MostRuns),
					Link = $"/Course/{course.ID}/{ResultType.MostRuns}",
					Rows = course.MostRuns().Take(3)
						.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.ToString()})
				},
				new()
				{
					Title = "Age Grade",
					Course = course,
					ResultType = new FormattedResultType(ResultType.Team),
					Link = $"/Course/{course.ID}/{ResultType.Team}",
					Rows = course.TeamPoints().OrderByDescending(p => p.AverageAgeGrade).Take(3)
						.Select(r => new LeaderboardRow { Rank = new Rank(r.AgeGradePoints), Name = r.Team.Display, Link = $"/Team/Index/{r.Team.Value}", Value = r.AverageAgeGrade.Display })
				},
				new()
				{
					Title = "Most Runs",
					Course = course,
					ResultType = new FormattedResultType(ResultType.Team),
					Link = $"/Course/{course.ID}/{ResultType.Team}",
					Rows = course.TeamPoints().OrderByDescending(p => p.TotalRuns).Take(3)
						.Select(r => new LeaderboardRow { Rank = new Rank(r.MostRunsPoints), Name = r.Team.Display, Link = $"/Team/Index/{r.Team.Value}", Value = r.TotalRuns.ToString() })
				},
				new()
				{
					Title = "Total Points",
					Course = course,
					ResultType = new FormattedResultType(ResultType.Team),
					Link = $"/Course/{course.ID}/{ResultType.Team}",
					Rows = course.TeamPoints().Take(3)
						.Select(r => new LeaderboardRow { Rank = r.Rank, Name = r.Team.Display, Link = $"/Team/Index/{r.Team.Value}", Value = r.TotalPoints.ToString() })
				}
		};

	private static Func<LeaderboardTable, bool> GetFilter(LeaderboardResultType type)
	{
		return type switch
		{
			LeaderboardResultType.F => t => t.Category == Category.F || t.Category == null && t.ResultType?.Value != ResultType.Team,
			LeaderboardResultType.M => t => t.Category == Category.M || t.Category == null && t.ResultType?.Value != ResultType.Team,
			_ => t => t.ResultType?.Value == ResultType.Team
		};
	}
}