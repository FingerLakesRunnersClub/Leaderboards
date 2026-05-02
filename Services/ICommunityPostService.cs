using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Services;

public interface ICommunityPostService
{
	CommunityPost[] GetPosts(Course course);
}