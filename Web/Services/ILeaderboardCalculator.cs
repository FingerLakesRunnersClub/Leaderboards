using FLRC.Leaderboards.Core.Leaders;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Web.ViewModels;

namespace FLRC.Leaderboards.Web.Services;

public interface ILeaderboardCalculator
{
	Leaderboard GetLeaderboard(Iteration iteration, LeaderboardResultType type, byte tableSize);
}