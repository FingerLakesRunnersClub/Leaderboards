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
	public async Task CanFindAthleteByNameAndAge()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new AthleteService(db);

		var id = Guid.NewGuid();
		await db.AddAsync(new Athlete { ID = id, Name = "Test", DateOfBirth = new DateOnly(1985, 2, 3)});
		await db.SaveChangesAsync();

		//act
		var athlete = await service.Find("Test", 41, new DateTime(2026, 02, 16));

		//assert
		Assert.Equal(id, athlete.ID);
	}

	[Fact]
	public async Task CanFindAthleteByNameAndApproximateAge()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new AthleteService(db);

		var id = Guid.NewGuid();
		await db.AddAsync(new Athlete { ID = id, Name = "Test", DateOfBirth = new DateOnly(1985, 2, 3)});
		await db.SaveChangesAsync();

		//act
		var athlete = await service.Find("Test", 40, new DateTime(2026, 2, 1));

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

	[Fact]
	public async Task CanUpdateAthlete()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new AthleteService(db);

		var athlete = new Athlete { ID = Guid.NewGuid(), Name = "Test", DateOfBirth = DateOnly.Parse("1985-01-01")};
		await db.AddAsync(athlete);
		await db.SaveChangesAsync();

		//act
		var updated = new Athlete { ID = athlete.ID, Name = "Test 2", DateOfBirth = DateOnly.Parse("1985-02-16"), IsPrivate = true };
		await service.UpdateAthlete(athlete, updated);

		//assert
		var result = db.Set<Athlete>().Single();
		Assert.Equal("Test 2", result.Name);
		Assert.Equal("02/16/1985", result.DateOfBirth.ToString("MM/dd/yyyy"));
		Assert.True(result.IsPrivate);
	}

	[Fact]
	public async Task UpdateDoesNotRemoveExistingData()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new AthleteService(db);

		var athlete = new Athlete { ID = Guid.NewGuid(), Name = "Test", DateOfBirth = DateOnly.Parse("1985-02-16"), IsPrivate = true };
		await db.AddAsync(athlete);

		//act
		var updated = new Athlete { ID = athlete.ID, Name = "Test 2" };
		await service.UpdateAthlete(athlete, updated);

		//assert
		var result = db.Set<Athlete>().Single();
		Assert.Equal("Test 2", result.Name);
		Assert.Equal("02/16/1985", result.DateOfBirth.ToString("MM/dd/yyyy"));
		Assert.True(result.IsPrivate);
	}

	[Fact]
	public async Task UpdateAddsNewLinkedAccounts()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new AthleteService(db);

		var athlete = new Athlete { ID = Guid.NewGuid(), Name = "Test", DateOfBirth = DateOnly.Parse("1985-02-16"), IsPrivate = true };
		await db.AddAsync(athlete);

		//act
		var updated = new Athlete { ID = athlete.ID, Name = "Test 2", LinkedAccounts = [new LinkedAccount { Type = "t1", Value = "v1" }]};
		await service.UpdateAthlete(athlete, updated);

		//assert
		var result = db.Set<Athlete>().Single();
		Assert.NotEmpty(result.LinkedAccounts);
	}
}