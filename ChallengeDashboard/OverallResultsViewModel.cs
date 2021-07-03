namespace FLRC.ChallengeDashboard
{
    public class OverallResultsViewModel<T> : OverallResultsViewModel
    {
        public RankedList<T> RankedResults { get; init; }
    }

    public class OverallResultsViewModel : DataTableViewModel
    {
        public override string Title => ResultType;
        public string ResultType { get; init; }

        public override string ResponsiveBreakpoint
            => ResultType switch
            {
                "Most Miles" => "md",
                _ => "lg"
            };
    }
}