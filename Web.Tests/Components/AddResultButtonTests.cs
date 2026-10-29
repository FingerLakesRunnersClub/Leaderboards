using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Components;
using FLRC.Leaderboards.Web.Services;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Components;

public sealed class AddResultButtonTests
{
	[Fact]
	public async Task ButtonDisplaysWhenAllConditionsMet()
	{
		//arrange
		var iterationManager = Substitute.For<IIterationManager>();
		var authService = Substitute.For<IAuthService>();
		var sessionManager = Substitute.For<ISessionManager>();

		var component = new AddResultButton(iterationManager, authService, sessionManager);

		var iteration = new Iteration
		{
			StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),
			EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
			Series = new Series { Features = [new Feature { SeriesID = Guid.NewGuid(), Key = nameof(FeatureSet.SelfTiming), Value = true }] }
		};
		iterationManager.ActiveIteration().Returns(iteration);

		authService.IsLoggedIn().Returns(true);
		sessionManager.GetCurrentAthlete().Returns(new Athlete { Registrations = [iteration] });

		var course = new Course { ID = Guid.NewGuid(), Race = new Race { Iterations = [iteration] } };

		//act
		var result = await component.InvokeAsync(course);

		//assert
		Assert.IsType<ViewViewComponentResult>(result);
	}

	[Fact]
	public async Task ButtonNotShownWhenNotSelfTiming()
	{
		//arrange
		var iterationManager = Substitute.For<IIterationManager>();
		var authService = Substitute.For<IAuthService>();
		var sessionManager = Substitute.For<ISessionManager>();

		var component = new AddResultButton(iterationManager, authService, sessionManager);

		var iteration = new Iteration
		{
			StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),
			EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
			Series = new Series { Features = [new Feature { SeriesID = Guid.NewGuid(), Key = nameof(FeatureSet.SelfTiming), Value = false }] }
		};
		iterationManager.ActiveIteration().Returns(iteration);

		authService.IsLoggedIn().Returns(true);
		sessionManager.GetCurrentAthlete().Returns(new Athlete { Registrations = [iteration] });

		var course = new Course { ID = Guid.NewGuid(), Race = new Race { Iterations = [iteration] } };

		//act
		var result = await component.InvokeAsync(course);

		//assert
		Assert.IsType<ContentViewComponentResult>(result);
	}

	[Fact]
	public async Task ButtonNotShownWhenNotLoggedIn()
	{
		//arrange
		var iterationManager = Substitute.For<IIterationManager>();
		var authService = Substitute.For<IAuthService>();
		var sessionManager = Substitute.For<ISessionManager>();

		var component = new AddResultButton(iterationManager, authService, sessionManager);

		var iteration = new Iteration
		{
			StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),
			EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
			Series = new Series { Features = [new Feature { SeriesID = Guid.NewGuid(), Key = nameof(FeatureSet.SelfTiming), Value = true }] }
		};
		iterationManager.ActiveIteration().Returns(iteration);

		authService.IsLoggedIn().Returns(false);
		sessionManager.GetCurrentAthlete().Returns(new Athlete { Registrations = [iteration] });

		var course = new Course { ID = Guid.NewGuid(), Race = new Race { Iterations = [iteration] } };

		//act
		var result = await component.InvokeAsync(course);

		//assert
		Assert.IsType<ContentViewComponentResult>(result);
	}

	[Fact]
	public async Task ButtonNotShownWhenNoActiveIteration()
	{
		//arrange
		var iterationManager = Substitute.For<IIterationManager>();
		var authService = Substitute.For<IAuthService>();
		var sessionManager = Substitute.For<ISessionManager>();

		var component = new AddResultButton(iterationManager, authService, sessionManager);

		var iteration = new Iteration
		{
			StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-2)),
			EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),
			Series = new Series { Features = [new Feature { SeriesID = Guid.NewGuid(), Key = nameof(FeatureSet.SelfTiming), Value = true }] }
		};
		iterationManager.ActiveIteration().Returns(iteration);

		authService.IsLoggedIn().Returns(true);
		sessionManager.GetCurrentAthlete().Returns(new Athlete { Registrations = [iteration] });

		var course = new Course { ID = Guid.NewGuid(), Race = new Race { Iterations = [iteration] } };

		//act
		var result = await component.InvokeAsync(course);

		//assert
		Assert.IsType<ContentViewComponentResult>(result);
	}

	[Fact]
	public async Task ButtonNotShownWhenCourseNotInActiveIteration()
	{
		//arrange
		var iterationManager = Substitute.For<IIterationManager>();
		var authService = Substitute.For<IAuthService>();
		var sessionManager = Substitute.For<ISessionManager>();

		var component = new AddResultButton(iterationManager, authService, sessionManager);

		var iteration = new Iteration
		{
			StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),
			EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
			Series = new Series { Features = [new Feature { SeriesID = Guid.NewGuid(), Key = nameof(FeatureSet.SelfTiming), Value = true }] }
		};
		iterationManager.ActiveIteration().Returns(iteration);

		authService.IsLoggedIn().Returns(true);
		sessionManager.GetCurrentAthlete().Returns(new Athlete { Registrations = [iteration] });

		var course = new Course { ID = Guid.NewGuid(), Race = new Race { Iterations = [] } };

		//act
		var result = await component.InvokeAsync(course);

		//assert
		Assert.IsType<ContentViewComponentResult>(result);
	}
}