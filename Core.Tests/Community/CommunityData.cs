using FLRC.Leaderboards.Core.Community;
using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Core.Tests.Community;

public static class CommunityData
{
	public static readonly Athlete Athlete1 = new() { ID = Guid.NewGuid(), DateOfBirth = new DateOnly(2000, 1, 1), LinkedAccounts = [new LinkedAccount { Type = LinkedAccount.Keys.Discourse, Value = "12" }] };
	public static readonly Athlete Athlete2 = new() { ID = Guid.NewGuid(), DateOfBirth = new DateOnly(1990, 1, 1), LinkedAccounts = [new LinkedAccount { Type = LinkedAccount.Keys.Discourse, Value = "23" }] };
	public static readonly Athlete Athlete3 = new() { ID = Guid.NewGuid(), DateOfBirth = new DateOnly(2000, 1, 1), LinkedAccounts = [new LinkedAccount { Type = LinkedAccount.Keys.Discourse, Value = "34" }] };
	public static readonly Athlete Athlete4 = new() { ID = Guid.NewGuid(), DateOfBirth = new DateOnly(1990, 1, 1) };
	public static readonly Athlete Private = new() { ID = Guid.NewGuid(), DateOfBirth = new DateOnly(1980, 1, 1), LinkedAccounts = [new LinkedAccount { Type = LinkedAccount.Keys.Discourse, Value = "56" }], IsPrivate = true };

	public static readonly Iteration Iteration = new() { ID = Guid.NewGuid(), StartDate = new DateOnly(2020, 1, 1), Athletes = [Athlete1, Athlete2, Athlete3, Athlete4, Private] };

	public static readonly User User1 = new() { Name = "A1", ID = 12, Username = "u1" };
	public static readonly User User2 = new() { Name = "A2", ID = 23, Username = "u2" };
	public static readonly User User3 = new() { Name = "A3", ID = 34, Username = "u3" };
	public static readonly User User5 = new() { Name = "A5", ID = 56, Username = "u5" };

	public static readonly User[] Users = [User1, User2, User3, User5];

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