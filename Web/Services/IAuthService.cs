using System.Security.Claims;
using System.Security.Principal;

namespace FLRC.Leaderboards.Web.Services;

public interface IAuthService
{
	Task LogIn(IIdentity identity);
	bool IsLoggedIn();
	ClaimsPrincipal GetCurrentUser();
	Task LogOut();
}