using Microsoft.Extensions.Configuration;

namespace FLRC.Leaderboards.Core.Series;

public sealed class SeriesSet : List<Series>
{
	public SeriesSet(IConfiguration section)
		=> AddRange(section.GetChildren().Select(Series));

	private static Series Series(IConfigurationSection item)
		=> item.Get<Series>();

	public Series this[string id]
		=> Find(s => s.ID == id);
}