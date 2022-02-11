using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Overall;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Results;
using FLRC.Leaderboards.Core.Teams;

namespace FLRC.Leaderboards.Core.Athletes;

public class AthleteOverallRow : Ranked<string>
{
	public string ID { get; init; }
	public string Name { get; init; }
	public Category Category { get; init; }
}

public class AthleteSummary
{
	public Athlete Athlete { get; }
	public IDictionary<Course, Ranked<Time>> Fastest { get; }
	public IDictionary<Course, Ranked<Time>> Average { get; }
	public IDictionary<Course, Ranked<ushort>> Runs { get; }
	public Dictionary<Course, IEnumerable<Result>> All { get; }

	public IEnumerable<AthleteOverallRow> Competitions { get; }

	public Ranked<Points> OverallPoints { get; }
	public Ranked<AgeGrade> OverallAgeGrade { get; }
	public Ranked<Miles> OverallMiles { get; }
	public TeamResults TeamResults { get; }

	public int TotalResults { get; }
	private readonly IReadOnlyCollection<Course> _results;
	private readonly Config _config;


	public AthleteSummary(Athlete athlete, IReadOnlyCollection<Course> results, Config config)
	{
		_results = results;
		_config = config;

		Athlete = athlete;
		Fastest = results.ToDictionary(c => c, c => c.Fastest(athlete.Category).FirstOrDefault(r => r.Result.Athlete.Equals(athlete)));
		Average = results.ToDictionary(c => c, c => c.BestAverage(athlete.Category).FirstOrDefault(r => r.Result.Athlete.Equals(athlete)));
		Runs = results.ToDictionary(c => c, c => c.MostRuns().FirstOrDefault(r => r.Result.Athlete.Equals(athlete)));
		All = results.ToDictionary(c => c, c => c.Results.Where(r => r.Athlete.Equals(athlete)));

		var overallViewModel = new OverallResults(results);
		Competitions = new List<AthleteOverallRow>
		{
			OverallRow("Points/F", Category.F, athlete, () => overallViewModel.MostPoints(athlete.Category).FirstOrDefault(r => r.Result.Athlete.Equals(athlete))),
			OverallRow("Points/M", Category.M, athlete, () => overallViewModel.MostPoints(athlete.Category).FirstOrDefault(r => r.Result.Athlete.Equals(athlete))),
			OverallRow("PointsTop3/F", Category.F, athlete, () => overallViewModel.MostPoints(3, athlete.Category).FirstOrDefault(r => r.Result.Athlete.Equals(athlete))),
			OverallRow("PointsTop3/M", Category.M, athlete, () => overallViewModel.MostPoints(3, athlete.Category).FirstOrDefault(r => r.Result.Athlete.Equals(athlete))),
			OverallRow("AgeGrade", null, athlete, () => overallViewModel.AgeGrade().FirstOrDefault(r => r.Result.Athlete.Equals(athlete))),
			OverallRow("Miles", null, athlete, () => overallViewModel.MostMiles().FirstOrDefault(r => r.Result.Athlete.Equals(athlete))),
			OverallRow("Team", null, athlete, () => overallViewModel.TeamPoints().FirstOrDefault(r => r.Team == athlete.Team))
		}.Where(c => c?.Value != null);

		OverallPoints = overallViewModel.MostPoints(athlete.Category).FirstOrDefault(r => r.Result.Athlete.Equals(athlete));
		OverallAgeGrade = overallViewModel.AgeGrade().FirstOrDefault(r => r.Result.Athlete.Equals(athlete));
		OverallMiles = overallViewModel.MostMiles().FirstOrDefault(r => r.Result.Athlete.Equals(athlete));
		TeamResults = overallViewModel.TeamPoints().FirstOrDefault(r => r.Team == athlete.Team);

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

	public IEnumerable<SimilarAthlete> SimilarAthletes
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
				            && m.Similarity.Value >= 80);
		}
	}

	private const byte percentThreshold = 5;

	private static bool IsMatch(Ranked<Time> mine, Ranked<Time> theirs)
		=> Time.AbsolutePercentDifference(mine.Value, theirs.Value) <= percentThreshold;
}