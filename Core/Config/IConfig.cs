namespace FLRC.Leaderboards.Core.Config;

public interface IConfig
{
	string App { get; }
	IFeatureSet Features { get; }

	IDictionary<uint, string> CourseNames { get; }
	IDictionary<string, string> Links { get; }
	IDictionary<string, string> Competitions { get; }
	IDictionary<string, byte> Awards { get; }

	string BirthdateField { get; }
	string PrivateField { get; }

	string AliasAPI { get; }
	string GroupAPI { get; }

	string CommunityURL { get; }
	string CommunityKey { get; }
}