using System;
using System.Collections.Generic;
using System.Linq;

namespace FLRC.ChallengeDashboard
{
    public class OverallViewModel
    {
        private readonly IEnumerable<Course> _courses;

        public OverallViewModel(IEnumerable<Course> courses) => _courses = courses;

        public RankedList<Points> MostPoints(Category category = null)
            => RankedList(_courses.SelectMany(c => c.Fastest(category)).GroupBy(r => r.Result.Athlete), g => new Points(g.Sum(r => r.Points.Value)), g => g.Sum(r => r.Points.Value));

        public RankedList<double> MostMiles(Category category = null)
            => RankedList(_courses.SelectMany(c => c.MostMiles(category)).GroupBy(r => r.Result.Athlete), g => g.Sum(r => r.Value), g => g.Sum(r => r.Value));

        public RankedList<double> AgeGrade(Category category = null)
            => RankedList(_courses.SelectMany(c => c.Fastest(category)).GroupBy(r => r.Result.Athlete), g => g.Average(r => r.AgeGrade.Value), g => g.Count());

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

        private RankedList<T1> RankedList<T1,T2,T3>(IEnumerable<IGrouping<Athlete, Ranked<T2>>> results, Func<IGrouping<Athlete, Ranked<T2>>, T1> getValue, Func<IGrouping<Athlete, Ranked<T2>>, T3> sort)
        {
            var ranks = new RankedList<T1>();
            var list = results.OrderByDescending(sort).ThenByDescending(getValue).ToList();
            for (ushort rank = 1; rank <= list.Count; rank++)
            {
                var result = list[rank - 1];
                var value = getValue(result);
                ranks.Add(new Ranked<T1>
                {
                    Rank = ranks.Any() && ranks.Last().Value.Equals(value) ? ranks.Last().Rank : new Rank(rank),
                    Result = new Result { Athlete = result.Key },
                    Count = (uint)result.Count(),
                    AgeGrade = new AgeGrade(result.Average(r => r.AgeGrade.Value)),
                    Value = value
                });
            }

            return ranks;
        }
    }
}