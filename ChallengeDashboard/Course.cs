using System;
using System.Collections.Generic;
using System.Linq;
using FLRC.AgeGradeCalculator;

namespace FLRC.ChallengeDashboard
{
    public class Course
    {
        public uint ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public double Distance { get; set; }
        public string URL { get; set; }

        public IEnumerable<Result> Results { get; set; }

        public RankedList<TimeSpan> Fastest(Category? category = null)
            => Rank(category, rs => true, rs => rs.OrderBy(r => r.Duration).First(), rs => rs.Min(r => r.Duration));

        public RankedList<TimeSpan> BestAverage(Category? category = null)
            => Rank(category, rs => rs.Count() >= AverageThreshold(category), rs => rs.Average(AverageThreshold(category)), rs => rs.Average(AverageThreshold(category)).Duration);

        public RankedList<ushort> MostRuns(Category? category = null)
            => RankDescending(category, rs => true, r => r.Average(), r => (ushort)r.Count());

        private IEnumerable<GroupedResult> GroupedResults(Category? category = null)
            => Results.Where(r => !category.HasValue || r.Athlete.Category == category.Value)
                .GroupBy(r => r.Athlete).Select(g => new GroupedResult(g));

        private ushort AverageThreshold(Category? category = null)
            => GroupedResults(category).Any()
                ? (ushort)Math.Ceiling(GroupedResults(category).Average(r => r.Count()))
                : (ushort)0;

        private RankedList<T> Rank<T>(Category? category, Func<GroupedResult, bool> filter, Func<GroupedResult, Result> getResult, Func<GroupedResult, T> sort)
            => RankedList(GroupedResults(category).Where(filter).OrderBy(sort), getResult, sort);

        private RankedList<T> RankDescending<T>(Category? category, Func<GroupedResult, bool> filter, Func<GroupedResult, Result> getResult, Func<GroupedResult, T> sort)
            => RankedList(GroupedResults(category).Where(filter).OrderByDescending(sort), getResult, sort);



        private RankedList<T> RankedList<T>(IOrderedEnumerable<GroupedResult> sorted, Func<GroupedResult, Result> getResult, Func<GroupedResult, T> getValue)
        {
            var ranks = new RankedList<T>();

            var list = sorted.ThenBy(rs => getResult(rs).Duration).ToList();
            for (ushort rank = 1; rank <= list.Count; rank++)
            {
                var results = list[rank - 1];
                var athlete = results.Key;
                var result = getResult(results);
                var value = getValue(results);
                ranks.Add(new Ranked<T>
                {
                    Rank = rank,
                    Athlete = athlete,
                    Result = result,
                    Value = value,
                    BehindLeader = rank == 1 ? default : result.Behind(ranks.First().Result),
                    AgeGrade = athlete.Category != null
                        ? AgeGradeCalculator.AgeGradeCalculator.GetAgeGrade(athlete.Category ?? throw null, athlete.Age, Distance, result.Duration)
                        : 0
                });
            }

            return ranks;
        }
    }
}