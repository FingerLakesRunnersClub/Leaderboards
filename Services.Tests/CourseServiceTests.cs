using FLRC.Leaderboards.Model;
using Xunit;

namespace FLRC.Leaderboards.Services.Tests;

public sealed class CourseServiceTests
{
	[Fact]
	public async Task CanGetAllCourses()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new CourseService(db);

		var race = new Race { Name = "Test", Type = "Trail", Description = "test" };
		await db.AddRangeAsync(
			new Course { ID = Guid.NewGuid(), Race = race, Distance = 5, Units = "km" },
			new Course { ID = Guid.NewGuid(), Race = race, Distance = 10, Units = "km" },
			new Course { ID = Guid.NewGuid(), Race = race, Distance = 15, Units = "km" }
			);
		await db.SaveChangesAsync();

		//act
		var courses = await service.All();

		//assert
		Assert.Equal(3, courses.Length);
	}

	[Fact]
	public async Task CanGetSomeCourses()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new CourseService(db);

		var race = new Race { Name = "Test", Type = "Trail", Description = "test" };
		var id1 = Guid.NewGuid();
		var id2 = Guid.NewGuid();
		var id3 = Guid.NewGuid();
		await db.AddRangeAsync(
			new Course { ID = id1, Race = race, Distance = 5, Units = "km" },
			new Course { ID = id2, Race = race, Distance = 10, Units = "km" },
			new Course { ID = id3, Race = race, Distance = 15, Units = "km" }
			);
		await db.SaveChangesAsync();

		//act
		var courses = await service.GetCourses([id1, id3]);

		//assert
		Assert.Equal(2, courses.Length);
	}

	[Fact]
	public async Task CanGetCourse()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new CourseService(db);

		var id = Guid.NewGuid();
		var race = new Race { Name = "Test", Type = "Trail", Description = "test" };
		await db.AddAsync(new Course { ID = id, Race = race, Distance = 5, Units = "km" });
		await db.SaveChangesAsync();

		//act
		var course = await service.Get(id);

		//assert
		Assert.Equal("Test", course.Race.Name);
	}
}