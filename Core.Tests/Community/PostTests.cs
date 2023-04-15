using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Community;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Results;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Community;

public class PostTests
{
	[Fact]
	public void HasNarrativeWhenHeaderFound()
	{
		//arrange
		var post = new Post
		{
			Content = "## Story"
		};

		//act
		var hasNarrative = post.HasNarrative();

		//assert
		Assert.True(hasNarrative);
	}

	[Fact]
	public void DoesNotHaveNarrativeWhenHeaderNotFound()
	{
		//arrange
		var post = new Post
		{
			Content = "# Story"
		};

		//act
		var hasNarrative = post.HasNarrative();

		//assert
		Assert.False(hasNarrative);
	}

	[Fact]
	public void DoesNotHaveNarrativeWhenQuoted()
	{
		//arrange
		var post = new Post
		{
			Content = "[quote] ## Story [/quote] lol"
		};

		//act
		var hasNarrative = post.HasNarrative();

		//assert
		Assert.False(hasNarrative);
	}

	[Fact]
	public void MatchesResultWhenNameAndDateMatch()
	{
		//arrange
		var post = new Post
		{
			Name = "User 123",
			Date = new DateTime(2022, 4, 7)
		};

		var result = new Result
		{
			Athlete = new Athlete { Name = "User 123" },
			StartTime = new Date(new DateTime(2022, 4, 7))
		};

		//act
		var matches = post.Matches(result);

		//assert
		Assert.True(matches);
	}

	[Fact] public void DoesNotMatchResultWhenNameDoesNotMatch()
	{
		//arrange
		var post = new Post
		{
			Name = "User 123",
			Date = new DateTime(2022, 4, 7)
		};

		var result = new Result
		{
			Athlete = new Athlete { Name = "User 456" },
			StartTime = new Date(new DateTime(2022, 4, 7))
		};

		//act
		var matches = post.Matches(result);

		//assert
		Assert.False(matches);
	}

	[Fact]
	public void DoesNotMatchResultWhenDateDoesNotMatch()
	{
		//arrange
		var post = new Post
		{
			Name = "User 123",
			Date = new DateTime(2022, 4, 7)
		};

		var result = new Result
		{
			Athlete = new Athlete { Name = "User 123" },
			StartTime = new Date(new DateTime(2022, 4, 6))
		};

		//act
		var matches = post.Matches(result);

		//assert
		Assert.False(matches);
	}
}