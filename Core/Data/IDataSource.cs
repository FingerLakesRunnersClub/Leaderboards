using System.Text.Json;
using FLRC.Leaderboards.Core.Results;

namespace FLRC.Leaderboards.Core.Data;

public interface IDataSource
{
	public string Name { get; }
	public string URL(uint courseID);
	IEnumerable<Result> ParseCourse(Course course, JsonElement json);
}