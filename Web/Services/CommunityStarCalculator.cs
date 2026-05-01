using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;

namespace FLRC.Leaderboards.Web.Services;

public sealed class CommunityStarCalculator(ICommunityPostService service) : ICommunityStarCalculator
{
	public async Task<CommunityStars> GetStars(Result result, Result[] all, IList<CommunityStars> existing)
	{
		var posts = await service.GetPosts(result.Course);
		var stars = new CommunityStars
		{
			Result = result,
			GroupRun = result.IsGroupRun(all) && !AlreadyHasStarToday(result, existing, s => s.GroupRun),
			StoryPost = posts.Any(p => p.Matches(result) && p.HasNarrative()) && !AlreadyHasStarToday(result, existing, s => s.StoryPost)
		};
		return stars.GroupRun || stars.StoryPost
			? stars
			: null;
	}

	private static bool AlreadyHasStarToday(Result result, IList<CommunityStars> existing, Func<CommunityStars, bool> starType)
		=> existing.Any(s => s.Result.Athlete == result.Athlete && s.Result.Course == result.Course && s.Result.StartTime.Date == result.StartTime.Date && starType(s));
}