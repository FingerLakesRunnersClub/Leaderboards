using System.Text.Json;
using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Data;

namespace FLRC.Leaderboards.Services;

public sealed class WebScorerStartList(IContextManager contextManager) : IDataSource
{
	public string URL(uint courseID)
	{
		var series = contextManager.Series().GetAwaiter().GetResult();
		var org = series.Setting[nameof(IConfig.WebScorerOrg)];
		var secret = series.Setting[nameof(IConfig.WebScorerSecret)];
		return $"https://www.webscorer.com/json/startlist?raceid={courseID}&apiid={org}&apipriv={secret}";
	}

	public Core.Results.Result[] ParseCourse(Core.Races.Course course, JsonElement json, IDictionary<string, string> aliases)
		=> throw new NotImplementedException();

	public Core.Athletes.Athlete ParseAthlete(JsonElement element, IDictionary<string, string> aliases)
		=> throw new NotImplementedException();
}