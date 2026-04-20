using FLRC.Leaderboards.Core.Teams;
using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web;

public static class AthleteExtensions
{
	extension(Athlete athlete)
	{
		public Team Team(Iteration iteration)
		{
			if (iteration?.StartDate is null)
				return null;

			var ageOnStart = athlete.AgeAsOf(iteration.StartDate.Value);
			var team = Core.Teams.Team.Teams.First(t => ageOnStart >= t.Value.MinAge && ageOnStart <= (t.Value.MaxAge ?? byte.MaxValue));
			return team.Value;
		}
	}
}