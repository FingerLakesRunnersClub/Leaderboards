using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Overall;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Ranking;
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

		awards.AddRange(Overall($"Overall Points ({Category.M.Display})", overall.MostPoints(Category.M), 5));
		awards.AddRange(Overall($"Overall Points ({Category.F.Display})", overall.MostPoints(Category.F), 5));
		awards.AddRange(Overall("Overall Miles", overall.MostMiles(), 10));
		awards.AddRange(Overall("Overall Age Grade", overall.AgeGrade(), 10));
		awards.AddRange(Overall("Overall Community", overall.CommunityStars(), 10));
		awards.AddRange(Team(overall.TeamMembers(overall.TeamPoints().First().Value.Team.Value)));
		awards.AddRange(Course($"Fastest ({Category.M.Display})", results.SelectMany(c => c.Fastest(Category.M))));
		awards.AddRange(Course($"Fastest ({Category.F.Display})", results.SelectMany(c => c.Fastest(Category.F))));
		awards.AddRange(Course($"Best Average ({Category.M.Display})", results.SelectMany(c => c.BestAverage(Category.M))));
		awards.AddRange(Course($"Best Average ({Category.F.Display})", results.SelectMany(c => c.BestAverage(Category.F))));
		awards.AddRange(Course("Most Runs", results.SelectMany(c => c.MostRuns())));
		awards.AddRange(AgeGroup(results, Category.M));
		awards.AddRange(AgeGroup(results, Category.F));

		return awards.GroupBy(a => a.Athlete).ToDictionary(a => a.Key, a => a.ToArray());
	}

	private IReadOnlyCollection<Award> Overall<T>(string title, RankedList<T> results, byte top)
		=> results.Where(r => r.Rank.Value <= top)
			.Select(r => new Award
			{
				Name = $"{r.Rank.Display} {title}",
				Value = r.Rank.Value == 1 ? Config.Awards["Overall"] : Config.Awards["Top"],
				Athlete = r.Result.Athlete
			})
			.ToArray();

	private IReadOnlyCollection<Award> Team(RankedList<TeamMember> members)
		=> members.Where(m => m.Rank.Value <= 10)
			.Select(r => new Award
			{
				Name = $"{r.Rank.Display} Top Team Member",
				Value = Config.Awards["Team"],
				Athlete = r.Result.Athlete
			})
			.ToArray();

	private IReadOnlyCollection<Award> Course<T>(string title, IEnumerable<Ranked<T>> results)
		=> results.Where(r => r.Rank.Value == 1)
			.Select(r => new Award
			{
				Name = $"{r.Result.CourseName} {title}",
				Value = Config.Awards["Course"],
				Athlete = r.Result.Athlete
			})
			.ToArray();

	private IReadOnlyCollection<Award> AgeGroup(IReadOnlyCollection<Course> results, Category category)
		=> Athlete.Teams.SelectMany(t => results.SelectMany(c => c.Fastest(category, t.Key)
				.Where(r => r.Rank.Value == (!c.Fastest(category, t.Key).First().Result.Athlete.Equals(c.Fastest(category).First().Result.Athlete) ? 1 : 2))
				.Select(r => new Award
				{
					Name = $"{r.Result.CourseName} {t.Value.Display} ({category.Display})",
					Value = Config.Awards["Age Group"],
					Athlete = r.Result.Athlete
				})))
			.ToArray();
}