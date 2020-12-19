using System.Collections.Generic;

namespace FLRC.ChallengeDashboard
{
    public abstract class ViewModel
    {
        public abstract string Title { get; }
        public virtual IDictionary<uint, string> CourseNames { get; set; }
    }
}