using FLRC.Leaderboards.Model;
using Xunit;

namespace FLRC.Leaderboards.Services.Tests;

public sealed class RaceServiceTests
{
	[Fact]
	public async Task CanGetAllRaces()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new RaceService(db);

		await db.AddRangeAsync(
			new Race { ID = Guid.NewGuid(), Name = "Test Race 1", Type = "Test", Description = "test" },
			new Race { ID = Guid.NewGuid(), Name = "Test Race 2", Type = "Test", Description = "test" }
		);
		await db.SaveChangesAsync();

		//act
		var races = await service.All();

		//assert
		Assert.Equal(2, races.Length);
	}

	[Fact]
	public async Task CanGetSpecificRaces()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new RaceService(db);

		var id1 = Guid.NewGuid();
		var id2 = Guid.NewGuid();
		var id3 = Guid.NewGuid();
		await db.AddRangeAsync(
			new Race { ID = id1, Name = "Test Race 1", Type = "Test", Description = "test" },
			new Race { ID = id2, Name = "Test Race 2", Type = "Test", Description = "test" },
			new Race { ID = id3, Name = "Test Race 3", Type = "Test", Description = "test" }
		);
		await db.SaveChangesAsync();

		//act
		var races = await service.GetRaces([id1, id3]);

		//assert
		Assert.Equal(2, races.Length);
		Assert.Equal("Test Race 1", races[0].Name);
		Assert.Equal("Test Race 3", races[1].Name);
	}

	[Fact]
	public async Task CanGetRace()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new RaceService(db);

		var r1 = new Race { ID = Guid.NewGuid(), Name = "Test Race 1", Type = "Test", Description = "test" };
		var r2 = new Race { ID = Guid.NewGuid(), Name = "Test Race 2", Type = "Test", Description = "test" };
		await db.AddRangeAsync(r1, r2);
		await db.SaveChangesAsync();

		//act
		var race = await service.Get(r1.ID);

		//assert
		Assert.Equal("Test Race 1", race.Name);
	}

	[Fact]
	public async Task CanAddRace()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new RaceService(db);

		//act
		var race = new Race { ID = Guid.NewGuid(), Name = "Test Race 1", Type = "Test", Description = "test" };
		await service.Add(race);

		//assert
		Assert.Equal("Test Race 1", db.Set<Race>().Single().Name);
	}

	[Fact]
	public async Task CanEditRace()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new RaceService(db);

		var race = new Race { ID = Guid.NewGuid(), Name = "Test Race 1", Type = "Test", Description = "test" };
		await db.AddAsync(race);
		await db.SaveChangesAsync();

		//act
		var updated = new Race { Name = "Test Race 2" };
		await service.Update(race, updated);

		//assert
		Assert.Equal("Test Race 2", db.Set<Race>().Single().Name);
	}

	[Fact]
	public async Task CanEditAndAddCourses()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new RaceService(db);

		var race = new Race { ID = Guid.NewGuid(), Name = "Test Race 1", Type = "Test", Description = "test" };
		var course = new Course { ID = Guid.NewGuid(), RaceID = race.ID, Distance = 1, Units = nameof(Core.Races.DistanceUnit.mi) };
		await db.AddRangeAsync(race, course);
		await db.SaveChangesAsync();

		//act
		course.Distance = 2;
		var courses = new Dictionary<Guid, Course>()
		{
			{ course.ID, course},
			{ Guid.Empty, new Course { Distance = 3, Units = nameof(Core.Races.DistanceUnit.km)} }
		};
		await service.UpdateCourses(race, courses);

		//assert
		var updatedCourses = db.Set<Race>().Single().Courses.OrderBy(c => c.Distance).ToArray();
		Assert.Equal(2, updatedCourses.Length);
		Assert.Equal(2, updatedCourses[0].Distance);
		Assert.Equal(3, updatedCourses[1].Distance);
	}
}