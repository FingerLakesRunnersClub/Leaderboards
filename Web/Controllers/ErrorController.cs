using FLRC.Leaderboards.Core;
using FLRC.Leaderboards.Core.Config;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public sealed class ErrorController : Controller
{
	private readonly IConfig _config;
	private readonly IHttpContextAccessor _http;

	public ErrorController(IConfig config, IHttpContextAccessor http)
	{
		_config = config;
		_http = http;
	}

	public ViewResult Index()
	{
		var exception = _http.HttpContext!.Features.Get<IExceptionHandlerFeature>();
		var error = new ErrorViewModel
		{
			Config = _config,
			Error = exception?.Error
		};
		return View("Error", error);
	}
}