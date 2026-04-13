using FLRC.Leaderboards.Core.Teams;
using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web;

public static class IterationExtensions
{
	extension(Iteration iteration)
	{
		public Athlete[] Team(Team team)
			=> iteration.StartDate is not null
				? iteration.Athletes
					.Where(a => a.Team(iteration) == team)
					.ToArray()
				: [];
	}
}