using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Overall;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Results;
using FLRC.Leaderboards.Core.Teams;

namespace FLRC.Leaderboards.Core.Reports;

public class AwardsViewModel : ViewModel
{
	public override string Title => "Awards";
	public IDictionary<Athlete, Award[]> Awards { get; }

	public AwardsViewModel(IConfig config, IReadOnlyCollection<Course> results)
	{
		Config = config;
		Awards = GetAwards(results);
	}

	private IDictionary<Athlete, Award[]> GetAwards(IReadOnlyCollection<Course> results)
	{
		var overall = new OverallResults(results);
		var awards = new List<Award>();

		awards.AddRange(Overall("Points/F", $"Overall Points ({Category.F.Display})", overall.MostPoints(Filter.F), 5));
		awards.AddRange(Overall("Points/M", $"Overall Points ({Category.M.Display})", overall.MostPoints(Filter.M), 5));
		awards.AddRange(Overall("Miles", "Overall Miles", overall.MostMiles(), 10));
		awards.AddRange(Overall("AgeGrade", "Overall Age Grade", overall.AgeGrade(), 10));
		awards.AddRange(Overall("Community", "Overall Community", overall.CommunityStars(), 10));
		awards.AddRange(Team(overall.TeamMembers(overall.TeamPoints().First().Value.Team)));
		awards.AddRange(Course("Fastest/F", $"Fastest ({Category.F.Display})", results.SelectMany(c => c.Fastest(Filter.F))));
		awards.AddRange(Course("Fastest/M", $"Fastest ({Category.M.Display})", results.SelectMany(c => c.Fastest(Filter.M))));
		awards.AddRange(Course("BestAverage/F", $"Best Average ({Category.F.Display})", results.SelectMany(c => c.BestAverage(Filter.F))));
		awards.AddRange(Course("BestAverage/M", $"Best Average ({Category.M.Display})", results.SelectMany(c => c.BestAverage(Filter.M))));
		awards.AddRange(Course("MostRuns", "Most Runs", results.SelectMany(c => c.MostRuns())));
		awards.AddRange(AgeGroup(results, Category.F));
		awards.AddRange(AgeGroup(results, Category.M));

		return awards.GroupBy(a => a.Athlete).ToDictionary(a => a.Key, a => a.ToArray());
	}

	private IReadOnlyCollection<Award> Overall<T>(string type, string title, RankedList<T> results, byte top)
		=> results.Where(r => r.Rank.Value <= top)
			.Select(r => new Award
			{
				Name = $"{r.Rank.Display} {title}",
				Link = $"/Overall/{type}",
				Value = r.Rank.Value == 1 ? Config.Awards["Overall"] : Config.Awards["Top"],
				Athlete = r.Result.Athlete
			})
			.ToArray();

	private IReadOnlyCollection<Award> Team(RankedList<TeamMember> members)
		=> members.Where(m => m.Rank.Value <= 10)
			.Select(r => new Award
			{
				Name = $"{r.Rank.Display} Top Team Member",
				Link = $"/Team/Members/{r.Result.Athlete.Team.Value}",
				Value = Config.Awards["Team"],
				Athlete = r.Result.Athlete
			})
			.ToArray();

	private IReadOnlyCollection<Award> Course<T>(string type, string title, IEnumerable<Ranked<T>> results)
		=> results.Where(r => r.Rank.Value == 1)
			.Select(r => new Award
			{
				Name = $"{r.Result.CourseName} {title}",
				Link = $"/Course/{r.Result.CourseID}/{r.Result.CourseDistance}/{type}",
				Value = Config.Awards["Course"],
				Athlete = r.Result.Athlete
			})
			.ToArray();

	private IReadOnlyCollection<Award> AgeGroup(IReadOnlyCollection<Course> results, Category category)
		=> Athlete.Teams.SelectMany(t => results.SelectMany(c => c.Fastest(new Filter(category, t.Value))
				.Where(r => r.Rank.Value == (!c.Fastest(new Filter(category, t.Value)).First().Result.Athlete.Equals(c.Fastest(new Filter(category)).First().Result.Athlete) ? 1 : 2))
				.Select(r => new Award
				{
					Name = $"{r.Result.CourseName} {t.Value.Display} ({category.Display})",
					Link = $"/Course/{r.Result.CourseID}/{r.Result.CourseDistance}/Fastest/{category.Display}?ag={t.Value.Value}",
					Value = Config.Awards["Age Group"],
					Athlete = r.Result.Athlete
				})))
			.ToArray();
}