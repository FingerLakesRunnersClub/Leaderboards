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

	[Fact]
	public async Task ImportDoesNotAddDuplicateResults()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new ResultService(db);

		var c1 = Guid.NewGuid();
		var c2 = Guid.NewGuid();
		var a1 =  Guid.NewGuid();
		var a2 =  Guid.NewGuid();
		var t1 = DateTime.Parse("2026-02-13 11:01:00 AM");
		var t2 = DateTime.Parse("2026-02-13 11:01:01 AM");
		var d1 = TimeSpan.FromMilliseconds(1234567);
		var d2 = TimeSpan.FromMilliseconds(2345678);
		await db.AddRangeAsync(
			new Result { ID = Guid.NewGuid(), CourseID = c1, AthleteID = a1, StartTime = t1, Duration = d1 },
			new Result { ID = Guid.NewGuid(), CourseID = c1, AthleteID = a1, StartTime = t1, Duration = d2 },
			new Result { ID = Guid.NewGuid(), CourseID = c1, AthleteID = a2, StartTime = t2, Duration = d1 },
			new Result { ID = Guid.NewGuid(), CourseID = c1, AthleteID = a2, StartTime = t2, Duration = d2 },
			new Result { ID = Guid.NewGuid(), CourseID = c2, AthleteID = a1, StartTime = t1, Duration = d1 },
			new Result { ID = Guid.NewGuid(), CourseID = c2, AthleteID = a1, StartTime = t1, Duration = d2 },
			new Result { ID = Guid.NewGuid(), CourseID = c2, AthleteID = a2, StartTime = t2, Duration = d1 },
			new Result { ID = Guid.NewGuid(), CourseID = c2, AthleteID = a2, StartTime = t2, Duration = d2 }
		);
		await db.SaveChangesAsync();

		var results = new[]
		{
			new Result { ID = Guid.NewGuid(), CourseID = c1, AthleteID = a1, StartTime = t1, Duration = d1 },
			new Result { ID = Guid.NewGuid(), CourseID = c1, AthleteID = a1, StartTime = t1, Duration = d2 },
			new Result { ID = Guid.NewGuid(), CourseID = c1, AthleteID = a1, StartTime = t2, Duration = d1 },
			new Result { ID = Guid.NewGuid(), CourseID = c1, AthleteID = a1, StartTime = t2, Duration = d2 },
			new Result { ID = Guid.NewGuid(), CourseID = c1, AthleteID = a2, StartTime = t1, Duration = d1 },
			new Result { ID = Guid.NewGuid(), CourseID = c1, AthleteID = a2, StartTime = t1, Duration = d2 },
			new Result { ID = Guid.NewGuid(), CourseID = c1, AthleteID = a2, StartTime = t2, Duration = d1 },
			new Result { ID = Guid.NewGuid(), CourseID = c1, AthleteID = a2, StartTime = t2, Duration = d2 },
			new Result { ID = Guid.NewGuid(), CourseID = c2, AthleteID = a1, StartTime = t1, Duration = d1 },
			new Result { ID = Guid.NewGuid(), CourseID = c2, AthleteID = a1, StartTime = t1, Duration = d2 },
			new Result { ID = Guid.NewGuid(), CourseID = c2, AthleteID = a1, StartTime = t2, Duration = d1 },
			new Result { ID = Guid.NewGuid(), CourseID = c2, AthleteID = a1, StartTime = t2, Duration = d2 },
			new Result { ID = Guid.NewGuid(), CourseID = c2, AthleteID = a2, StartTime = t1, Duration = d1 },
			new Result { ID = Guid.NewGuid(), CourseID = c2, AthleteID = a2, StartTime = t1, Duration = d2 },
			new Result { ID = Guid.NewGuid(), CourseID = c2, AthleteID = a2, StartTime = t2, Duration = d1 },
			new Result { ID = Guid.NewGuid(), CourseID = c2, AthleteID = a2, StartTime = t2, Duration = d2 }
		};

		//act
		await service.Import(results);

		//assert
		Assert.Equal(16, db.Set<Result>().Count());
	}
}