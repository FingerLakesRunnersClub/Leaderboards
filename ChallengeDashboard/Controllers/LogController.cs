using Microsoft.AspNetCore.Mvc;

namespace FLRC.ChallengeDashboard.Controllers;

public class LogController : Controller
{
	private readonly IDataService _dataService;
	private readonly DateTime _today;

	public LogController(IDataService dataService, DateTime? today = null)
	{
		_dataService = dataService;
		_today = today ?? DateTime.Today;
	}

	public async Task<ViewResult> Index(uint? id = null)
		=> View(await GetActivityLog(id, ActivityLogType.Recent, r => getGroup(r.StartTime.Value.Date), _today.Subtract(TimeSpan.FromDays(7))));

	public async Task<ViewResult> All(uint? id = null)
		=> View("Index", await GetActivityLog(id, ActivityLogType.Archive, r => getMonth(r.StartTime.Value.Date)));

	private async Task<ActivityLogViewModel> GetActivityLog(uint? id, ActivityLogType type, Func<Result, string> grouping, DateTime? filter = null)
	{
		var course = id != null ? await _dataService.GetResults(id.Value) : null;
		var results = course == null
			? await _dataService.GetAllResults()
			: new[] { course };

		return new ActivityLogViewModel
		{
			Type = type,
			CourseNames = _dataService.CourseNames,
			Links = _dataService.Links,
			Course = course,
			Results = results
				.SelectMany(c => c.Results)
				.Where(r => filter == null || r.StartTime.Value.Date > filter)
				.OrderByDescending(r => r.StartTime.Value)
				.GroupBy(grouping)
		};
	}

	private static string getGroup(DateTime date)
		=> date == DateTime.Today ? "Today"
			: date == DateTime.Today.Subtract(TimeSpan.FromDays(1)) ? "Yesterday"
			: "This Week";

	private static string getMonth(DateTime date)
		=> date.ToString("MMMM");
}