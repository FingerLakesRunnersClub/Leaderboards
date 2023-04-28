using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Results;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Results;

public sealed class CourseResultsViewModelTests
{
	[Fact]
	public void CanGetTitle()
	{
		//arrange
		var vm = new CourseResultsViewModel
		{
			ResultType = new FormattedResultType(ResultType.Fastest),
			Course = new Course { Race = new Race { Name = "Virgil Crest Ultramarathons" } }
		};

		//act
		var title = vm.Title;

		//assert
		Assert.Equal("Fastest — Virgil Crest Ultramarathons", title);
	}



	[Theory]
	[InlineData(ResultType.Fastest, "xl")]
	[InlineData(ResultType.BestAverage, "lg")]
	[InlineData(ResultType.MostRuns, "lg")]
	[InlineData(ResultType.Team, "xl")]
	public void ResponsiveBreakpointSet(ResultType resultType, string expected)
	{
		//arrange
		var vm = new CourseResultsViewModel { ResultType = new FormattedResultType(resultType) };

		//act
		var breakpoint = vm.ResponsiveBreakpoint;

		//assert
		Assert.Equal(expected, breakpoint);
	}
}