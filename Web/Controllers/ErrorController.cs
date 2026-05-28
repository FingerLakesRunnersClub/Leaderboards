using FLRC.Leaderboards.Web.ViewModels;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public sealed class ErrorController(IHttpContextAccessor http) : Controller
{
	public ViewResult Index()
	{
		var exception = http.HttpContext!.Features.Get<IExceptionHandlerFeature>();
		var error = exception?.Error;
		var title = error is not null ? "Error" : "Not Found";
		var vm = new ViewModel<Exception>(title, error);
		return View("Error", vm);
	}
}