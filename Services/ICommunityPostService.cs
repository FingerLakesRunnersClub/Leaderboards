using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Services;

public interface ICommunityPostService
{
	Task<CommunityPost[]> GetPosts(Course course);
}