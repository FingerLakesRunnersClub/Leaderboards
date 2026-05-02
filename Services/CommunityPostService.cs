using System.Collections.Concurrent;
using FLRC.Leaderboards.Model;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;

namespace FLRC.Leaderboards.Services;

public sealed class CommunityPostService(ICourseService courseService, ICommunityPostAPI communityAPI) : BackgroundService, ICommunityPostService
{
	private static readonly ConcurrentDictionary<Guid, ushort> CourseTopics = new();
	private static readonly Queue<ushort> TopicQueue = new();
	private static readonly MemoryCache PostCache = new(new MemoryCacheOptions());
	private static readonly MemoryCache FetchCache = new(new MemoryCacheOptions());

	private static readonly TimeSpan FetchThrottle = TimeSpan.FromSeconds(1);
	private static readonly TimeSpan FetchCacheLength = TimeSpan.FromMinutes(1);
	private static readonly TimeSpan PostCacheLength = TimeSpan.FromDays(365);

	public CommunityPost[] GetPosts(Course course)
	{
		if (!CourseTopics.TryGetValue(course.ID, out var topicID))
			return [];

		if (FetchCache.TryGetValue<CommunityPost[]>(topicID, out var latest))
			return latest!;

		if (!TopicQueue.Contains(topicID))
			TopicQueue.Enqueue(topicID);

		return PostCache.TryGetValue<CommunityPost[]>(topicID, out var cached)
			? cached!
			: [];
	}

	private async Task Update(ushort topicID)
	{
		try
		{
			var response = await communityAPI.GetPosts(topicID);
			var posts = communityAPI.ParsePosts(response);
			FetchCache.Set(topicID, posts, FetchCacheLength);
			PostCache.Set(topicID, posts, PostCacheLength);
		}
		catch (HttpRequestException)
		{
		}
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var courses = await courseService.All();

		var topics = courses.ToDictionary(c => c.ID, TopicID)
			.Where(t => t.Value is not null)
			.Cast<KeyValuePair<Guid, string>>();

		foreach (var topic in topics)
			CourseTopics.TryAdd(topic.Key, ushort.Parse(topic.Value));

		while (!stoppingToken.IsCancellationRequested)
		{
			if (TopicQueue.TryDequeue(out var next))
				await Update(next);

			await Task.Delay(FetchThrottle, stoppingToken);
		}
	}

	private static string? TopicID(Course c)
		=> c.Race.Links.FirstOrDefault(l => l.Type == RaceLink.Types.Community)?.URL.Split("/")[^1];
}