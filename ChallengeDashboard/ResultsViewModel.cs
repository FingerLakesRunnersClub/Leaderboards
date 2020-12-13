using FLRC.AgeGradeCalculator;

namespace FLRC.ChallengeDashboard
{
    public class ResultsViewModel<T> : ResultsViewModel
    {
        public RankedList<T> RankedResults { get; set; }
    }

    public class ResultsViewModel
    {
        public EntityType EntityType { get; set; }
        public ResultType ResultType { get; set; }
        public Category? Category { get; set; }
        public Course Course { get; set; }
    }
}