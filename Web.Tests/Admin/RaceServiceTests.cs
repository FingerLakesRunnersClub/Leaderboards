using FLRC.Leaderboards.Data.Models;
using FLRC.Leaderboards.Web.Areas.Admin.Services;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Admin;

public sealed class RaceServiceTests
{
	[Fact]
	public async Task CanGetAllRaces()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new RaceService(db);

		await db.AddRangeAsync(
			new Race { ID = Guid.NewGuid(), Name = "Test Race 1", Type = "Test" },
			new Race { ID = Guid.NewGuid(), Name = "Test Race 2", Type = "Test" }
		);
		await db.SaveChangesAsync();

		//act
		var races = await service.GetAllRaces();

		//assert
		Assert.Equal(2, races.Length);
	}

	[Fact]
	public async Task CanGetRace()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new RaceService(db);

		var r1 = new Race { ID = Guid.NewGuid(), Name = "Test Race 1", Type = "Test" };
		var r2 = new Race { ID = Guid.NewGuid(), Name = "Test Race 2", Type = "Test" };
		await db.AddRangeAsync(r1, r2);
		await db.SaveChangesAsync();

		//act
		var race = await service.GetRace(r1.ID);

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
		var race = new Race { ID = Guid.NewGuid(), Name = "Test Race 1", Type = "Test" };
		await service.AddRace(race, new Dictionary<Guid, Course>());

		//assert
		Assert.Equal("Test Race 1", db.Set<Race>().Single().Name);
	}

	[Fact]
	public async Task CanEditRace()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new RaceService(db);

		var race = new Race { ID = Guid.NewGuid(), Name = "Test Race 1", Type = "Test" };
		await db.AddAsync(race);
		await db.SaveChangesAsync();

		//act
		var updated = new Race { Name = "Test Race 2" };
		await service.UpdateRace(race, updated, new Dictionary<Guid, Course>());

		//assert
		Assert.Equal("Test Race 2", db.Set<Race>().Single().Name);
	}

	[Fact]
	public async Task CanEditAndAddCourses()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new RaceService(db);

		var race = new Race { ID = Guid.NewGuid(), Name = "Test Race 1", Type = "Test" };
		var course = new Course { ID = Guid.NewGuid(), RaceID = race.ID, Distance = 1, Units = nameof(Core.Races.DistanceUnit.mi) };
		await db.AddRangeAsync(race, course);
		await db.SaveChangesAsync();

		//act
		var updated = new Race { Name = "Test Race 2" };
		course.Distance = 2;
		var courses = new Dictionary<Guid, Course>()
		{
			{ course.ID, course},
			{ Guid.Empty, new Course { Distance = 3, Units = nameof(Core.Races.DistanceUnit.km)} }
		};
		await service.UpdateRace(race, updated, courses);

		//assert
		var updatedCourses = db.Set<Race>().Single().Courses.OrderBy(c => c.Distance).ToArray();
		Assert.Equal(2, updatedCourses.Length);
		Assert.Equal(2, updatedCourses[0].Distance);
		Assert.Equal(3, updatedCourses[1].Distance);
	}
}