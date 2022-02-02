using System.Text.RegularExpressions;
using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Metrics;

namespace FLRC.Leaderboards.Core.Data;

public static class DataParser
{
	public static double ParseDistance(string value)
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
				return digits * Course.MetersPerMile;
		}

		return digits;
	}

	public static Category ParseCategory(string value)
		=> Enum.TryParse<AgeGradeCalculator.Category>(value, true, out var category)
			? new Category(category)
			: null;
}