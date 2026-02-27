using Xunit;

namespace FLRC.Leaderboards.Model.Tests;

public sealed class AthleteTests
{
	[Theory]
	[InlineData(2000, 01, 01, 21)]
	[InlineData(2000, 01, 02, 20)]
	[InlineData(1950, 01, 01, 71)]
	[InlineData(1950, 01, 02, 70)]
	public void CanGetAgeAsOfACertainDate(ushort year, byte month, byte day, byte expected)
	{
		//arrange
		var athlete = new Athlete { DateOfBirth = new DateOnly(year, month, day) };

		var asOf = new DateTime(2021, 1, 1);

		//act
		var age = athlete.AgeAsOf(asOf);

		//assert
		Assert.Equal(expected, age);
	}

	[Fact]
	public void HasLinkedAccountIfTypeMatches()
	{
		//arrange
		var athlete = new Athlete { LinkedAccounts = [new LinkedAccount { Type = "test1" }] };

		//act
		var hasAccount = athlete.HasLinkedAccount("test1");

		//assert
		Assert.True(hasAccount);
	}

	[Fact]
	public void DoesNotHaveLinkedAccountIfTypeDoesNotMatch()
	{
		//arrange
		var athlete = new Athlete { LinkedAccounts = [new LinkedAccount { Type = "test1" }] };

		//act
		var hasAccount = athlete.HasLinkedAccount("test2");

		//assert
		Assert.False(hasAccount);
	}

	[Fact]
	public void IsRegisteredIfIterationInRegistrations()
	{
		//arrange
		var i1 = new Iteration { Name = "test1" };
		var athlete = new Athlete { Registrations = [i1] };

		//act
		var isRegistered = athlete.IsRegistered(i1);

		//assert
		Assert.True(isRegistered);
	}

	[Fact]
	public void IsNotRegisteredIfIterationNotInRegistrations()
	{
		//arrange
		var i1 = new Iteration { Name = "test1" };
		var i2 = new Iteration { Name = "test2" };
		var athlete = new Athlete { Registrations = [i1] };

		//act
		var isRegistered = athlete.IsRegistered(i2);

		//assert
		Assert.False(isRegistered);
	}
}