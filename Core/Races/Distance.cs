using System.Text.RegularExpressions;

namespace FLRC.Leaderboards.Core.Races;

public record Distance(string Value) : Formatted<string>(Value), IComparable<Distance>
{
	public const string DefaultKey = "Default";
	public const double MetersPerMile = 1609.344;
	private const double MarathonMeters = 42_195;

	public double Meters { get; } = ParseDistance(Value);
	public double Miles => Meters / MetersPerMile;


	public override string Display => Value;

	private static double ParseDistance(string value)
	{
		if (value.ToLowerInvariant().Contains("marathon"))
			return value.ToLowerInvariant().Contains("half")
				? MarathonMeters / 2
				: MarathonMeters;

		var split = Regex.Match(value, @"([\d\.]+)(.*)").Groups;
		if (split.Count < 2)
			return 0;

		var digits = double.Parse(split[1].Value.Trim());
		var units = split[2].Value.Trim();

		switch (units.ToLowerInvariant())
		{
			case "k":
			case "km":
			case "kms":
				return digits * 1000;
			case "mi":
			case "mile":
			case "miles":
				return digits * MetersPerMile;
		}

		return digits;
	}

	public int CompareTo(Distance other)
		=> Meters.CompareTo(other.Meters);
}