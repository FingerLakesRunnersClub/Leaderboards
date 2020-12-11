using System;
using System.Collections.Generic;
using System.Linq;

namespace ChallengeDashboard
{
    public class Course
    {
        public uint ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Distance { get; set; }
        public string URL { get; set; }

        public IEnumerable<Result> Results { get; set; }

        public RankedList<Result> Fastest(Category? category = null) => GroupedResults(category).Rank(rs => rs.OrderBy(r => r.Duration).First());

        public RankedList<Result> BestAverage(Category? category = null) => GroupedResults(category).Where(rs => rs.Count() >= AverageThreshold(category)).Rank(r => r.Average(AverageThreshold(category)));

        public RankedList<ushort> MostRuns(Category? category = null) => GroupedResults(category).RankDescending(r => (ushort)r.Count());

        private IEnumerable<GroupedResult> GroupedResults(Category? category = null)
            => Results.Where(r => !category.HasValue || r.Athlete.Category[0] == Convert.ToChar(category.Value))
                .GroupBy(r => r.Athlete).Select(g => new GroupedResult(g));

        private ushort AverageThreshold(Category? category = null)
            => GroupedResults(category).Any()
                ? (ushort)Math.Ceiling(GroupedResults(category).Average(r => r.Count()))
                : 0;
    }
}