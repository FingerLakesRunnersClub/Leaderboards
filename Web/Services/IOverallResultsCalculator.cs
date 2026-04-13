using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Teams;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Web.ViewModels;

namespace FLRC.Leaderboards.Web.Services;

public interface IOverallResultsCalculator
{
	RankedList<Points, Result> MostPoints(Filter filter = null);
	RankedList<Points, Result> MostPoints(byte limit, Filter filter = null);
	RankedList<Miles, Result> MostMiles(Filter filter = null);
	RankedList<AgeGrade, Result> AgeGrade(Filter filter = null);
	RankedList<Date, Result> Completed(Filter filter = null);

	RankedList<TeamResults, Result> TeamPoints(Filter filter = null);
	RankedList<TeamMember, Result> TeamMembers(Team team, Filter filter = null);
}