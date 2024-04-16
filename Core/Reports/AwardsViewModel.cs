using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Overall;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Results;
using FLRC.Leaderboards.Core.Teams;

namespace FLRC.Leaderboards.Core.Reports;

public sealed class AwardsViewModel : ViewModel
{
	public override string Title => "Awards";
	public IDictionary<Athlete, Award[]> Awards { get; }

	public AwardsViewModel(IConfig config, Course[] results)
	{
		Config = config;
		Awards = GetAwards(results);
	}

	private Dictionary<Athlete, Award[]> GetAwards(Course[] results)
	{
		var overall = new OverallResults(results);
		var awards = new List<Award>();

		awards.AddRange(Overall("Points/F", $"Overall Points ({Category.F.Display})", overall.MostPoints(Filter.F), 5));
		awards.AddRange(Overall("Points/M", $"Overall Points ({Category.M.Display})", overall.MostPoints(Filter.M), 5));
		awards.AddRange(Overall("Miles", "Overall Miles", overall.MostMiles(), 10));
		awards.AddRange(Overall("AgeGrade", "Overall Age Grade", overall.AgeGrade(), 10));
		awards.AddRange(Overall("Community", "Overall Community", overall.CommunityStars(), 10));
		awards.AddRange(Team(overall.TeamMembers(overall.TeamPoints()[0].Value.Team)));
		awards.AddRange(Course("Fastest/F", $"Fastest ({Category.F.Display})", results.SelectMany(c => c.Fastest(Filter.F)).ToArray()));
		awards.AddRange(Course("Fastest/M", $"Fastest ({Category.M.Display})", results.SelectMany(c => c.Fastest(Filter.M)).ToArray()));
		awards.AddRange(Course("BestAverage/F", $"Best Average ({Category.F.Display})", results.SelectMany(c => c.BestAverage(Filter.F)).ToArray()));
		awards.AddRange(Course("BestAverage/M", $"Best Average ({Category.M.Display})", results.SelectMany(c => c.BestAverage(Filter.M)).ToArray()));
		awards.AddRange(Course("MostRuns", "Most Runs", results.SelectMany(c => c.MostRuns()).ToArray()));
		awards.AddRange(AgeGroup(results, Category.F));
		awards.AddRange(AgeGroup(results, Category.M));

		return awards.GroupBy(a => a.Athlete).ToDictionary(a => a.Key, a => a.ToArray());
	}

	private Award[] Overall<T>(string type, string title, RankedList<T> results, byte top)
		=> results.Where(r => r.Rank.Value <= top)
			.Select(r => new Award
			{
				Name = $"{r.Rank.Display} {title}",
				Link = $"/Overall/{type}",
				Value = r.Rank.Value == 1 ? Config.Awards["Overall"] : Config.Awards["Top"],
				Athlete = r.Result.Athlete
			})
			.ToArray();

	private Award[] Team(RankedList<TeamMember> members)
		=> members.Where(m => m.Rank.Value <= 10)
			.Select(r => new Award
			{
				Name = $"{r.Rank.Display} Top Team Member",
				Link = $"/Team/Members/{r.Result.Athlete.Team.Value}",
				Value = Config.Awards["Team"],
				Athlete = r.Result.Athlete
			})
			.ToArray();

	private Award[] Course<T>(string type, string title, Ranked<T>[] results)
		=> results.Where(r => r.Rank.Value == 1)
			.Select(r => new Award
			{
				Name = $"{r.Result.CourseName} {title}",
				Link = $"/Course/{r.Result.CourseID}/{r.Result.CourseDistance}/{type}",
				Value = Config.Awards["Course"],
				Athlete = r.Result.Athlete
			})
			.ToArray();

	private Award[] AgeGroup(Course[] results, Category category)
		=> Athlete.Teams
			.SelectMany(t => results.SelectMany(c => CourseAgeGroupAwards(c, category, t.Value)))
			.ToArray();

	private Award[] CourseAgeGroupAwards(Course course, Category category, Team team)
	{
		var categoryWinners = course.Fastest(new Filter(category))
			.Where(r => r.Rank.Value == 1)
			.Select(r => r.Result.Athlete)
			.ToArray();

		var ageGroupResults = course.Fastest(new Filter(category, team));
		var winningRank = ageGroupResults.Find(r => !categoryWinners.Contains(r.Result.Athlete));
		return ageGroupResults.Where(r => r.Rank == winningRank?.Rank && !categoryWinners.Contains(r.Result.Athlete))
			.Select(r => new Award
			{
				Name = $"{r.Result.CourseName} {team.Display} ({category.Display})",
				Link = $"/Course/{r.Result.CourseID}/{r.Result.CourseDistance}/Fastest/{category.Display}?ag={team.Value}",
				Value = Config.Awards["Age Group"],
				Athlete = r.Result.Athlete
			}).ToArray();
	}
}