using FLRC.Leaderboards.Core.Community;

namespace FLRC.Leaderboards.Core.Races;

public sealed class Race
{
	public uint ID { get; init; }
	public string Name { get; init; }
	public string Type { get; init; }
	public DateTime Date { get; init; }
	public string Source { get; init; }
	public string URL { get; init; }

	public ushort CommunityID { get; init; }

	private IReadOnlyCollection<Post> _communityPosts = Array.Empty<Post>();
	public IReadOnlyCollection<Post> CommunityPosts
	{
		get => _communityPosts;
		set
		{
			_communityPosts = value;
			foreach (var course in Courses)
			{
				course.ClearCommunityCache();
			}
		}
	}

	public int CommunityHash { get; set; }

	public IReadOnlyCollection<Course> Courses { get; set; } = Array.Empty<Course>();
}