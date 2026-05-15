using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Web.ViewModels;
using Athlete = FLRC.Leaderboards.Model.Athlete;

namespace FLRC.Leaderboards.Web.Services;

public sealed class AwardsCalculator(IOverallResultsCalculator overall, IConfig config) : IAwardsCalculator
{
	public Dictionary<Athlete, Award[]> GetAwards(Iteration iteration)
	{
		var awards = new List<Award>();
		awards.AddRange(Overall("Points/F", $"Overall Points ({Category.F.Display})", overall.MostPoints(iteration, new Filter(Category.F)), 5));
		awards.AddRange(Overall("Points/M", $"Overall Points ({Category.M.Display})", overall.MostPoints(iteration, new Filter(Category.M)), 5));
		awards.AddRange(Overall("Miles", "Overall Miles", overall.MostMiles(iteration), 10));
		//awards.AddRange(Overall("Courses", "Most Courses", overall.MostCourses(iteration), 10));
		awards.AddRange(Overall("AgeGrade", "Overall Age Grade", overall.AgeGrade(iteration), 10));
		awards.AddRange(Overall("Community", "Overall Community", overall.Community(iteration), 10));
		awards.AddRange(Team(overall.TeamMembers(iteration, overall.TeamPoints(iteration)[0].Value.Team), iteration));

		var officialCourses = iteration.OfficialChallenge?.Courses.ToArray() ?? [];
		awards.AddRange(Course("Fastest/F", $"Fastest ({Category.F.Display})", officialCourses.SelectMany(c => c.Results.For(iteration).Fastest(new Filter(Category.F))).ToArray()));
		awards.AddRange(Course("Fastest/M", $"Fastest ({Category.M.Display})", officialCourses.SelectMany(c => c.Results.For(iteration).Fastest(new Filter(Category.M))).ToArray()));
		awards.AddRange(Course("BestAverage/F", $"Best Average ({Category.F.Display})", officialCourses.SelectMany(c => c.Results.For(iteration).BestAverage(new Filter(Category.F))).ToArray()));
		awards.AddRange(Course("BestAverage/M", $"Best Average ({Category.M.Display})", officialCourses.SelectMany(c => c.Results.For(iteration).BestAverage(new Filter(Category.M))).ToArray()));
		awards.AddRange(Course("MostRuns", "Most Runs", officialCourses.SelectMany(c => c.Results.For(iteration).MostRuns()).ToArray()));

		awards.AddRange(AgeGroup(iteration, officialCourses, Category.F));
		awards.AddRange(AgeGroup(iteration, officialCourses, Category.M));

		return awards.GroupBy(a => a.Athlete).ToDictionary(a => a.Key, a => a.ToArray());
	}

	private Award[] Overall<T>(string type, string title, RankedList<T, Result> results, byte top)
		=> results.Where(r => r.Rank.Value <= top)
			.Select(r => new Award
			{
				Name = $"{r.Rank.Display} {title}",
				Link = $"/Overall/{type}",
				Value = r.Rank.Value == 1 ? config.Awards["Overall"] : config.Awards["Top"],
				Athlete = r.Result.Athlete
			})
			.ToArray();

	private Award[] Team(RankedList<TeamMember, Result> members, Iteration iteration)
		=> members.Where(m => m.Rank.Value <= 10)
			.Select(r => new Award
			{
				Name = $"{r.Rank.Display} Top Team Member",
				Link = $"/Team/Members/{r.Result.Athlete.Team(iteration).Value}",
				Value = config.Awards["Team"],
				Athlete = r.Result.Athlete
			})
			.ToArray();

	private Award[] Course<T>(string type, string title, Ranked<T, Result>[] results)
		=> results.Where(r => r.Rank.Value == 1)
			.Select(r => new Award
			{
				Name = $"{r.Result.Course.Race.Name} {title}",
				Link = $"/Results/{type}/{r.Result.CourseID}",
				Value = config.Awards[config.CourseLabel],
				Athlete = r.Result.Athlete
			})
			.ToArray();

	private Award[] AgeGroup(Iteration iteration, Course[] courses, Category category)
		=> Core.Teams.Team.Teams
			.SelectMany(t => courses.SelectMany(course => CourseAgeGroupAwards(iteration, course, category, t.Value)))
			.ToArray();

	private Award[] CourseAgeGroupAwards(Iteration iteration, Course course, Category category, Core.Teams.Team team)
	{
		var categoryWinners = course.Results.For(iteration).Fastest(new Filter(category))
			.Where(r => r.Rank.Value == 1)
			.Select(r => r.Result.Athlete)
			.ToArray();

		var ageGroupResults = course.Results.For(iteration).Fastest(new Filter(category, team));
		var winningRank = ageGroupResults.Find(r => !categoryWinners.Contains(r.Result.Athlete));
		return ageGroupResults.Where(r => r.Rank == winningRank?.Rank && !categoryWinners.Contains(r.Result.Athlete))
			.Select(r => new Award
			{
				Name = $"{r.Result.Course.Race.Name} {team.Display} ({category.Display})",
				Value = config.Awards["Age Group"],
				Athlete = r.Result.Athlete
			}).ToArray();
	}
}