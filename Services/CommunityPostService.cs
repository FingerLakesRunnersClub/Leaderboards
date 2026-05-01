using FLRC.Leaderboards.Model;
using Microsoft.Extensions.Caching.Memory;

namespace FLRC.Leaderboards.Services;

public sealed class CommunityPostService(ICommunityPostAPI communityAPI) : ICommunityPostService
{
	private static readonly MemoryCache Cache = new(new MemoryCacheOptions());
	private static readonly MemoryCache PermaCache = new(new MemoryCacheOptions());

	public async Task<CommunityPost[]> GetPosts(Course course)
	{
		if (Cache.TryGetValue<CommunityPost[]>(course, out var cached))
			return cached!;

		var link = course.Race.Links.FirstOrDefault(l => l.Type == RaceLink.Types.Community);
		if (link is null)
			return [];

		var id = ushort.Parse(link.URL.Split("/")[^1]);
		var response = await communityAPI.GetPosts(id);

		try
		{
			var posts = communityAPI.ParsePosts(response);
			Cache.Set(course, posts, TimeSpan.FromMinutes(1));
			PermaCache.Set(course, posts, TimeSpan.FromDays(365));
			return posts;
		}
		catch (HttpRequestException)
		{
			return PermaCache.TryGetValue<CommunityPost[]>(course, out var permacached)
				? permacached!
				: [];
		}
	}
}