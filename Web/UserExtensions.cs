using System.Security.Claims;

namespace FLRC.Leaderboards.Web;

public static class UserExtensions
{
	extension(ClaimsPrincipal user)
	{
		public Dictionary<string, string> ClaimDictionary
			=> user.Claims.Dictionary;
	}

	extension(ClaimsIdentity user)
	{
		public Dictionary<string, string> ClaimDictionary
			=> user.Claims.Dictionary;
	}

	extension(IEnumerable<Claim> claims)
	{
		private Dictionary<string, string> Dictionary
			=> claims.ToDictionary(c => c.Type, c => c.Value);
	}
}