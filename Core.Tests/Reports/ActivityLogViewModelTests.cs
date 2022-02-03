using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Reports;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Reports;

public class ActivityLogViewModelTests
{
	[Fact]
	public void TitleIsSimplifiedForAllCourses()
	{
		//arrange
		var vm = new ActivityLogViewModel();

		//act
		var title = vm.Title;

		//assert
		Assert.Equal("Activity Log", title);
	}

	[Fact]
	public void TitleContainsCourseNameWhenSet()
	{
		//arrange
		var vm = new ActivityLogViewModel { Course = new Course { Race = new Race { Name = "test course" } } };

		//act
		var title = vm.Title;

		//assert
		Assert.StartsWith("Activity Log", title);
		Assert.EndsWith("test course", title);
	}
}