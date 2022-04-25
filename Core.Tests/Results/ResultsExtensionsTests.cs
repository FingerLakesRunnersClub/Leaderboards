using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Results;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Results;

public class ResultsExtensionsTests
{
	[Fact]
	public void PerformsNoFilteringByDefault()
	{
		//arrange
		var results = CourseData.Results;
		var filter = new Filter();

		//act
		var filtered = results.Filter(filter).ToArray();

		//assert
		Assert.Equal(results.Count, filtered.Count());
	}

	[Fact]
	public void CanFilterResultsByCategory()
	{
		//arrange
		var results = CourseData.Results;
		var filter = new Filter(Category.M);

		//act
		var filtered = results.Filter(filter).ToArray();

		//assert
		Assert.True(filtered.Any());
		Assert.True(filtered.All(r => r.Athlete.Category == Category.M));
	}

	[Fact]
	public void CanFilterResultsByCategoryAndAgeGroup()
	{
		//arrange
		var results = CourseData.Results;
		var filter = new Filter(Category.M, Athlete.Teams[2]);

		//act
		var filtered = results.Filter(filter).ToArray();

		//assert
		Assert.True(filtered.Any());
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
		var filtered = results.Filter(filter).ToArray();

		//assert
		Assert.True(filtered.Any());
		Assert.True(filtered.All(r => r.Athlete.Category == Category.M && r.AgeOnDayOfRun.ToString().StartsWith('7')));
	}
}