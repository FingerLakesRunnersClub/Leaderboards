using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Teams;
using FLRC.Leaderboards.Model;
using TeamMember = FLRC.Leaderboards.Web.ViewModels.TeamMember;

namespace FLRC.Leaderboards.Web.Services;

public interface IOverallResultsCalculator
{
	RankedList<Points, Result> MostPoints(Iteration iteration, Filter filter = null);
	RankedList<Points, Result> MostPoints(Iteration iteration, byte limit, Filter filter = null);
	RankedList<AgeGrade, Result> AgeGrade(Iteration iteration, Filter filter = null);
	RankedList<Date, Result> Completed(Iteration iteration, Filter filter = null);
	RankedList<Date, Result> CompletedPersonal(Iteration iteration, Filter filter = null);
	RankedList<Miles, Result> MostMiles(Iteration iteration, Filter filter = null);
	RankedList<Count, Result> MostCourses(Iteration iteration, Filter filter = null);
	RankedList<Count, Result> Community(Iteration iteration, Filter filter = null);
	RankedList<TeamResults, Result> TeamPoints(Iteration iteration, Filter filter = null);
	RankedList<TeamMember, Result> TeamMembers(Iteration iteration, Team team, Filter filter = null);
}