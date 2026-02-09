using FLRC.Leaderboards.Data.Models;
using FLRC.Leaderboards.Web.Areas.Admin.Services;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Admin;

public sealed class CourseServiceTests
{
	[Fact]
	public async Task CanGetCourse()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new CourseService(db);

		var id = Guid.NewGuid();
		await db.AddAsync(new Course { ID = id, Race = new Race { Name = "Test", Type = "Trail "}, Distance = 5, Units = "km" });
		await db.SaveChangesAsync();

		//act
		var course = await service.Get(id);

		//assert
		Assert.Equal("Test", course.Race.Name);
	}
}