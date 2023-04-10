using FLRC.Leaderboards.Core.Community;

namespace FLRC.Leaderboards.Core.Tests.Community;

public static class CommunityData
{
	public static readonly User User1 = new() { Name = "A1", ID = 12, Username = "u1" };
	public static readonly User User2 = new() { Name = "A2", ID = 23, Username = "u2" };
	public static readonly User User3 = new() { Name = "A3", ID = 34, Username = "u3" };
	public static readonly User User5 = new() { Name = "A5", ID = 56, Username = "u5" };

	public static readonly IReadOnlyCollection<User> Users
		= new[] { User1, User2, User3, User5 };

	public static readonly IDictionary<byte, string> Groups
		= new Dictionary<byte, string>
		{
			{ 0, "everybody-everybody" },
			{ 2, "youngins" },
			{ 3, "dirty-thirties" },
			{ 4, "masters" },
			{ 5, "oldies-but-goodies" }
		};
}