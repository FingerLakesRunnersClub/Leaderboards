using FLRC.Leaderboards.Core.Reports;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public sealed class LogController(ICourseService courseService, IResultService resultService, DateTime? date = null) : Controller
{
	private readonly DateTime _date = date ?? DateTime.Today;

	[HttpGet]
	public async Task<ViewResult> Index(Guid? id = null)
	{
		var log = await GetActivityLog(id, ActivityLogType.Recent, r => GetGroup(r.StartTime.Date), _date.Subtract(TimeSpan.FromDays(7)));
		var vm = new ViewModel<ActivityLog>("Activity Log" + (log.Course is not null ? $" — {log.Course.Race.Name}" : string.Empty), log);
		return View(vm);
	}

	[HttpGet]
	public async Task<ViewResult> All(Guid? id = null)
	{
		var log = await GetActivityLog(id, ActivityLogType.Archive, r => GetMonth(r.StartTime.Date));
		var vm = new ViewModel<ActivityLog>("Activity Log" + (log.Course is not null ? $" — {log.Course.Race.Name}" : string.Empty), log);
		return View("Index", vm);
	}

	private async Task<ActivityLog> GetActivityLog(Guid? id, ActivityLogType type, Func<Result, string> grouping, DateTime? filter = null)
	{
		var course = id is not null
			? await courseService.Get(id.Value)
			: null;

		var results = id is null
			? await resultService.All()
			: await resultService.Find(id.Value);

		return new ActivityLog
		{
			Type = type,
			Course = course,
			Results = results
				.Where(r => filter is null || r.StartTime.Date > filter)
				.OrderByDescending(r => r.StartTime)
				.GroupBy(grouping)
				.ToArray()
		};
	}

	private static string GetGroup(DateTime date)
		=> date == DateTime.Today ? "Today"
			: date == DateTime.Today.Subtract(TimeSpan.FromDays(1)) ? "Yesterday"
			: "This Week";

	private static string GetMonth(DateTime date)
		=> date.ToString("MMMM yyyy");
}