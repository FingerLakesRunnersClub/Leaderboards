namespace FLRC.ChallengeDashboard
{
    public class OverallResultsViewModel<T> : OverallResultsViewModel
    {
        public RankedList<T> RankedResults { get; init; }
    }

    public class OverallResultsViewModel : ViewModel
    {
        public override string Title => ResultType;
        public string ResultType { get; init; }
    }
}