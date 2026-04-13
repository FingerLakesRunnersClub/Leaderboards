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
	public async Task ButtonDisplaysWhenSelfTimingSetAndLoggedInAndActiveIteration()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var seriesService = Substitute.For<ISeriesService>();
		var contextProvider = Substitute.For<IContextProvider>();

		var component = new AddResultButton(authService, seriesService, contextProvider);

		authService.IsLoggedIn().Returns(true);
		seriesService.Find(Arg.Any<string>()).Returns(new Series { Features = [new Feature { SeriesID = Guid.NewGuid(), Key = nameof(FeatureSet.SelfTiming), Value = true }] });

		//act
		var course = new Course
		{
			ID = Guid.NewGuid(),
			Race = new Race
			{
				Iterations =
				[
					new Iteration { StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)), EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)) }
				]
			}
		};
		var result = await component.InvokeAsync(course);

		//assert
		Assert.IsType<ViewViewComponentResult>(result);
	}

	[Fact]
	public async Task ButtonNotShownWhenNotSelfTiming()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var seriesService = Substitute.For<ISeriesService>();
		var contextProvider = Substitute.For<IContextProvider>();

		var component = new AddResultButton(authService, seriesService, contextProvider);

		authService.IsLoggedIn().Returns(true);
		seriesService.Find(Arg.Any<string>()).Returns(new Series { Features = [new Feature { SeriesID = Guid.NewGuid(), Key = nameof(FeatureSet.SelfTiming), Value = false }] });

		//act
		var course = new Course
		{
			ID = Guid.NewGuid(),
			Race = new Race
			{
				Iterations =
				[
					new Iteration { StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)), EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)) }
				]
			}
		};
		var result = await component.InvokeAsync(course);

		//assert
		Assert.IsType<ContentViewComponentResult>(result);
	}

	[Fact]
	public async Task ButtonNotShownWhenNotLoggedIn()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var seriesService = Substitute.For<ISeriesService>();
		var contextProvider = Substitute.For<IContextProvider>();

		var component = new AddResultButton(authService, seriesService, contextProvider);

		authService.IsLoggedIn().Returns(false);
		seriesService.Find(Arg.Any<string>()).Returns(new Series { Features = [new Feature { SeriesID = Guid.NewGuid(), Key = nameof(FeatureSet.SelfTiming), Value = true }] });

		//act
		var course = new Course
		{
			ID = Guid.NewGuid(),
			Race = new Race
			{
				Iterations =
				[
					new Iteration { StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)), EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)) }
				]
			}
		};
		var result = await component.InvokeAsync(course);

		//assert
		Assert.IsType<ContentViewComponentResult>(result);
	}

	[Fact]
	public async Task ButtonNotShownWhenNoActiveIteration()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var seriesService = Substitute.For<ISeriesService>();
		var contextProvider = Substitute.For<IContextProvider>();

		var component = new AddResultButton(authService, seriesService, contextProvider);

		authService.IsLoggedIn().Returns(true);
		seriesService.Find(Arg.Any<string>()).Returns(new Series { Features = [new Feature { SeriesID = Guid.NewGuid(), Key = nameof(FeatureSet.SelfTiming), Value = true }] });

		//act
		var course = new Course
		{
			ID = Guid.NewGuid(),
			Race = new Race
			{
				Iterations =
				[
					new Iteration { StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-2)), EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)) }
				]
			}
		};
		var result = await component.InvokeAsync(course);

		//assert
		Assert.IsType<ContentViewComponentResult>(result);
	}
}