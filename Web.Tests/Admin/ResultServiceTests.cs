using FLRC.Leaderboards.Data.Models;
using FLRC.Leaderboards.Web.Areas.Admin.Services;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Admin;

public sealed class ResultServiceTests
{
	[Fact]
	public async Task CanGetResult()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new ResultService(db);

		var id = Guid.NewGuid();
		await db.AddAsync(new Result { ID = id, Duration = TimeSpan.FromMilliseconds(1234567) });
		await db.SaveChangesAsync();

		//act
		var result = await service.Get(id);

		//assert
		Assert.Equal(20, result.Duration.Minutes);
	}

	[Fact]
	public async Task CanFindResultsForCourse()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new ResultService(db);

		var id = Guid.NewGuid();
		var course = new Course { ID = id, Distance = 5, Units = "km" };
		var result = new Result { ID = Guid.NewGuid(), Course = course, Duration = TimeSpan.FromMilliseconds(1234567) };
		await db.AddAsync(result);
		await db.SaveChangesAsync();

		//act
		var results = await service.Find(id);

		//assert
		Assert.Equal(20, results.Single().Duration.Minutes);
	}

	[Fact]
	public async Task CanImportResults()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new ResultService(db);

		var results = new[]
		{
			new Result { ID = Guid.NewGuid(), Duration = TimeSpan.FromMilliseconds(1234567) },
			new Result { ID = Guid.NewGuid(), Duration = TimeSpan.FromMilliseconds(2345678) }
		};

		//act
		await service.Import(results);

		//assert
		Assert.Equal(2, db.Set<Result>().Count());
	}
}