using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.Services;

public interface ICommunityStarCalculator
{
	CommunityStars GetStars(Result result, Result[] all, IList<CommunityStars> existing);
}