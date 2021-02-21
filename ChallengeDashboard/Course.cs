using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace FLRC.ChallengeDashboard
{
    public class Course
    {
        public const double MetersPerMile = 1609.344;

        public uint ID { get; init; }
        public string Name { get; init; }
        public string Type { get; init; }
        public string Distance { get; init; }
        public double Meters { get; set; }
        public string URL { get; init; }

        public DateTime LastUpdated { get; set; }
        public int LastHash { get; set; }

        private IEnumerable<Result> _results;

        public IEnumerable<Result> Results
        {
            get => _results;
            set
            {
                _results = value;
                resetCaches();
            }
        }

        private void resetCaches()
        {
            _fastestCache.Clear();
            _averageCache.Clear();
            _mostRunsCache.Clear();
            _mostMilesCache.Clear();
            _thresholdCache.Clear();
            _teamCache = null;
        }

        private readonly IDictionary<string, RankedList<Time>> _fastestCache =
            new ConcurrentDictionary<string, RankedList<Time>>();

        public RankedList<Time> Fastest(Category category = null, byte? ag = null)
        {
            var key = category?.Value?.ToString() ?? "X";
            if (ag == null && _fastestCache.ContainsKey(key))
                return _fastestCache[key];

            var results = Rank(category, rs => ag == null || rs.Key.Team.Value == ag,
                rs => rs.OrderBy(r => r.Duration).First(), rs => rs.Min(r => r.Duration));

            if (ag != null)
                return results;

            return _fastestCache[key] = results;
        }

        private readonly IDictionary<string, RankedList<Time>> _averageCache =
            new Dictionary<string, RankedList<Time>>();

        public RankedList<Time> BestAverage(Category category = null)
        {
            var key = category?.Value?.ToString() ?? "X";
            if (_averageCache.ContainsKey(key))
                return _averageCache[key];

            return _averageCache[key] = Rank(category, rs => rs.Count() >= AverageThreshold(category),
                rs => rs.Average(AverageThreshold(category)), rs => rs.Average(AverageThreshold(category)).Duration);
        }

        private readonly IDictionary<string, RankedList<ushort>> _mostRunsCache =
            new Dictionary<string, RankedList<ushort>>();

        public RankedList<ushort> MostRuns(Category category = null)
        {
            var key = category?.Value?.ToString() ?? "X";
            if (_mostRunsCache.ContainsKey(key))
                return _mostRunsCache[key];

            return _mostRunsCache[key] =
                RankDescending(category, rs => true, r => r.Average(), r => (ushort) r.Count());
        }

        private readonly IDictionary<string, RankedList<double>> _mostMilesCache =
            new Dictionary<string, RankedList<double>>();

        public RankedList<double> MostMiles(Category category = null)
        {
            var key = category?.Value?.ToString() ?? "X";
            if (_mostMilesCache.ContainsKey(key))
                return _mostMilesCache[key];

            return _mostMilesCache[key] = RankDescending(category, rs => true, r => r.Average(),
                r => r.Count() * Meters / MetersPerMile);
        }

        private IEnumerable<GroupedResult> GroupedResults(Category category = null)
            => Results.Where(r => category == null || (r.Athlete.Category?.Equals(category) ?? false))
                .GroupBy(r => r.Athlete).Select(g => new GroupedResult(g));

        private readonly IDictionary<string, ushort> _thresholdCache = new Dictionary<string, ushort>();

        public ushort AverageThreshold(Category category = null)
        {
            var key = category?.Value?.ToString() ?? "X";
            if (_thresholdCache.ContainsKey(key))
                return _thresholdCache[key];

            var groupedResults = GroupedResults(category).ToList();
            return _thresholdCache[key] = groupedResults.Any()
                ? (ushort) Math.Ceiling(groupedResults.Average(r => r.Count()))
                : (ushort) 0;
        }

        private IEnumerable<TeamResults> _teamCache;

        public IEnumerable<TeamResults> TeamPoints()
        {
            if (_teamCache != null)
                return _teamCache;

            var teamResults = GroupedResults()
                .GroupBy(g => g.Key.Team)
                .Select(t => new TeamResults
                {
                    Team = t.Key,
                    AverageAgeGrade = new AgeGrade(t.OrderBy(rs => rs.Min(r => r.Duration))
                        .Take(10)
                        .Select(rs =>
                            AgeGradeCalculator.AgeGradeCalculator.GetAgeGrade(
                                rs.First().Athlete.Category?.Value ?? Category.M.Value ?? throw null,
                                rs.First().Athlete.Age,
                                Meters,
                                rs.Min(r => r.Duration.Value)))
                        .Average()
                    ),
                    TotalRuns = (ushort) t.Sum(rs => rs.Count())
                }).ToList();

            var fastestTeams = teamResults.OrderByDescending(t => t.AverageAgeGrade).ToArray();
            for (var x = 0; x < fastestTeams.Length; x++)
                fastestTeams[x].AgeGradePoints =
                    x > 0 && fastestTeams[x].AverageAgeGrade.Equals(fastestTeams[x - 1].AverageAgeGrade)
                        ? fastestTeams[x - 1].AgeGradePoints
                        : (byte) (x + 1);

            var mostRunTeams = teamResults.OrderByDescending(t => t.TotalRuns).ToArray();
            for (var x = 0; x < mostRunTeams.Length; x++)
                mostRunTeams[x].MostRunsPoints =
                    x > 0 && mostRunTeams[x].TotalRuns.Equals(mostRunTeams[x - 1].TotalRuns)
                        ? mostRunTeams[x - 1].MostRunsPoints
                        : (byte) (x + 1);

            return _teamCache = teamResults.Rank();
        }

        private RankedList<T> Rank<T>(Category category, Func<GroupedResult, bool> filter,
            Func<GroupedResult, Result> getResult, Func<GroupedResult, T> sort)
            => RankedList(GroupedResults(category).Where(filter).OrderBy(sort), getResult, sort);

        private RankedList<T> RankDescending<T>(Category category, Func<GroupedResult, bool> filter,
            Func<GroupedResult, Result> getResult, Func<GroupedResult, T> sort)
            => RankedList(GroupedResults(category).Where(filter).OrderByDescending(sort), getResult, sort);

        private RankedList<T1> RankedList<T1>(IOrderedEnumerable<GroupedResult> sorted,
            Func<GroupedResult, Result> getResult, Func<GroupedResult, T1> getValue)
        {
            var ranks = new RankedList<T1>();

            var list = sorted.ThenBy(rs => getResult(rs).Duration).ToList();
            for (ushort rank = 1; rank <= list.Count; rank++)
            {
                var results = list[rank - 1];
                var athlete = results.Key;
                var result = getResult(results);
                var value = getValue(results);
                ranks.Add(new Ranked<T1>
                {
                    Rank = ranks.Any() && ranks.Last().Value.Equals(value) ? ranks.Last().Rank : new Rank(rank),
                    Result = result,
                    Value = value,
                    Count = (uint) results.Count(),
                    BehindLeader = rank == 1 ? new Time(TimeSpan.Zero) : result.Behind(ranks.First().Result),
                    Points = new Points(rank == 1
                        ? 100
                        : ranks.First().Result.Duration.Value.TotalSeconds / result.Duration.Value.TotalSeconds * 100),
                    AgeGrade = new AgeGrade(athlete.Category != null
                        ? AgeGradeCalculator.AgeGradeCalculator.GetAgeGrade(athlete.Category.Value ?? throw null,
                            athlete.Age, Meters, result.Duration.Value)
                        : 0)
                });
            }

            return ranks;
        }
    }
}