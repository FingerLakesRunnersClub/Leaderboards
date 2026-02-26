using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Data.Models;
using FLRC.Leaderboards.Web.Areas.Admin.Services;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Admin;

public sealed class RegistrationManagerTests
{
	[Fact]
	public async Task CanImportRegistrationInfoFromWebScorer()
	{
		//arrange
		var iterationService = Substitute.For<IIterationService>();
		var importManager = Substitute.For<IImportManager>();
		var resultService = Substitute.For<IResultService>();
		var manager = new RegistrationManager(importManager, iterationService, resultService);

		//act
		var iteration = new Iteration { RegistrationType = nameof(WebScorer), RegistrationContext = "123" };
		await manager.Update(iteration);

		//assert
		await importManager.Received().ImportAthletes(nameof(WebScorer), 123);
	}

	[Fact]
	public async Task CanUseResultsAsRegistrationInfo()
	{
		//arrange
		var iterationService = Substitute.For<IIterationService>();
		var importManager = Substitute.For<IImportManager>();
		var resultService = Substitute.For<IResultService>();
		var manager = new RegistrationManager(importManager, iterationService, resultService);

		//act
		var id = Guid.NewGuid();
		var iteration = new Iteration { RegistrationType = "AnyRace", Races = [new Race { Courses = [new Course { ID = id }] }] };
		await manager.Update(iteration);

		//assert
		await resultService.Received().Find(id);
	}
}