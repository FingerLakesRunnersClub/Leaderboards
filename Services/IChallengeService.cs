using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Services;

public interface IChallengeService : IDataService<Challenge>
{
	Task AddConnection(Athlete athlete, Challenge challenge);
	Task UpdateCourses(Challenge challenge, Course[] courses);
}