using System.Security.Claims;

namespace FLRC.Leaderboards.Web.ViewModels;

public sealed record HeaderViewModel
{
	public string Title { get; init; }
	public Dictionary<string, string> ReportMenu { get; init; }
	public string CourseMenuLabel { get; init; }
	public IDictionary<uint, string> CourseMenu { get; init; }
	public IDictionary<string, string> LinksMenu { get; init; }
	public ClaimsPrincipal User { get; init; }
}