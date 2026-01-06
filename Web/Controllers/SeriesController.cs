using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Series;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public sealed class SeriesController : Controller
{
	private readonly ISeriesManager _seriesManager;
	private readonly IConfig _config;

	public SeriesController(ISeriesManager seriesManager, IConfig config)
	{
		_seriesManager = seriesManager;
		_config = config;
	}

	public async Task<ViewResult> Index(string id)
	{
		var results = await _seriesManager.Earliest();
		var series = id is not null ? _config.Series[id] : _config.Series.First();
		var vm = new SeriesViewModel
		{
			Config = _config,
			Series = series,
			Results = results.TryGetValue(series, out var result) ?  result : []
		};
		return View(vm);
	}
}