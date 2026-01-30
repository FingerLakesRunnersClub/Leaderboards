using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Areas.Admin;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public abstract class AdminController : Controller
{
}