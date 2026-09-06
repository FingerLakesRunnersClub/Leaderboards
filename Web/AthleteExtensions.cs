using System.Collections.Concurrent;
using FLRC.Leaderboards.Core.Teams;
using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web;

public static class AthleteExtensions
{
	private static readonly ConcurrentDictionary<(Guid, Guid), Team> TeamCache = new();

	extension(Athlete athlete)
	{
		public Team Team(Iteration iteration)
		{
			if (TeamCache.TryGetValue((athlete.ID, iteration.ID), out var cached))
				return cached;

			if (iteration.StartDate is null)
				return null;

			var ageOnStart = athlete.AgeAsOf(iteration.StartDate.Value);
			var team = Core.Teams.Team.Teams.First(t => ageOnStart >= t.Value.MinAge && ageOnStart <= (t.Value.MaxAge ?? byte.MaxValue));
			return TeamCache.AddOrUpdate((athlete.ID, iteration.ID), team.Value, (_, _) => team.Value);
		}
	}
}