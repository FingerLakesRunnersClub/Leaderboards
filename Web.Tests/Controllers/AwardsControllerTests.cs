using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Controllers;
using FLRC.Leaderboards.Web.Services;
using FLRC.Leaderboards.Web.ViewModels;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Controllers;

public sealed class AwardsControllerTests
{
	[Fact]
	public async Task GetsAwardsFromCalculator()
	{
		//arrange
		var iterationManager = Substitute.For<IIterationManager>();
		var calculator = Substitute.For<IAwardsCalculator>();
		var controller = new AwardsController(iterationManager, calculator);

		var awards = new Dictionary<Athlete, Award[]>()
		{
			{ OverallData.Athlete1, [new Award { Value = 1 }] },
			{ OverallData.Athlete2, [new Award { Value = 2 }, new Award { Value = 3 }] },
			{ OverallData.Athlete3, [new Award { Value = 4 }, new Award { Value = 5 }, new Award { Value = 6 }] },
		};
		calculator.GetAwards(Arg.Any<Iteration>()).Returns(awards);

		//act
		var response = await controller.Index();

		//assert
		var vm = (ViewModel<Dictionary<Athlete,Award[]>>) response.Model!;
		var athletes = vm.Data.Count;
		var count = vm.Data.Sum(athlete => athlete.Value.Length);
		var amount = vm.Data.Sum(athlete => athlete.Value.Sum(award => award.Value));
		Assert.Equal(3, athletes);
		Assert.Equal(6, count);
		Assert.Equal(21, amount);
	}
}