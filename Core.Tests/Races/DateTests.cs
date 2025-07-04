using FLRC.Leaderboards.Core.Races;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Races;

public sealed class DateTests
{
	[Fact]
	public void CanFormatDate()
	{
		//arrange
		var date = new Date(new DateTime(2012, 3, 4, 5, 6, 7, DateTimeKind.Utc));

		//act
		var display = date.Display;

		//assert
		Assert.Equal("3/4/12 12:06am", display);
	}

	[Fact]
	public void RemovesMidnight()
	{
		//arrange
		var date = new Date(new DateTime(2012, 3, 4, 0, 0, 0, DateTimeKind.Local));

		//act
		var display = date.Display;

		//assert
		Assert.Equal("3/4/12", display);
	}

	[Fact]
	public void CanHandleDST()
	{
		//arrange
		var date = new Date(new DateTime(2012, 3, 14, 5, 6, 7, DateTimeKind.Utc));

		//act
		var display = date.Display;

		//assert
		Assert.Equal("3/14/12 1:06am", display);
	}

	[Theory]
	[InlineData("6/4/21", "5/29/21")]
	[InlineData("6/5/21", "6/5/21")]
	[InlineData("6/6/21", "6/5/21")]
	[InlineData("6/7/21", "6/5/21")]
	[InlineData("6/8/21", "6/5/21")]
	[InlineData("6/9/21", "6/5/21")]
	[InlineData("6/10/21", "6/5/21")]
	[InlineData("6/11/21", "6/5/21")]
	[InlineData("6/12/21", "6/12/21")]
	[InlineData("4/15/22", "4/9/22")]
	[InlineData("4/16/22", "4/16/22")]
	[InlineData("4/17/22", "4/16/22")]
	[InlineData("4/18/22", "4/16/22")]
	[InlineData("4/19/22", "4/16/22")]
	[InlineData("4/20/22", "4/16/22")]
	[InlineData("4/21/22", "4/16/22")]
	[InlineData("4/22/22", "4/16/22")]
	[InlineData("4/23/22", "4/23/22")]
	public void CanGetWeekFromDay(string day, string expectedWeek)
	{
		//arrange
		var date = new Date(DateTime.Parse(day));

		//act
		var week = date.Week;

		//assert
		Assert.Equal(DateOnly.Parse(expectedWeek), week);
	}

	[Fact]
	public void CanCompareDates()
	{
		//arrange
		var d1 = new Date(DateTime.Parse("1/1/2000"));

		//act
		var d2 = new Date(DateTime.Parse("1/2/2000"));

		//assert
		Assert.Equal(-1, d1.CompareTo(d2));
		Assert.Equal(1, d2.CompareTo(d1));
	}
}