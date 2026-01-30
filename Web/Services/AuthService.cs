using FLRC.Leaderboards.Core.Auth;
using Microsoft.AspNetCore.Http;

namespace FLRC.Leaderboards.Web.Services;

public interface IAuthService
{
	Task LogIn();
	Task LogOut();
}

public sealed class AuthService(IDiscourseAuthenticator discourse, IHttpContextAccessor accessor) : IAuthService
{
	public Task LogIn()
	{
		throw new NotImplementedException();
	}

	public Task LogOut()
	{
		throw new NotImplementedException();
	}
}