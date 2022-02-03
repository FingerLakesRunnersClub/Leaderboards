using System.Text.RegularExpressions;

namespace FLRC.Leaderboards.Core;

public record Distance : Formatted<string>
{
	public const double MetersPerMile = 1609.344;
	public double Meters { get; }

	public double Miles => Meters / MetersPerMile;

	public Distance(string value) : base(value)
		=> Meters = ParseDistance(value);

	public override string Display => Value;

	private static double ParseDistance(string value)
	{
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
}