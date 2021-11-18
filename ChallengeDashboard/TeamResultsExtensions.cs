namespace FLRC.ChallengeDashboard;

public static class TeamResultsExtensions
{
	public static IEnumerable<TeamResults> Rank(this IEnumerable<TeamResults> teamResults)
	{
		var topTeams = teamResults.OrderBy(t => t.TotalPoints).ToArray();
		for (var x = 0; x < topTeams.Length; x++)
			topTeams[x].Rank = x > 0 && topTeams[x].TotalPoints == topTeams[x - 1].TotalPoints
				? topTeams[x - 1].Rank
				: new Rank((ushort)(x + 1));

		return topTeams;
	}
}