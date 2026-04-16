using FLRC.Leaderboards.Web.Areas.Admin.Controllers;
using FLRC.Leaderboards.Web.ViewModels;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Admin;

public sealed class HomeControllerTests
{
	[Fact]
	public void CanRenderAdminMenu()
	{
		//arrange
		var controller = new HomeController();

		//act
		var response = controller.Index();

		//assert
		var model = response.Model as ViewModel<string>;
		Assert.Equal("Admin Menu", model!.Title);
	}
}