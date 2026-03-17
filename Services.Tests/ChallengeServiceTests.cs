using FLRC.Leaderboards.Model;
using Xunit;

namespace FLRC.Leaderboards.Services.Tests;

public sealed class ChallengeServiceTests
{
	[Fact]
	public async Task CanAddNewChallenge()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new ChallengeService(db);

		//act
		await service.Add(new Challenge { Name = "test" });

		//assert
		Assert.Equal("test", db.Set<Challenge>().First().Name);
	}

	[Fact]
	public async Task CanAddAthleteToChallenge()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new ChallengeService(db);

		var athlete = new Athlete { ID = Guid.NewGuid() };
		var challenge = new Challenge { ID = Guid.NewGuid() };

		//act
		await service.AddConnection(athlete, challenge);

		//assert
		Assert.Equal(challenge, athlete.Challenges.First());
	}
}