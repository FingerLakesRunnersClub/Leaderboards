using System.Text.Json;
using FLRC.Leaderboards.Core.Athletes;

namespace FLRC.Leaderboards.Core.Auth;

public sealed class WebScorerAuthenticator(HttpClient http) : IWebScorerAuthenticator
{
	public async Task<Athlete> Login(string email, string password)
	{
		var url = $"https://api.webscorer.com/racetimer/webscorerapi/account?emailaddress={email}&password={password}";
		var response = await http.GetStringAsync(url);
		var json = JsonSerializer.Deserialize<JsonElement>(response);

		return new Athlete
		{
			ID =  json.GetProperty("UserId").GetUInt32(),
			Name = $"{json.GetProperty("FirstName").GetString()} {json.GetProperty("LastName").GetString()}"
		};
	}
}