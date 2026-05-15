using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Web.ViewModels;

namespace FLRC.Leaderboards.Web.Services;

public interface IAwardsCalculator
{
	Dictionary<Athlete, Award[]> GetAwards(Iteration iteration);
}