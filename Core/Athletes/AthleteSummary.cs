using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Overall;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Results;
using FLRC.Leaderboards.Core.Teams;

namespace FLRC.Leaderboards.Core.Athletes;

public sealed class AthleteSummary
{
	public Athlete Athlete { get; }
	public IDictionary<Course, Ranked<Time>> Fastest { get; }
	public IDictionary<Course, Ranked<Performance>> Farthest { get; }
	public IDictionary<Course, Ranked<Time>> Average { get; }
	public IDictionary<Course, Ranked<ushort>> Runs { get; }
	public Dictionary<Course, Ranked<Stars>> CommunityStars { get; }
	public Dictionary<Course, Result[]> All { get; }

	public AthleteOverallRow[] Competitions { get; }

	public Ranked<Points> OverallPoints { get; }
	public Ranked<AgeGrade> OverallAgeGrade { get; }
	public Ranked<Miles> OverallMiles { get; }
	public Ranked<Stars> OverallCommunityStars { get; }
	public Ranked<TeamResults> TeamResults { get; }

	public int TotalResults { get; }

	private readonly Course[] _results;
	private readonly IConfig _config;


	public AthleteSummary(Athlete athlete, Course[] results, IConfig config)
	{
		_results = results;
		_config = config;

		Athlete = athlete;
		var filter = new Filter(athlete.Category);
		Fastest = results.ToDictionary(c => c, c => c.Fastest(filter).Find(r => r.Result.Athlete.Equals(athlete)));
		Farthest = results.ToDictionary(c => c, c => c.Farthest(filter).Find(r => r.Result.Athlete.Equals(athlete)));
		Average = results.ToDictionary(c => c, c => c.BestAverage(filter).Find(r => r.Result.Athlete.Equals(athlete)));
		Runs = results.ToDictionary(c => c, c => c.MostRuns().Find(r => r.Result.Athlete.Equals(athlete)));
		CommunityStars = results.ToDictionary(c => c, c => c.CommunityStars().Find(r => r.Result.Athlete.Equals(athlete)));
		All = results.ToDictionary(c => c, c => c.Results.Where(r => r.Athlete.Equals(athlete)).ToArray());

		if (_config.FileSystemResults is not null)
			return;

		var overall = new OverallResults(results);
		Competitions = new[]
			{
				OverallRow("Points/F", Category.F, athlete, () => overall.MostPoints(filter).Find(r => r.Result.Athlete.Equals(athlete))),
				OverallRow("Points/M", Category.M, athlete, () => overall.MostPoints(filter).Find(r => r.Result.Athlete.Equals(athlete))),
				OverallRow("PointsTop3/F", Category.F, athlete, () => overall.MostPoints(3, filter).Find(r => r.Result.Athlete.Equals(athlete))),
				OverallRow("PointsTop3/M", Category.M, athlete, () => overall.MostPoints(3, filter).Find(r => r.Result.Athlete.Equals(athlete))),
				OverallRow("AgeGrade", null, athlete, () => overall.AgeGrade().Find(r => r.Result.Athlete.Equals(athlete))),
				OverallRow("Miles", null, athlete, () => overall.MostMiles().Find(r => r.Result.Athlete.Equals(athlete))),
				OverallRow("Community", null, athlete, () => overall.CommunityStars().Find(r => r.Result.Athlete.Equals(athlete))),
				OverallRow("Team", null, athlete, () => overall.TeamPoints().Find(r => r.Value.Team == athlete.Team))
			}.Where(c => c?.Value != null)
			.ToArray();

		OverallPoints = overall.MostPoints(filter).Find(r => r.Result.Athlete.Equals(athlete));
		OverallAgeGrade = overall.AgeGrade().Find(r => r.Result.Athlete.Equals(athlete));
		OverallMiles = overall.MostMiles().Find(r => r.Result.Athlete.Equals(athlete));
		OverallCommunityStars = overall.CommunityStars().Find(r => r.Result.Athlete.Equals(athlete));
		TeamResults = overall.TeamPoints().Find(r => r.Value.Team == athlete.Team);

		TotalResults = Fastest.Count(r => r.Value != null) + Average.Count(r => r.Value != null);
	}

	private AthleteOverallRow OverallRow<T>(string id, Category category, Athlete athlete, Func<Ranked<T>> results) where T : Formattable
	{
		if (!_config.Competitions.TryGetValue(id, out var name) || category != null && category != athlete.Category)
			return null;

		var result = results();
		return new AthleteOverallRow
		{
			ID = id,
			Name = name,
			Rank = result?.Rank,
			Value = result?.Value?.Display
		};
	}

	public SimilarAthlete[] SimilarAthletes()
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

	private const byte PercentThreshold = 5;

	private static bool IsMatch(Ranked<Time> mine, Ranked<Time> theirs)
		=> Time.AbsolutePercentDifference(mine.Value, theirs.Value) <= PercentThreshold;
}