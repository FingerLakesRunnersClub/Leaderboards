using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Leaders;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Results;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Leaders;

public class LeaderboardViewModelTests
{
	[Fact]
	public void TitleIsStatic()
	{
		//arrange
		var vm = new LeaderboardViewModel(new List<Course>(), LeaderboardResultType.Team);

		//act
		var title = vm.Title;

		//assert
		Assert.Equal("Leaderboard", title);
	}

	[Fact]
	public void TeamResultsContainsAllTables()
	{
		//arrange
		var vm = new LeaderboardViewModel(LeaderboardData.Courses, LeaderboardResultType.Team);

		//act
		var results = vm.CourseResults;

		//assert
		var result = results.First().Value.ToList();
		Assert.Equal("1–29", result.First(r => r.ResultType.Value == ResultType.Team && r.Title == "Age Grade").Rows.First().Name);
		Assert.Equal("30–39", result.First(r => r.ResultType.Value == ResultType.Team && r.Title == "Most Runs").Rows.First().Name);
		Assert.Equal("1–29", result.First(r => r.ResultType.Value == ResultType.Team && r.Title == "Total Points").Rows.First().Name);
	}

	[Fact]
	public void FResultsContainsAllTables()
	{
		//arrange
		var vm = new LeaderboardViewModel(LeaderboardData.Courses, LeaderboardResultType.F);

		//act
		var results = vm.CourseResults;

		//assert
		var result = results.First().Value.ToList();
		Assert.Equal("3:02:01", result.First(r => r.Category == Category.F && r.ResultType.Value == ResultType.Fastest).Rows.First().Value);
		Assert.StartsWith("4:", result.First(r => r.Category == Category.F && r.ResultType.Value == ResultType.BestAverage).Rows.First().Value);
		Assert.StartsWith("3", result.First(r => r.ResultType.Value == ResultType.MostRuns).Rows.First().Value);
	}

	[Fact]
	public void MResultsContainsAllTables()
	{
		//arrange
		var vm = new LeaderboardViewModel(LeaderboardData.Courses, LeaderboardResultType.M);

		//act
		var results = vm.CourseResults;

		//assert
		var result = results.First().Value.ToList();
		Assert.Equal("1:02:03", result.First(r => r.Category == Category.M && r.ResultType.Value == ResultType.Fastest).Rows.First().Value);
		Assert.StartsWith("2:", result.First(r => r.Category == Category.M && r.ResultType.Value == ResultType.BestAverage).Rows.First().Value);
		Assert.StartsWith("3", result.First(r => r.ResultType.Value == ResultType.MostRuns).Rows.First().Value);
	}

	[Fact]
	public void OverallResultsContainsAllTables()
	{
		//arrange
		var vm = new LeaderboardViewModel(LeaderboardData.Courses, LeaderboardResultType.Team);

		//act
		var results = vm.OverallResults.ToList();

		//assert
		Assert.Equal("A3", results.First(r => r.Title == "Most Points (F)").Rows.First().Name);
		Assert.Equal("A1", results.First(r => r.Title == "Most Points (M)").Rows.First().Name);
		Assert.Equal("A2", results.First(r => r.Title == "Most Miles").Rows.First().Name);
		Assert.Equal("1–29", results.First(r => r.Title == "Top Teams").Rows.First().Name);
	}
}