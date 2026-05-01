using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.Services;

public interface ICommunityStarCalculator
{
	Task<CommunityStars> GetStars(Result result, Result[] all, IList<CommunityStars> existing);
}