using FLRC.Leaderboards.Model;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FLRC.Leaderboards.Services.Tests;

public sealed class AthleteServiceTests
{
	[Fact]
	public async Task AllAthletesAreOrderedByName()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new AthleteService(db);

		await db.AddRangeAsync(
			new Athlete { ID = Guid.NewGuid(), Name = "Test 2" },
			new Athlete { ID = Guid.NewGuid(), Name = "Test 1" }
		);
		await db.SaveChangesAsync();

		//act
		var athletes = await service.GetAllAthletes();

		//assert
		Assert.Equal("Test 1", athletes[0].Name);
		Assert.Equal("Test 2", athletes[1].Name);
	}

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
		await db.AddAsync(new Athlete { ID = id, Name = "Test", DateOfBirth = new DateOnly(1985, 2, 3) });
		await db.SaveChangesAsync();

		//act
		var athlete = await service.Find("Test", DateOnly.Parse("2/3/1985"));

		//assert
		Assert.Equal(id, athlete!.ID);
	}

	[Fact]
	public async Task CanFindAthleteByNameAndAge()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new AthleteService(db);

		var id = Guid.NewGuid();
		await db.AddAsync(new Athlete { ID = id, Name = "Test", DateOfBirth = new DateOnly(1985, 2, 3) });
		await db.SaveChangesAsync();

		//act
		var athlete = await service.Find("Test", 41, new DateTime(2026, 02, 16));

		//assert
		Assert.Equal(id, athlete!.ID);
	}

	[Fact]
	public async Task CanFindAthleteByNameAndApproximateAge()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new AthleteService(db);

		var id = Guid.NewGuid();
		await db.AddAsync(new Athlete { ID = id, Name = "Test", DateOfBirth = new DateOnly(1985, 2, 3) });
		await db.SaveChangesAsync();

		//act
		var athlete = await service.Find("Test", 40, new DateTime(2026, 2, 1));

		//assert
		Assert.Equal(id, athlete!.ID);
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
				new LinkedAccount { ID = Guid.NewGuid(), AthleteID = id, Type = "Email", Value = "test@example.com" }
			]
		});
		await db.SaveChangesAsync();

		//act
		var athlete = await service.Find("Email", "test@example.com");

		//assert
		Assert.Equal("Test", athlete!.Name);
	}

	[Fact]
	public async Task CanAddAthlete()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new AthleteService(db);

		//act
		var athlete = new Athlete { ID = Guid.NewGuid(), Name = "Test" };
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

		var athlete = new Athlete { ID = Guid.NewGuid(), Name = "Test", DateOfBirth = DateOnly.Parse("1985-01-01") };
		await db.AddAsync(athlete);
		await db.SaveChangesAsync();

		//act
		var updated = new Athlete { ID = athlete.ID, Name = "Test 2", DateOfBirth = DateOnly.Parse("1985-02-16"), IsPrivate = true };
		await service.UpdateAthlete(athlete, updated);

		//assert
		var result = db.Set<Athlete>().Single();
		Assert.Equal("Test 2", result.Name);
		Assert.Equal("02/16/1985", result.DateOfBirth?.ToString("MM/dd/yyyy"));
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
		await db.SaveChangesAsync();

		//act
		var updated = new Athlete { ID = athlete.ID, Name = "Test 2" };
		await service.UpdateAthlete(athlete, updated);

		//assert
		var result = db.Set<Athlete>().Single();
		Assert.Equal("Test 2", result.Name);
		Assert.Equal("02/16/1985", result.DateOfBirth?.ToString("MM/dd/yyyy"));
		Assert.True(result.IsPrivate);
	}

	[Fact]
	public async Task CanAddNewLinkedAccounts()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new AthleteService(db);

		var athlete = new Athlete { ID = Guid.NewGuid(), Name = "Test", DateOfBirth = DateOnly.Parse("1985-02-16"), IsPrivate = true };
		await db.AddAsync(athlete);
		await db.SaveChangesAsync();

		//act
		var account = new LinkedAccount { Type = "t1", Value = "v1" };
		await service.AddLinkedAccount(athlete, account);

		//assert
		var result = db.Set<Athlete>().Single();
		Assert.NotEmpty(result.LinkedAccounts);
	}

	[Fact]
	public async Task CanAddAthleteAsAdmin()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new AthleteService(db);

		var athlete = new Athlete { ID = Guid.NewGuid(), Name = "Test" };
		await db.AddAsync(athlete);
		await db.SaveChangesAsync();

		//act
		await service.AddAdmin(athlete);

		//assert
		Assert.True(athlete.IsAdmin);
	}

	[Fact]
	public async Task CanRemoveAthleteAsAdmin()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new AthleteService(db);

		var id = Guid.NewGuid();
		var athlete = new Athlete { ID = id, Name = "Test", Admins = [new Admin { ID = id }] };
		await db.AddAsync(athlete);
		await db.SaveChangesAsync();

		//act
		await service.RemoveAdmin(athlete);

		//assert
		Assert.False(athlete.IsAdmin);
	}

	[Fact]
	public async Task CanDelete()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new AthleteService(db);

		var id = Guid.NewGuid();
		var athlete = new Athlete { ID = id, Name = "Test" };
		await db.AddAsync(athlete);
		await db.SaveChangesAsync();

		//act
		await service.DeleteAthlete(athlete);

		//assert
		Assert.Empty(await db.Set<Athlete>().ToArrayAsync());
	}
}