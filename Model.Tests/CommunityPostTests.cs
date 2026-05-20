using Xunit;

namespace FLRC.Leaderboards.Model.Tests;

public sealed class CommunityPostTests
{
	[Fact]
	public void MatchesWhenAthleteHasLinkedAccountAndResultDateMatches()
	{
		//arrange
		var post = new CommunityPost { ID = 123, Date = new DateTime(2020, 1, 1) };
		var account = new LinkedAccount { Type = LinkedAccount.Keys.Discourse, Value = "123" };
		var athlete = new Athlete { LinkedAccounts = [account] };
		var result = new Result { StartTime = new DateTime(2020, 1, 1), Athlete = athlete };

		//act
		var matches = post.Matches(result);

		//assert
		Assert.True(matches);
	}

	[Fact]
	public void DoesNotMatchWhenAthleteHasLinkedAccountButResultDateDoesNotMatch()
	{
		//arrange
		var post = new CommunityPost { ID = 123, Date = new DateTime(2020, 1, 1) };
		var account = new LinkedAccount { Type = LinkedAccount.Keys.Discourse, Value = "123" };
		var athlete = new Athlete { LinkedAccounts = [account] };
		var result = new Result { StartTime = new DateTime(2020, 1, 2), Athlete = athlete };

		//act
		var matches = post.Matches(result);

		//assert
		Assert.False(matches);
	}

	[Fact]
	public void DoesNotMatchWhenAthleteHasNoLinkedAccount()
	{
		//arrange
		var post = new CommunityPost { ID = 123, Date = new DateTime(2020, 1, 1) };
		var account = new LinkedAccount { Type = LinkedAccount.Keys.Discourse, Value = "234" };
		var athlete = new Athlete { LinkedAccounts = [account] };
		var result = new Result { StartTime = new DateTime(2020, 1, 1), Athlete = athlete };

		//act
		var matches = post.Matches(result);

		//assert
		Assert.False(matches);
	}

	[Fact]
	public void HasNarrativeWhenHeaderMatches()
	{
		//arrange
		var post = new CommunityPost { Content = "## Story"};

		//act
		var narrative = post.HasNarrative();

		//assert
		Assert.True(narrative);
	}

	[Fact]
	public void DoesNotHaveNarrativeWhenHeaderDoesNotMatch()
	{
		//arrange
		var post = new CommunityPost { Content = "ooh, fun!"};

		//act
		var narrative = post.HasNarrative();

		//assert
		Assert.False(narrative);
	}

	[Fact]
	public void DoesNotHaveNarrativeWhenHeaderIsQuoted()
	{
		//arrange
		var post = new CommunityPost { Content = "[quote]## Story[/quote]"};

		//act
		var narrative = post.HasNarrative();

		//assert
		Assert.False(narrative);
	}
}