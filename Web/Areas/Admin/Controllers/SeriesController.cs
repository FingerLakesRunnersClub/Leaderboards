using FLRC.Leaderboards.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FLRC.Leaderboards.Web.Admin;

[Area("Admin")]
public sealed class SeriesController : Controller
{
	private readonly DbContext _db;

	public SeriesController(DbContext db)
		=> _db = db;

	public ViewResult Index()
		=> View(_db.Set<Series>());
}