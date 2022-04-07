using FLRC.Leaderboards.Core.Community;

namespace FLRC.Leaderboards.Core.Races;

public class Race
{
	public uint ID { get; init; }
	public string Name { get; init; }
	public string Type { get; init; }
	public DateTime Date { get; set; }
	public string Source { get; init; }
	public string URL { get; init; }

	public ushort CommunityID { get; init; }
	public IReadOnlyCollection<Post> CommunityPosts { get; set; }
	public int CommunityHash { get; set; }

	public IReadOnlyCollection<Course> Courses { get; set; }
}