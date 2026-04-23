using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Model;
using Course = FLRC.Leaderboards.Model.Course;

namespace FLRC.Leaderboards.Web;

public static class IterationExtensions
{
    extension(Iteration iteration)
    {
        public Course[] AllCourses
            => iteration.OfficialChallengeCourses.Concat(iteration.OtherCourses).ToArray();

        public Course[] OfficialChallengeCourses
            => iteration.OfficialChallenge?.Courses.OrderBy(c => new Distance(c.DistanceDisplay).Meters).ToArray() ?? [];

        public Course[] OtherCourses
            => iteration.Races.SelectMany(r => r.Courses).Except(iteration.OfficialChallengeCourses).OrderBy(c => c.Race.Name).ToArray();
    }
}