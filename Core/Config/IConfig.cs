using FLRC.Leaderboards.Core.Series;

namespace FLRC.Leaderboards.Core.Config;

public interface IConfig
{
	string App { get; }
	string CourseLabel { get; }
	IFeatureSet Features { get; }

	IDictionary<uint, string> CourseNames { get; set; }
	IDictionary<string, string> Links { get; }
	IDictionary<string, string> Competitions { get; }
	IDictionary<string, byte> Awards { get; }

	string BirthdateField { get; }
	string PrivateField { get; }

	string AliasAPI { get; }
	string GroupAPI { get; }
	string PersonalAPI { get; }

	string CommunityURL { get; }
	string CommunityKey { get; }
	ushort CommunityRetryDelay { get; }
	IDictionary<byte, string> CommunityGroups { get; }

	string SeriesTitle { get; }
	SeriesSet Series { get; }

	string FileSystemResults { get; }
}