using Microsoft.Extensions.Configuration;

namespace FLRC.Leaderboards.Core.Series;

public sealed class SeriesSet : List<Series>
{
	public SeriesSet(IConfiguration section)
		=> AddRange(section.GetChildren().Select(Series));

	private static Series Series(IConfigurationSection item)
		=> new()
		{
			ID = item.GetValue<string>("ID"),
			Name = item.GetValue<string>("Name"),
			HourLimit = item.GetValue<byte>("HourLimit"),
			Races = item.GetSection("Races").Get<uint[]>()
		};

	public Series this[string id]
		=> Find(s => s.ID == id);
}