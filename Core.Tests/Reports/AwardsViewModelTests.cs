using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Reports;
using FLRC.Leaderboards.Core.Results;
using FLRC.Leaderboards.Core.Tests.Leaders;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Reports;

public sealed class AwardsViewModelTests
{
	[Fact]
	public void CalculatesCorrectAwards()
	{
		//arrange
		var results = LeaderboardData.Courses;

		//act
		var vm = new AwardsViewModel(TestHelpers.Config, results);

		//assert
		var athletes = vm.Awards.Count;
		var count = vm.Awards.Sum(athlete => athlete.Value.Length);
		var amount = vm.Awards.Sum(athlete => athlete.Value.Sum(award => award.Value));
		Assert.Equal(4, athletes);
		Assert.Equal(27, count);
		Assert.Equal(91, amount);
		Assert.Contains("1st Overall Points (M)", vm.Awards[LeaderboardData.Athlete1].Select(a => a.Name));
		Assert.Contains("2nd Overall Community", vm.Awards[LeaderboardData.Athlete1].Select(a => a.Name));
		Assert.Contains("2nd Overall Points (M)", vm.Awards[LeaderboardData.Athlete2].Select(a => a.Name));
		Assert.Contains("1st Overall Miles", vm.Awards[LeaderboardData.Athlete2].Select(a => a.Name));
		Assert.Contains("1st Overall Points (F)", vm.Awards[LeaderboardData.Athlete3].Select(a => a.Name));
		Assert.Contains("1st Overall Community", vm.Awards[LeaderboardData.Athlete4].Select(a => a.Name));
		Assert.Contains("2nd Overall Points (F)", vm.Awards[LeaderboardData.Athlete4].Select(a => a.Name));
		Assert.Contains("1st Overall Miles", vm.Awards[LeaderboardData.Athlete4].Select(a => a.Name));
	}

	[Fact]
	public void CourseAgeGroupWinnersSkipCategoryWinners()
	{
		//arrange
		var course = new Course { Race = new Race { Name = "Mile" }, Distance = new Distance("1 mile") };

		var a1 = new Athlete { ID = 1, Name = "A1", Category = Category.F, DateOfBirth = DateTime.Parse("7/1/1990") };
		var a2 = new Athlete { ID = 2, Name = "A2", Category = Category.F, DateOfBirth = DateTime.Parse("7/1/1990") };
		var a3 = new Athlete { ID = 3, Name = "A3", Category = Category.F, DateOfBirth = DateTime.Parse("7/1/1990") };
		var a4 = new Athlete { ID = 4, Name = "A4", Category = Category.F, DateOfBirth = DateTime.Parse("7/1/1990") };

		course.Results =
		[
			new Result { Course = course, Athlete = a1, StartTime = new Date(DateTime.Parse("6/1/2020")), Duration = new Time(TimeSpan.FromMinutes(6)) },
			new Result { Course = course, Athlete = a1, StartTime = new Date(DateTime.Parse("8/1/2020")), Duration = new Time(TimeSpan.FromMinutes(5)) },
			new Result { Course = course, Athlete = a2, StartTime = new Date(DateTime.Parse("6/1/2020")), Duration = new Time(TimeSpan.FromMinutes(6)) },
			new Result { Course = course, Athlete = a3, StartTime = new Date(DateTime.Parse("6/1/2020")), Duration = new Time(TimeSpan.FromMinutes(5)) },
			new Result { Course = course, Athlete = a4, StartTime = new Date(DateTime.Parse("8/1/2020")), Duration = new Time(TimeSpan.FromMinutes(6)) }
		];

		//act
		var vm = new AwardsViewModel(TestHelpers.Config, [course]);

		//assert
		var awards = vm.Awards;
		Assert.Contains("Mile Fastest (F)", awards[a1].Select(a => a.Name));
		Assert.Contains("Mile 1–29 (F)", awards[a2].Select(a => a.Name));
		Assert.Contains("Mile Fastest (F)", awards[a3].Select(a => a.Name));
		Assert.Contains("Mile 30–39 (F)", awards[a4].Select(a => a.Name));
	}
}