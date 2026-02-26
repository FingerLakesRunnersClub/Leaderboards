using System.Text.Json;

namespace FLRC.Leaderboards.Core.Auth;

public sealed class WebScorerAuthenticator(HttpClient http) : IWebScorerAuthenticator
{
	public async Task<uint> Login(string email, string password)
	{
		var url = $"https://api.webscorer.com/racetimer/webscorerapi/account?emailaddress={email}&password={password}";
		var response = await http.GetStringAsync(url);
		var json = JsonSerializer.Deserialize<JsonElement>(response);
		return json.GetProperty("UserId").GetUInt32();
	}
}