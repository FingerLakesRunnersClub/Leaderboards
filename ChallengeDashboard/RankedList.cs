using System.Collections.Generic;
using System.Linq;

namespace FLRC.ChallengeDashboard
{
    public class RankedList<T> : List<Ranked<T>>
    {
        public RankedList()
        {
        }
        
        public RankedList(IEnumerable<Ranked<T>> list)
        {
            AddRange(list);
        }
    }
}