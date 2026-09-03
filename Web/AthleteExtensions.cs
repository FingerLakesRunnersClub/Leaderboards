using System.Collections.Concurrent;
using FLRC.Leaderboards.Core.Teams;
using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web;

public static class AthleteExtensions
{
	private static readonly ConcurrentDictionary<(Athlete, Iteration), Team> TeamCache = new();

	extension(Athlete athlete)
	{
		public Team Team(Iteration iteration)
		{
			if (TeamCache.TryGetValue((athlete, iteration), out var cached))
				return cached;

			if (iteration?.StartDate is null)
				return null;

			var ageOnStart = athlete.AgeAsOf(iteration.StartDate.Value);
			var team = Core.Teams.Team.Teams.First(t => ageOnStart >= t.Value.MinAge && ageOnStart <= (t.Value.MaxAge ?? byte.MaxValue));
			return TeamCache.AddOrUpdate((athlete, iteration), team.Value, (_, _) => team.Value);
		}
	}
}