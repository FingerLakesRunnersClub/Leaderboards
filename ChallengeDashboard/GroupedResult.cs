using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FLRC.ChallengeDashboard
{
    public class GroupedResult : IGrouping<Athlete, Result>, IComparable<GroupedResult>, IComparable
    {
        private readonly IGrouping<Athlete, Result> _group;

        public GroupedResult(IGrouping<Athlete, Result> group) => _group = group;

        public Result Average(int threshold)
            => new Result
            {
                Athlete = Key,
                Duration = TimeSpan.FromSeconds(_group.OrderBy(r => r.Duration)
                    .Take(threshold).Average(r => r.Duration.TotalSeconds))
            };

        public Athlete Key => _group.Key;
        public IEnumerator<Result> GetEnumerator() => _group.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int CompareTo(GroupedResult other) => _group.Key.ID.CompareTo(other.Key.ID);
        public int CompareTo(object obj) => CompareTo(obj as GroupedResult);
    }
}