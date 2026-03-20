using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Results;

namespace FLRC.Leaderboards.Core.Teams;

public static class TeamResultsExtensions
{
	public static RankedList<TeamResults, Result> Rank(this IEnumerable<TeamResults> teamResults)
	{
		var ranks = new RankedList<TeamResults, Result>();
		var ranked = teamResults.OrderBy(t => t.TotalPoints).ToArray();
		for (byte rank = 1; rank <= ranked.Length; rank++)
		{
			var value = ranked[rank - 1];
			ranks.Add(new Ranked<TeamResults, Result>
			{
				All = ranks,
				Rank = ranks.Any() && ranks[^1].Value.TotalPoints.Equals(value.TotalPoints) ? ranks[^1].Rank : new Rank(rank),
				AgeGrade = value.AverageAgeGrade,
				Value = value
			});
		}

		return ranks;
	}
}