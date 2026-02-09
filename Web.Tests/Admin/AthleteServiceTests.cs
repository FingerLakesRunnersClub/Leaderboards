using FLRC.Leaderboards.Data.Models;
using FLRC.Leaderboards.Web.Areas.Admin.Services;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Admin;

public sealed class AthleteServiceTests
{
	[Fact]
	public async Task CanGetAthlete()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new AthleteService(db);

		var id = Guid.NewGuid();
		await db.AddAsync(new Athlete { ID = id, Name = "Test" });
		await db.SaveChangesAsync();

		//act
		var athlete = await service.Get(id);

		//assert
		Assert.Equal("Test", athlete.Name);
	}

	[Fact]
	public async Task CanFindAthleteByNameAndDOB()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new AthleteService(db);

		var id = Guid.NewGuid();
		await db.AddAsync(new Athlete { ID = id, Name = "Test", DateOfBirth = new DateOnly(1985, 2, 3)});
		await db.SaveChangesAsync();

		//act
		var athlete = await service.Find("Test", DateOnly.Parse("2/3/1985"));

		//assert
		Assert.Equal(id, athlete.ID);
	}

	[Fact]
	public async Task CanFindAthleteByLinkedAccount()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new AthleteService(db);

		var id = Guid.NewGuid();
		await db.AddAsync(new Athlete
		{
			ID = id, Name = "Test", LinkedAccounts =
			[
				new LinkedAccount { ID = Guid.NewGuid(), AthleteID = id, Type = "Email", Value = "test@example.com"}
			]
		});
		await db.SaveChangesAsync();

		//act
		var athlete = await service.Find("Email", "test@example.com");

		//assert
		Assert.Equal("Test", athlete.Name);
	}

	[Fact]
	public async Task CanAddAthlete()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new AthleteService(db);

		//act
		var athlete = new Athlete { Name = "Test" };
		await service.AddAthlete(athlete);

		//assert
		Assert.Equal("Test", db.Set<Athlete>().Single().Name);
	}
}