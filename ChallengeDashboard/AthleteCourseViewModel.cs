namespace FLRC.ChallengeDashboard
{
    public class AthleteCourseViewModel : ViewModel
    {
        public override string Title => $"{Athlete.Name} — {Course.Name}";
        public Athlete Athlete { get; init; }
        public Course Course { get; init; }
        public RankedList<Time> Results { get; init; }
    }
}