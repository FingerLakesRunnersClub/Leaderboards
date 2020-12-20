using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FLRC.ChallengeDashboard
{
    public class GroupedResult : IGrouping<Athlete, Result>, IComparable<GroupedResult>
    {
        private readonly IGrouping<Athlete, Result> _group;

        public GroupedResult(IGrouping<Athlete, Result> group) => _group = group;

        public Result Average(ushort? threshold = null)
            => new Result
            {
                Athlete = Key,
                Duration = new Time(TimeSpan.FromSeconds(_group.OrderBy(r => r.Duration)
                    .Take(threshold ?? _group.Count()).Average(r => r.Duration.Value.TotalSeconds)))
            };

        public Athlete Key => _group.Key;
        public IEnumerator<Result> GetEnumerator() => _group.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int CompareTo(GroupedResult other) => _group.Key.ID.CompareTo(other.Key.ID);
    }
}