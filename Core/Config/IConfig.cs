namespace FLRC.Leaderboards.Core.Config;

public interface IConfig
{
	string App { get; }
	IDictionary<uint, string> CourseNames { get; }
	IDictionary<string, string> Links { get; }
	IFeatureSet Features { get; }
	IDictionary<string, string> Competitions { get; }
	string CommunityURL { get; }
	string CommunityKey { get; }
}