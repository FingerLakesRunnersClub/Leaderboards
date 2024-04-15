using FLRC.Leaderboards.Core;
using FLRC.Leaderboards.Core.Tests;
using FLRC.Leaderboards.Web.Controllers;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests;

public sealed class ErrorControllerTests
{
	[Fact]
	public void CanGenerateErrorPageForException()
	{
		//arrange
		var exception = new ExceptionHandlerFeature
		{
			Error = new Exception("The system is down")
		};
		var features = Substitute.For<IFeatureCollection>();
		features.Revision.Returns(1);
		features.Get<IExceptionHandlerFeature>().Returns(exception);
		var http = Substitute.For<IHttpContextAccessor>();
		var context = new DefaultHttpContext(features);
		http.HttpContext.Returns(context);
		var controller = new ErrorController(TestHelpers.Config, http);

		//act
		var response = controller.Index();

		//assert
		var vm = (ErrorViewModel) response.Model!;
		Assert.Equal("Error", vm.Title);
		Assert.Equal("The system is down", vm.Error.Message);
	}

	[Fact]
	public void DefaultsToNotFoundIfNoError()
	{
		//arrange
		var http = Substitute.For<IHttpContextAccessor>();
		http.HttpContext.Returns(new DefaultHttpContext());
		var controller = new ErrorController(TestHelpers.Config, http);

		//act
		var response = controller.Index();

		//assert
		var vm = (ErrorViewModel) response.Model!;
		Assert.Equal("Not Found", vm.Title);
	}
}