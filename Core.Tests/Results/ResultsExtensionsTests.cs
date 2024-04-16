using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Results;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Results;

public sealed class ResultsExtensionsTests
{
	[Fact]
	public void PerformsNoFilteringByDefault()
	{
		//arrange
		var results = CourseData.Results;
		var filter = new Filter();

		//act
		var filtered = results.Filter(filter);

		//assert
		Assert.Equal(results.Length, filtered.Length);
	}

	[Fact]
	public void CanFilterResultsByCategory()
	{
		//arrange
		var results = CourseData.Results;
		var filter = new Filter(Category.M);

		//act
		var filtered = results.Filter(filter);

		//assert
		Assert.NotEmpty(filtered);
		Assert.True(filtered.All(r => r.Athlete.Category == Category.M));
	}

	[Fact]
	public void CanFilterResultsByCategoryAndAgeGroup()
	{
		//arrange
		var results = CourseData.Results;
		var filter = new Filter(Category.M, Athlete.Teams[2]);

		//act
		var filtered = results.Filter(filter);

		//assert
		Assert.NotEmpty(filtered);
		Assert.True(filtered.All(r => r.Athlete.Category == Category.M && r.AgeOnDayOfRun.ToString().StartsWith('2')));
	}

	[Fact]
	public void CanFilterResultsByCategoryTopAgeGroup()
	{
		//arrange
		var results = CourseData.Results.ToList();
		results.Add(new Result
		{
			Athlete = new Athlete { DateOfBirth = new DateTime(1952, 1, 1), Category = Category.M },
			StartTime = new Date(new DateTime(2022, 4, 25))
		});
		var filter = new Filter(Category.M, Athlete.Teams[6]);

		//act
		var filtered = results.ToArray().Filter(filter);

		//assert
		Assert.NotEmpty(filtered);
		Assert.True(filtered.All(r => r.Athlete.Category == Category.M && r.AgeOnDayOfRun.ToString().StartsWith('7')));
	}
}