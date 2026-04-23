using System.Security.Claims;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Controllers;
using FLRC.Leaderboards.Web.Services;
using FLRC.Leaderboards.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using Xunit;
using Result = FLRC.Leaderboards.Model.Result;

namespace FLRC.Leaderboards.Web.Tests.Controllers;

public sealed class ResultsControllerTests
{
	[Fact]
	public async Task CanGetFastestResults()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var courseService = Substitute.For<ICourseService>();
		var iterationService = Substitute.For<IIterationService>();
		var resultService = Substitute.For<IResultService>();
		var adminService = Substitute.For<IAdminService>();

		var controller = new ResultsController(authService, athleteService, iterationManager, courseService, iterationService, resultService, adminService);

		var a1 = new Athlete { ID = Guid.NewGuid() };
		var a2 = new Athlete { ID = Guid.NewGuid() };
		var course = new Course { Distance = 10, Units = "km" };
		var results = new[]
		{
			new Result { Course = course, Athlete = a1, StartTime = new DateTime(2022, 4, 26), Duration = TimeSpan.Parse("2:34") },
			new Result { Course = course, Athlete = a2, StartTime = new DateTime(2022, 4, 26), Duration = TimeSpan.Parse("1:23") }
		};
		courseService.Get(Arg.Any<Guid>()).Returns(course);
		resultService.Find(Arg.Any<Guid>()).Returns(results);

		//act
		var response = await controller.Fastest(Guid.NewGuid());

