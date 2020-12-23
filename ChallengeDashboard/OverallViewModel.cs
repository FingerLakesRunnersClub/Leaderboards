using System.Collections.Generic;
using System.Linq;

namespace FLRC.ChallengeDashboard
{
    public class OverallViewModel
    {
        private readonly IEnumerable<Course> _courses;

        public OverallViewModel(IEnumerable<Course> courses) => _courses = courses;

        public RankedList<Points> MostPoints(Category category = null)
            => RankedList(_courses.SelectMany(c => c.Fastest(category))
                .GroupBy(r => r.Athlete)
                .ToDictionary(g => g.Key, g => new Points(g.Sum(r => r.Points.Value)))
                .OrderByDescending(r => r.Value.Value));

        public RankedList<double> MostMiles(Category category = null)
            => RankedList(_courses.SelectMany(c => c.MostMiles(category))
                .GroupBy(r => r.Athlete)
                .ToDictionary(g => g.Key, g => g.Sum(r => r.Value))
                .OrderByDescending(r => r.Value));

        public IEnumerable<TeamResults> TeamPoints()
            => _courses.SelectMany(c => c.TeamPoints())
                .GroupBy(r => r.Team)
                .Select(g => new TeamResults
                {
                    Team = g.Key,
                    AgeGradePoints = (byte)(g.Sum(r => r.AgeGradePoints) + 7 * (10 - g.Count())),
                    MostRunsPoints = (byte)(g.Sum(r => r.MostRunsPoints) + 7 * (10 - g.Count()))
                })
                .Rank();

        private RankedList<T> RankedList<T>(IOrderedEnumerable<KeyValuePair<Athlete, T>> results)
        {
            var ranks = new RankedList<T>();
            var list = results.ToList();
            for (ushort rank = 1; rank <= list.Count(); rank++)
            {
                var result = list[rank - 1];
                ranks.Add(new Ranked<T>
                {
                    Rank = ranks.Any() && ranks.Last().Value.Equals(result.Value) ? ranks.Last().Rank : new Rank(rank),
                    Athlete = result.Key,
                    Value = result.Value
                });
            }

            return ranks;
        }
    }
}