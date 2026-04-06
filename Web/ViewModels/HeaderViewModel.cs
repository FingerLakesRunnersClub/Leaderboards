using System.Security.Claims;
using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.ViewModels;

public sealed record HeaderViewModel
{
	public Series Series { get; init; }
	public Dictionary<string, string> ReportMenu { get; init; }
	public string CourseMenuLabel { get; init; }
	public Course[] OfficialCourses { get; set; }
	public Course[] OtherCourses { get; set; }
	public IDictionary<string, string> LinksMenu { get; init; }
	public bool EnableAuth { get; init; }
	public ClaimsPrincipal User { get; init; }
}