		//assert
		var vm = response.Model as ViewModel<CourseResults<Time>>;
		Assert.Equal(a2.ID, vm!.Data.Results.First().Result.Athlete.ID);
	}

	[Fact]
	public async Task CanGetBestAverageResults()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var courseService = Substitute.For<ICourseService>();
		var iterationService = Substitute.For<IIterationService>();
		var resultService = Substitute.For<IResultService>();
		var adminService = Substitute.For<IAdminService>();

		var controller = new ResultsController(authService, athleteService, iterationManager, courseService, iterationService, resultService, adminService);

		var a1 = new Athlete { ID = Guid.NewGuid() };
		var a2 = new Athlete { ID = Guid.NewGuid() };
		var course = new Course { Distance = 10, Units = "km" };
		var results = new[]
		{
			new Result { Course = course, Athlete = a1, StartTime = new DateTime(2022, 4, 26), Duration = TimeSpan.Parse("2:34") },
			new Result { Course = course, Athlete = a2, StartTime = new DateTime(2022, 4, 26), Duration = TimeSpan.Parse("1:23") }
		};
		courseService.Get(Arg.Any<Guid>()).Returns(course);
		resultService.Find(Arg.Any<Guid>()).Returns(results);

		//act
		var response = await controller.Fastest(Guid.NewGuid());

		//assert
		var vm = response.Model as ViewModel<CourseResults<Time>>;
		Assert.Equal(a2.ID, vm!.Data.Results.First().Result.Athlete.ID);
	}

	[Fact]
	public async Task CanGetMostRuns()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var courseService = Substitute.For<ICourseService>();
		var iterationService = Substitute.For<IIterationService>();
		var resultService = Substitute.For<IResultService>();
		var adminService = Substitute.For<IAdminService>();

		var controller = new ResultsController(authService, athleteService, iterationManager, courseService, iterationService, resultService, adminService);

		var a1 = new Athlete { ID = Guid.NewGuid() };
		var a2 = new Athlete { ID = Guid.NewGuid() };
		var course = new Course { Distance = 10, Units = "km" };
		var results = new[]
		{
			new Result { Course = course, Athlete = a1, StartTime = new DateTime(2022, 4, 26), Duration = TimeSpan.Parse("2:34") },
			new Result { Course = course, Athlete = a2, StartTime = new DateTime(2022, 4, 26), Duration = TimeSpan.Parse("1:23") }
		};
		courseService.Get(Arg.Any<Guid>()).Returns(course);
		resultService.Find(Arg.Any<Guid>()).Returns(results);

		//act
		var response = await controller.Fastest(Guid.NewGuid());

		//assert
		var vm = response.Model as ViewModel<CourseResults<Time>>;
		Assert.Equal(a2.ID, vm!.Data.Results.First().Result.Athlete.ID);
	}

	[Fact]
	public async Task CanViewAddForm()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var courseService = Substitute.For<ICourseService>();
		var iterationService = Substitute.For<IIterationService>();
		var resultService = Substitute.For<IResultService>();
		var adminService = Substitute.For<IAdminService>();

		var controller = new ResultsController(authService, athleteService, iterationManager, courseService, iterationService, resultService, adminService);

		authService.GetCurrentUser().Returns(new ClaimsPrincipal(new ClaimsIdentity([new Claim("external_id", "123")])));

		//act
		var result = await controller.Add(Guid.NewGuid());

		//assert
		Assert.Equal("Form", result.ViewName);
	}

	[Fact]
	public async Task CanSubmitAddForm()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var courseService = Substitute.For<ICourseService>();
		var iterationService = Substitute.For<IIterationService>();
		var resultService = Substitute.For<IResultService>();
		var adminService = Substitute.For<IAdminService>();

		var controller = new ResultsController(authService, athleteService, iterationManager, courseService, iterationService, resultService, adminService);

		authService.GetCurrentUser().Returns(new ClaimsPrincipal(new ClaimsIdentity([new Claim("external_id", "123")])));
		athleteService.Find(Arg.Any<string>(), Arg.Any<string>()).Returns(new Athlete { DateOfBirth = new DateOnly(2000, 1, 1) });
		courseService.Get(Arg.Any<Guid>()).Returns(new Course { Distance = 10, Units = "km", Race = new Race { Type = "Road" } });

		var data = new Dictionary<string, StringValues>
		{
			{ "StartTime", "2026-04-03T14:15:16" },
			{ "Duration[h]", "1" },
			{ "Duration[m]", "23" },
			{ "Duration[s]", "45" }
		};
		var form = new FormCollection(data);

		//act
		await controller.Add(Guid.NewGuid(), form);

		//assert
		await resultService.Received().Add(Arg.Is<Result>(r => r.Duration == new TimeSpan(1, 23, 45)));
	}

	[Fact]
	public async Task CannotSubmitInvalidResultToAddForm()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var courseService = Substitute.For<ICourseService>();
		var iterationService = Substitute.For<IIterationService>();
		var resultService = Substitute.For<IResultService>();
		var adminService = Substitute.For<IAdminService>();

		var controller = new ResultsController(authService, athleteService, iterationManager, courseService, iterationService, resultService, adminService);

		authService.GetCurrentUser().Returns(new ClaimsPrincipal(new ClaimsIdentity([new Claim("external_id", "123")])));
		athleteService.Find(Arg.Any<string>(), Arg.Any<string>()).Returns(new Athlete { DateOfBirth = new DateOnly(2020, 1, 1) });
		courseService.Get(Arg.Any<Guid>()).Returns(new Course { Distance = 40, Units = "km", Race = new Race { Type = "Road" } });

		var data = new Dictionary<string, StringValues>
		{
			{ "StartTime", "2026-04-03T14:15:16" },
			{ "Duration[h]", "1" },
			{ "Duration[m]", "23" },
			{ "Duration[s]", "45" }
		};
		var form = new FormCollection(data);

		//act
		await controller.Add(Guid.NewGuid(), form);

		//assert
		await resultService.DidNotReceive().Add(Arg.Any<Result>());
	}

	[Fact]
	public async Task CanViewEditForm()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var courseService = Substitute.For<ICourseService>();
		var iterationService = Substitute.For<IIterationService>();
		var resultService = Substitute.For<IResultService>();
		var adminService = Substitute.For<IAdminService>();

		var controller = new ResultsController(authService, athleteService, iterationManager, courseService, iterationService, resultService, adminService);

		var user = new ClaimsPrincipal(new ClaimsIdentity([new Claim("external_id", "123")]));
		var athlete = new Athlete { ID = Guid.NewGuid(), DateOfBirth = new DateOnly(2000, 1, 1) };
		var course = new Course { ID = Guid.NewGuid(), Distance = 10, Units = "km", Race = new Race { Type = "Road" } };
		var result = new Result { ID = Guid.NewGuid(), Athlete = athlete, Course = course, Duration = new TimeSpan(1, 2, 3) };
		authService.GetCurrentUser().Returns(user);
		athleteService.Find(Arg.Any<string>(), Arg.Any<string>()).Returns(athlete);
		courseService.Get(Arg.Any<Guid>()).Returns(course);
		resultService.Get(Arg.Any<Guid>()).Returns(result);

		//act
		var response = await controller.Edit(Guid.NewGuid());

		//assert
		var view = response as ViewResult;
		Assert.Equal("Form", view!.ViewName);
	}

	[Fact]
	public async Task CannotViewEditFormForSomeoneElse()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var courseService = Substitute.For<ICourseService>();
		var iterationService = Substitute.For<IIterationService>();
		var resultService = Substitute.For<IResultService>();
		var adminService = Substitute.For<IAdminService>();

		var controller = new ResultsController(authService, athleteService, iterationManager, courseService, iterationService, resultService, adminService);

		var user = new ClaimsPrincipal(new ClaimsIdentity([new Claim("external_id", "123")]));
		var athlete1 = new Athlete { ID = Guid.NewGuid(), DateOfBirth = new DateOnly(2000, 1, 1) };
		var athlete2 = new Athlete { ID = Guid.NewGuid(), DateOfBirth = new DateOnly(2000, 1, 1) };
		var course = new Course { ID = Guid.NewGuid(), Distance = 10, Units = "km", Race = new Race { Type = "Road" } };
		var result = new Result { ID = Guid.NewGuid(), Athlete = athlete1, Course = course, Duration = new TimeSpan(1, 2, 3) };
		authService.GetCurrentUser().Returns(user);
		athleteService.Find(Arg.Any<string>(), Arg.Any<string>()).Returns(athlete2);
		courseService.Get(Arg.Any<Guid>()).Returns(course);
		resultService.Get(Arg.Any<Guid>()).Returns(result);

		//act
		var response = await controller.Edit(Guid.NewGuid());

		//assert
		Assert.IsType<ForbidResult>(response);
	}

	[Fact]
	public async Task AdminCanViewEditFormForSomeoneElse()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var courseService = Substitute.For<ICourseService>();
		var iterationService = Substitute.For<IIterationService>();
		var resultService = Substitute.For<IResultService>();
		var adminService = Substitute.For<IAdminService>();

		var controller = new ResultsController(authService, athleteService, iterationManager, courseService, iterationService, resultService, adminService);

		var user = new ClaimsPrincipal(new ClaimsIdentity([new Claim("external_id", "123")]));
		var athlete1 = new Athlete { ID = Guid.NewGuid(), DateOfBirth = new DateOnly(2000, 1, 1) };
		var athlete2 = new Athlete { ID = Guid.NewGuid(), DateOfBirth = new DateOnly(2000, 1, 1) };
		var course = new Course { ID = Guid.NewGuid(), Distance = 10, Units = "km", Race = new Race { Type = "Road" } };
		var result = new Result { ID = Guid.NewGuid(), Athlete = athlete1, Course = course, Duration = new TimeSpan(1, 2, 3) };
		authService.GetCurrentUser().Returns(user);
		athleteService.Find(Arg.Any<string>(), Arg.Any<string>()).Returns(athlete2);
		courseService.Get(Arg.Any<Guid>()).Returns(course);
		resultService.Get(Arg.Any<Guid>()).Returns(result);
		adminService.Verify(Arg.Any<Guid>()).Returns(true);

		//act
		var response = await controller.Edit(Guid.NewGuid());

		//assert
		var view = response as ViewResult;
		Assert.Equal("Form", view!.ViewName);
	}

	[Fact]
	public async Task CanSubmitEditForm()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var courseService = Substitute.For<ICourseService>();
		var iterationService = Substitute.For<IIterationService>();
		var resultService = Substitute.For<IResultService>();
		var adminService = Substitute.For<IAdminService>();

		var controller = new ResultsController(authService, athleteService, iterationManager, courseService, iterationService, resultService, adminService);

		var user = new ClaimsPrincipal(new ClaimsIdentity([new Claim("external_id", "123")]));
		var athlete = new Athlete { ID = Guid.NewGuid(), DateOfBirth = new DateOnly(2000, 1, 1) };
		var course = new Course { ID = Guid.NewGuid(), Distance = 10, Units = "km", Race = new Race { Type = "Road" } };
		var result = new Result { ID = Guid.NewGuid(), Athlete = athlete, Course = course, Duration = new TimeSpan(1, 2, 3) };
		authService.GetCurrentUser().Returns(user);
		athleteService.Find(Arg.Any<string>(), Arg.Any<string>()).Returns(athlete);
		courseService.Get(Arg.Any<Guid>()).Returns(course);
		resultService.Get(Arg.Any<Guid>()).Returns(result);

		var data = new Dictionary<string, StringValues>
		{
			{ "StartTime", "2026-04-03T14:15:16" },
			{ "Duration[h]", "1" },
			{ "Duration[m]", "23" },
			{ "Duration[s]", "45" }
		};
		var form = new FormCollection(data);

		//act
		await controller.Edit(Guid.NewGuid(), form);

		//assert
		await resultService.Received().Update(Arg.Any<Result>(), Arg.Is<Result>(r => r.Duration == new TimeSpan(1, 23, 45)));
	}

	[Fact]
	public async Task CannotSubmitEditFormForSomeoneElse()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var courseService = Substitute.For<ICourseService>();
		var iterationService = Substitute.For<IIterationService>();
		var resultService = Substitute.For<IResultService>();
		var adminService = Substitute.For<IAdminService>();

		var controller = new ResultsController(authService, athleteService, iterationManager, courseService, iterationService, resultService, adminService);

		var user = new ClaimsPrincipal(new ClaimsIdentity([new Claim("external_id", "123")]));
		var athlete1 = new Athlete { ID = Guid.NewGuid(), DateOfBirth = new DateOnly(2000, 1, 1) };
		var athlete2 = new Athlete { ID = Guid.NewGuid(), DateOfBirth = new DateOnly(2000, 1, 1) };
		var course = new Course { ID = Guid.NewGuid(), Distance = 10, Units = "km", Race = new Race { Type = "Road" } };
		var result = new Result { ID = Guid.NewGuid(), Athlete = athlete1, Course = course, Duration = new TimeSpan(1, 2, 3) };
		authService.GetCurrentUser().Returns(user);
		athleteService.Find(Arg.Any<string>(), Arg.Any<string>()).Returns(athlete2);
		courseService.Get(Arg.Any<Guid>()).Returns(course);
		resultService.Get(Arg.Any<Guid>()).Returns(result);

		var data = new Dictionary<string, StringValues>
		{
			{ "StartTime", "2026-04-03T14:15:16" },
			{ "Duration[h]", "1" },
			{ "Duration[m]", "23" },
			{ "Duration[s]", "45" }
		};
		var form = new FormCollection(data);

		//act
		var response = await controller.Edit(Guid.NewGuid(), form);

		//assert
		Assert.IsType<ForbidResult>(response);
	}

	[Fact]
	public async Task AdminCanSubmitEditFormForSomeoneElse()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var courseService = Substitute.For<ICourseService>();
		var iterationService = Substitute.For<IIterationService>();
		var resultService = Substitute.For<IResultService>();
		var adminService = Substitute.For<IAdminService>();

		var controller = new ResultsController(authService, athleteService, iterationManager, courseService, iterationService, resultService, adminService);

		var user = new ClaimsPrincipal(new ClaimsIdentity([new Claim("external_id", "123")]));
		var athlete1 = new Athlete { ID = Guid.NewGuid(), DateOfBirth = new DateOnly(2000, 1, 1) };
		var athlete2 = new Athlete { ID = Guid.NewGuid(), DateOfBirth = new DateOnly(2000, 1, 1) };
		var course = new Course { ID = Guid.NewGuid(), Distance = 10, Units = "km", Race = new Race { Type = "Road" } };
		var result = new Result { ID = Guid.NewGuid(), Athlete = athlete1, Course = course, Duration = new TimeSpan(1, 2, 3) };
		authService.GetCurrentUser().Returns(user);
		athleteService.Find(Arg.Any<string>(), Arg.Any<string>()).Returns(athlete2);
		courseService.Get(Arg.Any<Guid>()).Returns(course);
		resultService.Get(Arg.Any<Guid>()).Returns(result);
		adminService.Verify(Arg.Any<Guid>()).Returns(true);

		var data = new Dictionary<string, StringValues>
		{
			{ "StartTime", "2026-04-03T14:15:16" },
			{ "Duration[h]", "1" },
			{ "Duration[m]", "23" },
			{ "Duration[s]", "45" }
		};
		var form = new FormCollection(data);

		//act
		await controller.Edit(Guid.NewGuid(), form);

		//assert
		await resultService.Received().Update(Arg.Any<Result>(), Arg.Is<Result>(r => r.Duration == new TimeSpan(1, 23, 45)));
	}

	[Fact]
	public async Task CannotSubmitInvalidResultToEditForm()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var courseService = Substitute.For<ICourseService>();
		var iterationService = Substitute.For<IIterationService>();
		var resultService = Substitute.For<IResultService>();
		var adminService = Substitute.For<IAdminService>();

		var controller = new ResultsController(authService, athleteService, iterationManager, courseService, iterationService, resultService, adminService);

		authService.GetCurrentUser().Returns(new ClaimsPrincipal(new ClaimsIdentity([new Claim("external_id", "123")])));
		athleteService.Find(Arg.Any<string>(), Arg.Any<string>()).Returns(new Athlete { DateOfBirth = new DateOnly(2020, 1, 1) });
		courseService.Get(Arg.Any<Guid>()).Returns(new Course { Distance = 40, Units = "km", Race = new Race { Type = "Road" } });
		resultService.Get(Arg.Any<Guid>()).Returns(new Result { Duration = new TimeSpan(1, 2, 3) });

		var data = new Dictionary<string, StringValues>
		{
			{ "StartTime", "2026-04-03T14:15:16" },
			{ "Duration[h]", "1" },
			{ "Duration[m]", "23" },
			{ "Duration[s]", "45" }
		};
		var form = new FormCollection(data);

		//act
		await controller.Edit(Guid.NewGuid(), form);

		//assert
		await resultService.DidNotReceive().Update(Arg.Any<Result>(), Arg.Any<Result>());
	}
}