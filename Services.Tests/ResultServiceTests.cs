using FLRC.Leaderboards.Model;
using Xunit;

namespace FLRC.Leaderboards.Services.Tests;

public sealed class ResultServiceTests
{
	[Fact]
	public async Task CanGetAllResults()
	{
		var db = TestHelpers.CreateDB();
		var service = new ResultService(db);

		var athlete = new Athlete { ID = Guid.NewGuid(), Name = "Test" };
		await db.AddRangeAsync(
			new Result { ID = Guid.NewGuid(), Duration = TimeSpan.FromMilliseconds(1234567), Athlete = athlete },
			new Result { ID = Guid.NewGuid(), Duration = TimeSpan.FromMilliseconds(2345678), Athlete = athlete }
		);
		await db.SaveChangesAsync();

		//act
		var results = await service.All();

		//assert
		Assert.Equal(2, results.Length);
	}

	[Fact]
	public async Task CanGetResult()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new ResultService(db);

		var id = Guid.NewGuid();
		var athlete = new Athlete { ID = Guid.NewGuid(), Name = "Test" };
		await db.AddAsync(new Result { ID = id, Duration = TimeSpan.FromMilliseconds(1234567), Athlete = athlete });
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
		var athlete = new Athlete { ID = Guid.NewGuid(), Name = "Test" };
		var result = new Result { ID = Guid.NewGuid(), Course = course, Duration = TimeSpan.FromMilliseconds(1234567), Athlete = athlete };
		await db.AddAsync(result);
		await db.SaveChangesAsync();

		//act
		var results = await service.Find(id);

