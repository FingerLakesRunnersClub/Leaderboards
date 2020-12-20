namespace FLRC.ChallengeDashboard
{
    public class ResultsViewModel<T> : ResultsViewModel
    {
        public RankedList<T> RankedResults { get; init; }
    }

    public class ResultsViewModel : ViewModel
    {
        public override string Title => $"{ResultType.Display} — {Course.Name}";

        public FormattedResultType ResultType { get; init; }
        public Category Category { get; init; }
        public Course Course { get; init; }
    }
}