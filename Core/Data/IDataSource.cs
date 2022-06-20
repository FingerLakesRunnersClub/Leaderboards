using System.Text.Json;
using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Results;

namespace FLRC.Leaderboards.Core.Data;

public interface IDataSource
{
	public string Name { get; }
	public string URL(uint courseID);
	IReadOnlyCollection<Result> ParseCourse(Course course, JsonElement json, IDictionary<string, string> aliases);
	Athlete ParseAthlete(JsonElement element, IDictionary<string, string> aliases);
}