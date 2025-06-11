using FLRC.Leaderboards.Core.Series;
using Microsoft.Extensions.Configuration;

namespace FLRC.Leaderboards.Core.Config;

public sealed record AppConfig : IConfig
{
	public string App { get; }
	public IFeatureSet Features { get; }

	public IDictionary<uint, string> CourseNames { get; set; }
	public IDictionary<string, string> Links { get; }
	public IDictionary<string, string> Competitions { get; }
	public IDictionary<string, byte> Awards { get; }

	public string BirthdateField { get; }
	public string PrivateField { get; }

	public string AliasAPI { get; }
	public string GroupAPI { get; }
	public string PersonalAPI { get; }

	public string CommunityURL { get; }
	public string CommunityKey { get; }
	public ushort CommunityRetryDelay { get; }
	public IDictionary<byte, string> CommunityGroups { get; }

	public string SeriesTitle { get; }
	public SeriesSet Series { get; }

	public string FileSystemResults { get; }

	public AppConfig(IConfiguration config)
	{
		App = config.GetValue<string>("App");
		Features = new FeatureSet(config.GetSection("Features"));
		CourseNames = config.GetSection("Races").GetChildren().ToDictionary(c => c.GetValue<uint>("ID"), c => c.GetValue<string>("Name"));
		Links = GetStringDictionary(config.GetSection("Links"));
		Competitions = GetStringDictionary(config.GetSection("Competitions"));
		Awards = GetByteDictionary(config.GetSection("Awards"));

		var customFields = config.GetSection("CustomFields");
		BirthdateField = customFields.GetValue<string>("Birthdate");
		PrivateField = customFields.GetValue<string>("Private");

		AliasAPI = config.GetValue<string>("AliasAPI");
		GroupAPI = config.GetValue<string>("GroupAPI");
		PersonalAPI = config.GetValue<string>("PersonalAPI");

		CommunityURL = config.GetValue<string>("CommunityURL");
		CommunityKey = Environment.GetEnvironmentVariable("CommunityKey");
		CommunityGroups = GetByteKeyedStringDictionary(config.GetSection("CommunityGroups"));
		CommunityRetryDelay = config.GetValue<ushort>("CommunityRetryDelay");

		SeriesTitle = config.GetValue<string>("SeriesTitle");
		Series = new SeriesSet(config.GetSection("Series"));

		FileSystemResults = config.GetValue<string>("FileSystemResults");
	}

	private static Dictionary<string, string> GetStringDictionary(IConfiguration section)
		=> section.GetChildren().SelectMany(c => c.GetChildren())
			.ToDictionary(c => c.Key, c => c.Value);

	private static Dictionary<string, byte> GetByteDictionary(IConfiguration section)
		=> GetStringDictionary(section)
			.ToDictionary(c => c.Key, c => byte.Parse(c.Value));

	private static Dictionary<byte,string> GetByteKeyedStringDictionary(IConfiguration section)
		=> GetStringDictionary(section)
			.ToDictionary(c => byte.Parse(c.Key), c => c.Value);
}