using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Tests;
using FLRC.Leaderboards.Model;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Services.Tests;

public sealed class LegacyDataConverterTests
{
	[Fact]
	public async Task CanConvertResults()
	{
		//arrange
		var athleteService = Substitute.For<IAthleteService>();
		var converter = new LegacyDataConverter(athleteService);

		var athlete = new Athlete { ID = Guid.NewGuid(), Name = "Test" };
		athleteService.Find(Arg.Any<string>(), Arg.Any<string>()).Returns(athlete);

		//act
		var results = await converter.ConvertResults(Guid.NewGuid(), nameof(WebScorer), CourseData.Results);

		//assert
		Assert.Equal(8, results.Length);
		Assert.Equal(athlete.ID, results[0].AthleteID);
	}

	[Fact]
	public async Task CanGetAthlete()
	{
		//arrange
		var athleteService = Substitute.For<IAthleteService>();
		var converter = new LegacyDataConverter(athleteService);

		//act
		var athlete = await converter.GetAthlete(nameof(WebScorer), CourseData.Athlete1);

		//assert
		Assert.Equal("A1", athlete.Name);
	}

	[Fact]
	public async Task CanMatchAthleteByExternalID()
	{
		//arrange
		var athleteService = Substitute.For<IAthleteService>();
		var converter = new LegacyDataConverter(athleteService);

		athleteService.Find(LinkedAccount.Keys.WebScorer, 123.ToString()).Returns(new Athlete { Name = "New1" });

		//act
		var athlete = await converter.GetAthlete(nameof(WebScorer), CourseData.Athlete1);

		//assert
		Assert.Equal("New1", athlete.Name);
	}

	[Fact]
	public async Task CanMatchAthleteByDOB()
	{
		//arrange
		var athleteService = Substitute.For<IAthleteService>();
		var converter = new LegacyDataConverter(athleteService);

		athleteService.Find("A1", DateOnly.Parse("1/1/2000")).Returns(new Athlete { Name = "New1" });

		//act
		var athlete = await converter.GetAthlete(nameof(WebScorer), CourseData.Athlete1);

		//assert
		Assert.Equal("New1", athlete.Name);
	}

	[Fact]
	public async Task CanMatchAthleteByAge()
	{
		//arrange
		var athleteService = Substitute.For<IAthleteService>();
		var converter = new LegacyDataConverter(athleteService);

		var date = DateTime.Parse("1/1/2020");
		athleteService.Find("A1", 20, date).Returns(new Athlete { Name = "New1" });

		//act
		var athlete = await converter.GetAthlete(nameof(WebScorer), CourseData.Athlete1, date);

		//assert
		Assert.Equal("New1", athlete.Name);
	}

	[Fact]
	public async Task AddsNewAthleteOnImport()
	{
		//arrange
		var athleteService = Substitute.For<IAthleteService>();
		var converter = new LegacyDataConverter(athleteService);

		//act
		var athlete = await converter.GetAthlete(nameof(WebScorer), CourseData.Athlete1);

		//assert
		await athleteService.Received().AddAthlete(athlete);
	}

	[Fact]
	public async Task DoesNotAddMatchedAthleteOnImport()
	{
		//arrange
		var athleteService = Substitute.For<IAthleteService>();
		var converter = new LegacyDataConverter(athleteService);

		athleteService.Find(LinkedAccount.Keys.WebScorer, 123.ToString()).Returns(new Athlete { Name = "New1" });

		//act
		var athlete = await converter.GetAthlete(nameof(WebScorer), CourseData.Athlete1);

		//assert
		await athleteService.DidNotReceive().AddAthlete(athlete);
	}

	[Fact]
	public async Task UpdatesAthleteWithNewLinkedAccount()
	{
		//arrange
		var athleteService = Substitute.For<IAthleteService>();
		var converter = new LegacyDataConverter(athleteService);

		var existing = new Athlete
		{
			Name = "New1",
			DateOfBirth = DateOnly.Parse("1/1/2000"),
			Category = 'M'
		};
		athleteService.Find(LinkedAccount.Keys.WebScorer, 123.ToString()).Returns(existing);

		//act
		await converter.GetAthlete(nameof(WebScorer), CourseData.Athlete1);

		//assert
		await athleteService.Received().AddLinkedAccount(Arg.Any<Athlete>(), Arg.Any<LinkedAccount>());
	}

	[Fact]
	public async Task DoesNotUpdateAthleteWithExistingLinkedAccount()
	{
		//arrange
		var athleteService = Substitute.For<IAthleteService>();
		var converter = new LegacyDataConverter(athleteService);

		var existing = new Athlete
		{
			Name = "New1",
			DateOfBirth = DateOnly.Parse("1/1/2000"),
			Category = 'M',
			LinkedAccounts = [new LinkedAccount { Type = LinkedAccount.Keys.WebScorer, Value = 123.ToString() }]
		};
		athleteService.Find(LinkedAccount.Keys.WebScorer, 123.ToString()).Returns(existing);

		//act
		await converter.GetAthlete(nameof(WebScorer), CourseData.Athlete1);

		//assert
		await athleteService.DidNotReceive().UpdateAthlete(Arg.Any<Athlete>(), Arg.Any<Athlete>());
	}
}