using FLRC.Leaderboards.Core;
using FLRC.Leaderboards.Web.Areas.Admin.Controllers;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Admin;

public class HomeControllerTests
{
	[Fact]
	public void CanRenderAdminMenu()
	{
		//arrange
		var controller = new HomeController();

		//act
		var response = controller.Index();

		//assert
		var model = response.Model as ViewModel;
		Assert.Equal("Admin Menu", model!.Title);
	}
}