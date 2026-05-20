using FLRC.Leaderboards.Model;
using Xunit;

namespace FLRC.Leaderboards.Services.Tests;

public sealed class ChallengeServiceTests
{
	[Fact]
	public async Task CanGetAllChallenges()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new ChallengeService(db);

		var iteration = new Iteration { Name = "Test" };
		await db.AddRangeAsync(
			new Challenge { ID = Guid.NewGuid(), Name = "test1", Iteration = iteration },
			new Challenge { ID = Guid.NewGuid(), Name = "test2", Iteration = iteration },
			new Challenge { ID = Guid.NewGuid(), Name = "test3", Iteration = iteration }
		);
		await db.SaveChangesAsync();

		//act
		var challenges = await service.All();

		//assert
		Assert.Equal(3, challenges.Length);
	}

	[Fact]
	public async Task CanGetChallenge()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new ChallengeService(db);

		var id = Guid.NewGuid();
		var iteration = new Iteration { Name = "Test" };
		await db.AddAsync(new Challenge { ID = id, Name = "test1", Iteration = iteration });
		await db.SaveChangesAsync();

		//act
		var challenge = await service.Get(id);

		//assert
		Assert.Equal("test1", challenge.Name);
	}

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

	[Fact]
	public async Task CanRemoveAthleteFromChallenge()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new ChallengeService(db);

		var challenge = new Challenge { ID = Guid.NewGuid(), Name = "Test" };
		var athlete = new Athlete { ID = Guid.NewGuid(), Name = "Test", Challenges = [challenge]};
		await db.AddAsync(challenge);
		await db.AddAsync(athlete);
		await db.SaveChangesAsync();

		//act
		await service.RemoveConnection(athlete, challenge);

		//assert
		Assert.Empty(athlete.Challenges);
	}

	[Fact]
	public async Task RemovingPersonalChallengeDeletesChallenge()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new ChallengeService(db);

		var challenge = new Challenge { ID = Guid.NewGuid(), Name = "Test", IsOfficial = false };
		var athlete = new Athlete { ID = Guid.NewGuid(), Name = "Test", Challenges = [challenge]};
		await db.AddAsync(challenge);
		await db.AddAsync(athlete);
		await db.SaveChangesAsync();

		//act
		await service.RemoveConnection(athlete, challenge);

		//assert
		Assert.Empty(db.Set<Challenge>());
	}

	[Fact]
	public async Task RemovingOfficialChallengeDoesNotDeleteChallenge()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new ChallengeService(db);

		var challenge = new Challenge { ID = Guid.NewGuid(), Name = "Test", IsOfficial = true };
		var athlete = new Athlete { ID = Guid.NewGuid(), Name = "Test", Challenges = [challenge]};
		await db.AddAsync(challenge);
		await db.AddAsync(athlete);
		await db.SaveChangesAsync();

		//act
		await service.RemoveConnection(athlete, challenge);

		//assert
		Assert.NotEmpty(db.Set<Challenge>());
	}

	[Fact]
	public async Task CanUpdateChallenge()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new ChallengeService(db);

		var iteration = new Iteration { Name = "Test" };
		var challenge = new Challenge { ID = Guid.NewGuid(), Name = "test1", Iteration = iteration };
		await db.AddAsync(challenge);

		//act
		await service.Update(challenge, new Challenge { Name = "test2" });

		//assert
		Assert.Equal("test2", db.Set<Challenge>().First().Name);
	}

	[Fact]
	public async Task CanUpdateCourses()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new ChallengeService(db);

		var c1 = new Course { ID = Guid.NewGuid(), Distance = 5, Units = "km", Race = new Race { Name = "R1", Type = "Road", Description = "test" } };
		var c2 = new Course { ID = Guid.NewGuid(), Distance = 10, Units = "km", Race = new Race { Name = "R2", Type = "Road", Description = "test" } };
		var c3 = new Course { ID = Guid.NewGuid(), Distance = 15, Units = "km", Race = new Race { Name = "R3", Type = "Road", Description = "test" } };
		var iteration = new Iteration { Name = "Test" };
		var challenge = new Challenge { ID = Guid.NewGuid(), Name = "test1", Iteration = iteration, Courses = [c1, c2] };
		await db.AddAsync(challenge);

		//act
		await service.UpdateCourses(challenge, [c2, c3]);

		//assert
		var updated = db.Set<Challenge>().First().Courses.ToArray();
		Assert.Equal(2, updated.Length);
		Assert.Equal("R2", updated[0].Race.Name);
		Assert.Equal("R3", updated[1].Race.Name);
	}
}