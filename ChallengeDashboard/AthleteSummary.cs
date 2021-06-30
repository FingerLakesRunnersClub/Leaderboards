using System.Collections.Generic;
using System.Linq;

namespace FLRC.ChallengeDashboard
{
    public class AthleteSummary
    {
        public Athlete Athlete { get; }
        public IDictionary<Course, Ranked<Time>> Fastest { get; }
        public IDictionary<Course, Ranked<Time>> Average { get; }
        public IDictionary<Course, Ranked<ushort>> Runs { get; }
        public Dictionary<Course, IEnumerable<Result>> All { get; }
        
        public Ranked<Points> OverallPoints { get; }
        public Ranked<AgeGrade> OverallAgeGrade { get; }
        public Ranked<double> OverallMiles { get; }
        public TeamResults TeamResults { get; }

        public int TotalResults { get; }

        public AthleteSummary(Athlete athlete, IReadOnlyCollection<Course> results)
        {
            Athlete = athlete;
            Fastest = results.ToDictionary(c => c, c => c.Fastest(athlete.Category).FirstOrDefault(r => r.Result.Athlete.ID == athlete.ID));
            Average = results.ToDictionary(c => c, c => c.BestAverage(athlete.Category).FirstOrDefault(r => r.Result.Athlete.ID == athlete.ID));
            Runs = results.ToDictionary(c => c, c => c.MostRuns().FirstOrDefault(r => r.Result.Athlete.ID == athlete.ID));
            All = results.ToDictionary(c => c, c => c.Results.Where(r => r.Athlete.ID == athlete.ID));
            
            var overallViewModel = new OverallResults(results);
            OverallPoints = overallViewModel.MostPoints(athlete.Category).FirstOrDefault(r => r.Result.Athlete.ID == athlete.ID);
            OverallAgeGrade = overallViewModel.AgeGrade().FirstOrDefault(r => r.Result.Athlete.ID == athlete.ID);
            OverallMiles = overallViewModel.MostMiles().FirstOrDefault(r => r.Result.Athlete.ID == athlete.ID);
            TeamResults = overallViewModel.TeamPoints().FirstOrDefault(r => r.Team == athlete.Team);

            TotalResults = Fastest.Count(r => r.Value != null) + Average.Count(r => r.Value != null);
        }
    }
}