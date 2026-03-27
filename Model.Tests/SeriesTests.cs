using Xunit;

namespace FLRC.Leaderboards.Model.Tests;

public class SeriesTests
{
	[Fact]
	public void CanGetSettingByName()
	{
		//arrange
		var id = Guid.NewGuid();
		var series = new Series { ID = id, Settings = [new Setting { SeriesID = id, Key = "test", Value = "val" }] };

		//act
		var setting = series.Setting["test"];

		//assert
		Assert.Equal("val", setting);
	}

	[Fact]
	public void CanGetFeatureByName()
	{
		//arrange
		var id = Guid.NewGuid();
		var series = new Series { ID = id, Features = [new Feature { SeriesID = id, Key = "test", Value = true }] };

		//act
		var enabled = series.Feature["test"];

		//assert
		Assert.True(enabled);
	}
}