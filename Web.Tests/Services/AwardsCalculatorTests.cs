using FLRC.Leaderboards.Core.Tests;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Web.Services;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Services;

public sealed class AwardsCalculatorTests
{
	[Fact]
	public void CalculatesCorrectAwards()
	{
		//arrange
		var starCalculator = Substitute.For<ICommunityStarCalculator>();
		var overall = new OverallResultsCalculator(starCalculator);
		var calculator = new AwardsCalculator(overall, TestHelpers.Config);

		starCalculator.GetStars(OverallData.Result1, Arg.Any<Result[]>(), Arg.Any<IList<CommunityStars>>()).Returns(new CommunityStars { Result = OverallData.Result1, GroupRun = true, StoryPost = true });
		starCalculator.GetStars(OverallData.Result2, Arg.Any<Result[]>(), Arg.Any<IList<CommunityStars>>()).Returns(new CommunityStars { Result = OverallData.Result2, GroupRun = false, StoryPost = true });
		starCalculator.GetStars(OverallData.Result3, Arg.Any<Result[]>(), Arg.Any<IList<CommunityStars>>()).Returns(new CommunityStars { Result = OverallData.Result3, GroupRun = false, StoryPost = true });
		starCalculator.GetStars(OverallData.Result4, Arg.Any<Result[]>(), Arg.Any<IList<CommunityStars>>()).Returns(new CommunityStars { Result = OverallData.Result4, GroupRun = false, StoryPost = false });
		starCalculator.GetStars(OverallData.Result5, Arg.Any<Result[]>(), Arg.Any<IList<CommunityStars>>()).Returns(new CommunityStars { Result = OverallData.Result5, GroupRun = false, StoryPost = false });
		starCalculator.GetStars(OverallData.Result6, Arg.Any<Result[]>(), Arg.Any<IList<CommunityStars>>()).Returns(new CommunityStars { Result = OverallData.Result6, GroupRun = false, StoryPost = true });
		starCalculator.GetStars(OverallData.Result7, Arg.Any<Result[]>(), Arg.Any<IList<CommunityStars>>()).Returns(new CommunityStars { Result = OverallData.Result7, GroupRun = false, StoryPost = true });
		starCalculator.GetStars(OverallData.Result8, Arg.Any<Result[]>(), Arg.Any<IList<CommunityStars>>()).Returns(new CommunityStars { Result = OverallData.Result8, GroupRun = true, StoryPost = true });
		starCalculator.GetStars(OverallData.Result9, Arg.Any<Result[]>(), Arg.Any<IList<CommunityStars>>()).Returns(new CommunityStars { Result = OverallData.Result9, GroupRun = true, StoryPost = true });
		starCalculator.GetStars(OverallData.Result10, Arg.Any<Result[]>(), Arg.Any<IList<CommunityStars>>()).Returns(new CommunityStars { Result = OverallData.Result10, GroupRun = false, StoryPost = false });

		//act
		var awards = calculator.GetAwards(OverallData.Iteration);

		//assert
		var athletes = awards.Count;
		var count = awards.Sum(athlete => athlete.Value.Length);
		var amount = awards.Sum(athlete => athlete.Value.Sum(award => award.Value));
		Assert.Equal(4, athletes);
		Assert.Equal(30, count);
		Assert.Equal(110, amount);

		var athlete1 = awards[OverallData.Athlete1].Select(a => a.Name).ToArray();
		Assert.Contains("1st Overall Points (M)", athlete1);
		Assert.Contains("2nd Overall Community", athlete1);

		var athlete2 = awards[OverallData.Athlete2].Select(a => a.Name).ToArray();
		Assert.Contains("2nd Overall Points (M)", athlete2);
		Assert.Contains("1st Overall Miles", athlete2);

		var athlete3 = awards[OverallData.Athlete3].Select(a => a.Name).ToArray();
		Assert.Contains("1st Overall Points (F)", athlete3);

		var athlete4 = awards[OverallData.Athlete4].Select(a => a.Name).ToArray();
		Assert.Contains("1st Overall Community", athlete4);
		Assert.Contains("2nd Overall Points (F)", athlete4);
		Assert.Contains("1st Overall Miles", athlete4);
	}

	[Fact]
	public void CourseAgeGroupWinnersSkipCategoryWinners()
	{
		//arrange
		var overall = new OverallResultsCalculator(Substitute.For<ICommunityStarCalculator>());
		var calculator = new AwardsCalculator(overall, TestHelpers.Config);

		var a1 = new Athlete { ID = Guid.NewGuid(), Name = "A1", Category = 'F', DateOfBirth = DateOnly.Parse("7/1/1990") };
		var a2 = new Athlete { ID = Guid.NewGuid(), Name = "A2", Category = 'F', DateOfBirth = DateOnly.Parse("7/1/1990") };
		var a3 = new Athlete { ID = Guid.NewGuid(), Name = "A3", Category = 'F', DateOfBirth = DateOnly.Parse("7/1/1990") };
		var a4 = new Athlete { ID = Guid.NewGuid(), Name = "A4", Category = 'F', DateOfBirth = DateOnly.Parse("7/1/1990") };

		var course = new Course
		{
			Race = new Race { Name = "Mile", Type = "Road" },
			Distance = 1,
			Units = "mi"
		};
		var results = new[]
		{
			new Result { Course = course, Athlete = a1, StartTime = DateTime.Parse("6/1/2020"), Duration = TimeSpan.FromMinutes(6) },
			new Result { Course = course, Athlete = a1, StartTime = DateTime.Parse("8/1/2020"), Duration = TimeSpan.FromMinutes(5) },
			new Result { Course = course, Athlete = a2, StartTime = DateTime.Parse("6/1/2020"), Duration = TimeSpan.FromMinutes(6) },
			new Result { Course = course, Athlete = a3, StartTime = DateTime.Parse("6/1/2020"), Duration = TimeSpan.FromMinutes(5) },
			new Result { Course = course, Athlete = a4, StartTime = DateTime.Parse("8/1/2020"), Duration = TimeSpan.FromMinutes(6) }
		};
		foreach (var result in results)
			course.Results.Add(result);

		var iteration = new Iteration
		{
			Races = [new Race { Courses = [course] }],
			Challenges = [new Challenge { IsOfficial = true, IsPrimary = true, Courses = [course] }],
			StartDate = new DateOnly(2020, 1, 1)
		};

		//act
		var awards = calculator.GetAwards(iteration);

		//assert
		Assert.Contains("Mile Fastest (F)", awards[a1].Select(a => a.Name));
		Assert.Contains("Mile 1–29 (F)", awards[a2].Select(a => a.Name));
		Assert.Contains("Mile Fastest (F)", awards[a3].Select(a => a.Name));
		Assert.Contains("Mile 30–39 (F)", awards[a4].Select(a => a.Name));
	}
}