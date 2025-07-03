using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Races;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Data;

public class ResultFileReaderTests
{
	[Fact]
	public void CanParseLongDistances()
	{
		//arrange
		var course = new Course { Race = new Race() };
		const string line = "  Robert Sutherland                   M             23      123'04.50\"";

		//act
		var result = ResultFileReader.ParseResult(course, line);

		//assert
		Assert.Equal("123'04.50\"", result.Performance.Display);
	}
}