		//assert
		Assert.Equal(20, results.Single().Duration.Minutes);
	}

	[Fact]
	public async Task CanFindDuplicateWhenAllFieldsMatch()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new ResultService(db);

		var course = new Course { ID = Guid.NewGuid(), Distance = 5, Units = "km" };
		var athlete = new Athlete { ID = Guid.NewGuid(), Name = "Test" };
		var result = new Result
		{
			ID = Guid.NewGuid(),
			Course = course,
			Athlete = athlete,
			StartTime = DateTime.Parse("1/2/2023 4:56pm"),
			Duration = TimeSpan.FromMilliseconds(1234567)
		};
		await db.AddAsync(result);
		await db.SaveChangesAsync();

		var newResult = new Result
		{
			ID = Guid.NewGuid(),
			Course = course,
			Athlete = athlete,
			StartTime = result.StartTime,
			Duration = result.Duration
		};

		//act
		var duplicates = await service.FindDuplicates(newResult);

		//assert
		Assert.NotEmpty(duplicates);
	}

	[Fact]
	public async Task EditingSelfIsNotDuplicate()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new ResultService(db);

		var course = new Course { ID = Guid.NewGuid(), Distance = 5, Units = "km" };
		var athlete = new Athlete { ID = Guid.NewGuid(), Name = "Test" };
		var result = new Result
		{
			ID = Guid.NewGuid(),
			Course = course,
			Athlete = athlete,
			StartTime = DateTime.Parse("1/2/2023 4:56pm"),
			Duration = TimeSpan.FromMilliseconds(1234567)
		};
		await db.AddAsync(result);
		await db.SaveChangesAsync();

		var newResult = new Result
		{
			ID = result.ID,
			Course = course,
			Athlete = athlete,
			StartTime = result.StartTime,
			Duration = result.Duration
		};

		//act
		var duplicates = await service.FindDuplicates(newResult);

		//assert
		Assert.Empty(duplicates);
	}

	[Fact]
	public async Task MismatchedAthleteIsNotDuplicate()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new ResultService(db);

		var course = new Course { ID = Guid.NewGuid(), Distance = 5, Units = "km" };
		var athlete = new Athlete { ID = Guid.NewGuid(), Name = "Test" };
		var result = new Result
		{
			ID = Guid.NewGuid(),
			Course = course,
			Athlete = athlete,
			StartTime = DateTime.Parse("1/2/2023 4:56pm"),
			Duration = TimeSpan.FromMilliseconds(1234567)
		};
		await db.AddAsync(result);
		await db.SaveChangesAsync();

		var newResult = new Result
		{
			ID = result.ID,
			Course = course,
			Athlete = new Athlete(),
			StartTime = result.StartTime,
			Duration = result.Duration
		};

		//act
		var duplicates = await service.FindDuplicates(newResult);

		//assert
		Assert.Empty(duplicates);
	}

	[Fact]
	public async Task MismatchedStartTimeIsNotDuplicate()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new ResultService(db);

		var course = new Course { ID = Guid.NewGuid(), Distance = 5, Units = "km" };
		var athlete = new Athlete { ID = Guid.NewGuid(), Name = "Test" };
		var result = new Result
		{
			ID = Guid.NewGuid(),
			Course = course,
			Athlete = athlete,
			StartTime = DateTime.Parse("1/2/2023 4:56pm"),
			Duration = TimeSpan.FromMilliseconds(1234567)
		};
		await db.AddAsync(result);
		await db.SaveChangesAsync();

		var newResult = new Result
		{
			ID = result.ID,
			Course = course,
			Athlete = athlete,
			StartTime = result.StartTime.AddMinutes(1),
			Duration = result.Duration
		};

		//act
		var duplicates = await service.FindDuplicates(newResult);

		//assert
		Assert.Empty(duplicates);
	}

	[Fact]
	public async Task MismatchedDurationIsNotDuplicate()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new ResultService(db);

		var course = new Course { ID = Guid.NewGuid(), Distance = 5, Units = "km" };
		var athlete = new Athlete { ID = Guid.NewGuid(), Name = "Test" };
		var result = new Result
		{
			ID = Guid.NewGuid(),
			Course = course,
			Athlete = athlete,
			StartTime = DateTime.Parse("1/2/2023 4:56pm"),
			Duration = TimeSpan.FromMilliseconds(1234567)
		};
		await db.AddAsync(result);
		await db.SaveChangesAsync();

		var newResult = new Result
		{
			ID = result.ID,
			Course = course,
			Athlete = athlete,
			StartTime = result.StartTime,
			Duration = result.Duration.Add(TimeSpan.FromSeconds(1))
		};

		//act
		var duplicates = await service.FindDuplicates(newResult);

		//assert
		Assert.Empty(duplicates);
	}

	[Fact]
	public async Task MismatchedCourseCanBeDuplicate()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new ResultService(db);

		var course = new Course { ID = Guid.NewGuid(), Distance = 5, Units = "km" };
		var athlete = new Athlete { ID = Guid.NewGuid(), Name = "Test" };
		var result = new Result
		{
			ID = Guid.NewGuid(),
			Course = course,
			Athlete = athlete,
			StartTime = DateTime.Parse("1/2/2023 4:56pm"),
			Duration = TimeSpan.FromMilliseconds(1234567)
		};
		await db.AddAsync(result);
		await db.SaveChangesAsync();

		var newResult = new Result
		{
			ID = Guid.NewGuid(),
			Course = new Course(),
			Athlete = athlete,
			StartTime = result.StartTime,
			Duration = result.Duration
		};

		//act
		var duplicates = await service.FindDuplicates(newResult);

		//assert
		Assert.NotEmpty(duplicates);
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
		var a1 = Guid.NewGuid();
		var a2 = Guid.NewGuid();
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

	[Fact]
	public async Task CanAddResult()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new ResultService(db);

		var result = new Result();

		//act
		await service.Add(result);

		//assert
		Assert.Equal(1, db.Set<Result>().Count());
	}

	[Fact]
	public async Task CanEditResult()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new ResultService(db);

		var result = new Result
		{
			ID = Guid.NewGuid(),
			StartTime = new DateTime(2000, 1, 1),
			Duration = new TimeSpan(1, 2, 3)
		};
		await db.AddAsync(result);
		await db.SaveChangesAsync();

		var updated = new Result
		{
			ID = Guid.NewGuid(),
			StartTime = new DateTime(2001, 2, 3),
			Duration = new TimeSpan(2, 3, 4)
		};

		//act
		await service.Update(result, updated);

		//assert
		var newResult = db.Set<Result>().Single();
		Assert.Equal(new DateTime(2001, 2, 3), newResult.StartTime);
		Assert.Equal(new TimeSpan(2, 3, 4), newResult.Duration);
	}

	[Fact]
	public async Task CanDeleteResult()
	{
		//arrange
		var db = TestHelpers.CreateDB();
		var service = new ResultService(db);

		var result = new Result
		{
			ID = Guid.NewGuid(),
			StartTime = new DateTime(2000, 1, 1),
			Duration = new TimeSpan(1, 2, 3)
		};
		await db.AddAsync(result);
		await db.SaveChangesAsync();

		//act
		await service.Delete(result);

		//assert
		Assert.Empty(db.Set<Result>());
	}
}