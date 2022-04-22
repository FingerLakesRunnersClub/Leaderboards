using FLRC.Leaderboards.Core.Reports;
using FLRC.Leaderboards.Core.Tests.Leaders;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Reports;

public class AwardsViewModelTests
{
	[Fact]
	public void CalculatesCorrectAwards()
	{
		//arrange
		var results = LeaderboardData.Courses;

		//act
		var vm = new AwardsViewModel(TestHelpers.Config, results);

		//assert
		var awards = vm.Awards;
		var athletes = awards.Count;
		var count = awards.Sum(athlete => athlete.Value.Length);
		var amount = awards.Sum(athlete => athlete.Value.Sum(award => award.Value));
		Assert.Equal(4, athletes);
		Assert.Equal(27, count);
		Assert.Equal(91, amount);
		Assert.Contains("1st Overall Points (M)", awards[LeaderboardData.Athlete1].Select(a => a.Name));
		Assert.Contains("1st Overall Community", awards[LeaderboardData.Athlete1].Select(a => a.Name));
		Assert.Contains("2nd Overall Points (M)", awards[LeaderboardData.Athlete2].Select(a => a.Name));
		Assert.Contains("1st Overall Miles", awards[LeaderboardData.Athlete2].Select(a => a.Name));
		Assert.Contains("1st Overall Points (F)", awards[LeaderboardData.Athlete3].Select(a => a.Name));
		Assert.Contains("2nd Overall Community", awards[LeaderboardData.Athlete3].Select(a => a.Name));
		Assert.Contains("2nd Overall Points (F)", awards[LeaderboardData.Athlete4].Select(a => a.Name));
		Assert.Contains("1st Overall Miles", awards[LeaderboardData.Athlete4].Select(a => a.Name));
	}
}