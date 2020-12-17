using System.Collections.Generic;

namespace FLRC.ChallengeDashboard
{
    public class AthleteViewModel
    {
        public Athlete Athlete { get; set; }
        public IDictionary<Course, IEnumerable<Result>> Results { get; set; }
    }
}