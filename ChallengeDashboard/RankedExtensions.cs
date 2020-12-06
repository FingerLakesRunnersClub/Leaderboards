using System;
using System.Collections.Generic;
using System.Linq;

namespace ChallengeDashboard
{
    public static class RankedExtensions
    {
        public static RankedList<T> Rank<T>(this IEnumerable<GroupedResult> results, Func<GroupedResult, T> sort) =>
            results.OrderBy(sort).Rank(sort);

        public static RankedList<T> RankDescending<T>(this IEnumerable<GroupedResult> results,
            Func<GroupedResult, T> sort) => results.OrderByDescending(sort).Rank(sort);

        private static RankedList<T> Rank<T>(this IOrderedEnumerable<GroupedResult> sorted, Func<GroupedResult, T> getValue)
        {
            var ranks = new RankedList<T>();

            var list = sorted.ToList();
            for (ushort rank = 1; rank <= list.Count; rank++)
            {
                var result = list[rank - 1];
                var value = getValue(result);
                ranks.Add(new Ranked<T>
                {
                    Rank = rank,
                    Athlete = result.Key,
                    Value = value,
                    BehindLeader = rank == 1 ? default : Diff(value, ranks.First().Value)
                });
            }

            return ranks;
        }

        private static TimeSpan Diff<T>(T t1, T t2)
        {
            if (typeof(T) != typeof(Result))
                return default;

            return t1 is Result r1 && t2 is Result r2 ? r1.Duration - r2.Duration : default;
        }
    }
}