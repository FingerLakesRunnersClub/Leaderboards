using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Overall;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Results;
using FLRC.Leaderboards.Core.Teams;

namespace FLRC.Leaderboards.Core.Athletes;

public class AthleteSummary
{
	public Athlete Athlete { get; }
	public IDictionary<Course, Ranked<Time>> Fastest { get; }
	public IDictionary<Course, Ranked<Time>> Average { get; }
	public IDictionary<Course, Ranked<ushort>> Runs { get; }
	public Dictionary<Course, Ranked<Stars>> CommunityStars { get; }
	public Dictionary<Course, Result[]> All { get; }

	public IReadOnlyCollection<AthleteOverallRow> Competitions { get; }

	public Ranked<Points> OverallPoints { get; }
	public Ranked<AgeGrade> OverallAgeGrade { get; }
	public Ranked<Miles> OverallMiles { get; }
	public Ranked<Stars> OverallCommunityStars { get; }
	public TeamResults TeamResults { get; }

	public int TotalResults { get; }
	private readonly IReadOnlyCollection<Course> _results;
	private readonly IConfig _config;


	public AthleteSummary(Athlete athlete, IReadOnlyCollection<Course> results, IConfig config)
	{
		_results = results;
		_config = config;

		Athlete = athlete;
		Fastest = results.ToDictionary(c => c, c => c.Fastest(athlete.Category).FirstOrDefault(r => r.Result.Athlete.Equals(athlete)));
		Average = results.ToDictionary(c => c, c => c.BestAverage(athlete.Category).FirstOrDefault(r => r.Result.Athlete.Equals(athlete)));
		Runs = results.ToDictionary(c => c, c => c.MostRuns().FirstOrDefault(r => r.Result.Athlete.Equals(athlete)));
		CommunityStars = results.ToDictionary(c => c, c => c.CommunityStars().FirstOrDefault(r => r.Result.Athlete.Equals(athlete)));
		All = results.ToDictionary(c => c, c => c.Results.Where(r => r.Athlete.Equals(athlete)).ToArray());

		var overall = new OverallResults(results);
		Competitions = new[]
			{
				OverallRow("Points/F", Category.F, athlete, () => overall.MostPoints(athlete.Category).FirstOrDefault(r => r.Result.Athlete.Equals(athlete))),
				OverallRow("Points/M", Category.M, athlete, () => overall.MostPoints(athlete.Category).FirstOrDefault(r => r.Result.Athlete.Equals(athlete))),
				OverallRow("PointsTop3/F", Category.F, athlete, () => overall.MostPoints(3, athlete.Category).FirstOrDefault(r => r.Result.Athlete.Equals(athlete))),
				OverallRow("PointsTop3/M", Category.M, athlete, () => overall.MostPoints(3, athlete.Category).FirstOrDefault(r => r.Result.Athlete.Equals(athlete))),
				OverallRow("AgeGrade", null, athlete, () => overall.AgeGrade().FirstOrDefault(r => r.Result.Athlete.Equals(athlete))),
				OverallRow("Miles", null, athlete, () => overall.MostMiles().FirstOrDefault(r => r.Result.Athlete.Equals(athlete))),
				OverallRow("Community", null, athlete, () => overall.CommunityStars().FirstOrDefault(r => r.Result.Athlete.Equals(athlete))),
				OverallRow("Team", null, athlete, () => overall.TeamPoints().FirstOrDefault(r => r.Team == athlete.Team))
			}.Where(c => c?.Value != null)
			.ToArray();

		OverallPoints = overall.MostPoints(athlete.Category).FirstOrDefault(r => r.Result.Athlete.Equals(athlete));
		OverallAgeGrade = overall.AgeGrade().FirstOrDefault(r => r.Result.Athlete.Equals(athlete));
		OverallMiles = overall.MostMiles().FirstOrDefault(r => r.Result.Athlete.Equals(athlete));
		OverallCommunityStars = overall.CommunityStars().FirstOrDefault(r => r.Result.Athlete.Equals(athlete));
		TeamResults = overall.TeamPoints().FirstOrDefault(r => r.Team == athlete.Team);

		TotalResults = Fastest.Count(r => r.Value != null) + Average.Count(r => r.Value != null);
	}

	private AthleteOverallRow OverallRow<T>(string id, Category category, Athlete athlete, Func<Ranked<T>> results) where T : Formattable
	{
		if (!_config.Competitions.ContainsKey(id) || category != null && category != athlete.Category)
			return null;

		var result = results();
		return new AthleteOverallRow
		{
			ID = id,
			Name = _config.Competitions[id],
			Rank = result?.Rank,
			Value = result?.Value?.Display
		};
	}

	public IReadOnlyCollection<SimilarAthlete> SimilarAthletes
	{
		get
		{
			var fastMatches = _results.ToDictionary(c => c, c => c.Fastest().Where(r => Fastest[c] != null && !r.Result.Athlete.Equals(Athlete) && IsMatch(Fastest[c], r)));
			var avgMatches = _results.ToDictionary(c => c, c => c.BestAverage().Where(r => Average[c] != null && !r.Result.Athlete.Equals(Athlete) && IsMatch(Average[c], r)));

			var athletes = fastMatches.SelectMany(c => c.Value.Select(r => r.Result.Athlete))
				.Union(avgMatches.SelectMany(c => c.Value.Select(r => r.Result.Athlete)))
				.Distinct()
				.Select(a => new AthleteSummary(a, _results, _config));

			return athletes.Select(their => new SimilarAthlete(this, their))
				.Where(m => Math.Abs(m.FastestPercent.Value) < 10
				            && (m.AveragePercent == null || Math.Abs(m.AveragePercent.Value) < 10)
				            && m.Similarity.Value >= 80)
				.ToArray();
		}
	}

	private const byte percentThreshold = 5;

	private static bool IsMatch(Ranked<Time> mine, Ranked<Time> theirs)
		=> Time.AbsolutePercentDifference(mine.Value, theirs.Value) <= percentThreshold;
}