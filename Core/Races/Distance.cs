using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace FLRC.Leaderboards.Core.Races;

public record Distance(string Value) : Formatted<string>(Value), IComparable<Distance>
{
	public const double MetersPerMile = 1609.344;
	public const double MetersPerMarathon = 42_195;
	public const double MilesPerMarathon = MetersPerMarathon / MetersPerMile;

	public virtual double Meters { get; } = ParseDistance(Value);
	public double Miles => Meters / MetersPerMile;

	public override string Display => Value;

	private static readonly Regex RegexPattern = Patterns.Distance();

	private static readonly ConcurrentDictionary<string, double> DistanceCache = new();
	private static readonly Func<string, double, double> KeepCache = (_, d) => d;

	protected static double ParseDistance(string value)
	{
		if (DistanceCache.TryGetValue(value, out var cached))
			return cached;

		if (value.Contains("marathon", StringComparison.InvariantCultureIgnoreCase))
			return DistanceCache.AddOrUpdate(value,
				value.Contains("half", StringComparison.InvariantCultureIgnoreCase)
					? MetersPerMarathon / 2
					: MetersPerMarathon,
				KeepCache);

		var split = RegexPattern.Match(value).Groups;
		if (split.Count < 2)
			return DistanceCache.AddOrUpdate(value, 0, KeepCache);

		var digits = double.Parse(split[1].Value.Trim());
		var units = split[2].Value.Trim();

		switch (units.ToLowerInvariant())
		{
			case "k":
			case "km":
			case "kms":
				return DistanceCache.AddOrUpdate(value, digits * 1000, KeepCache);
			case "mi":
			case "mile":
			case "miles":
				return DistanceCache.AddOrUpdate(value, digits * MetersPerMile, KeepCache);
		}

		return DistanceCache.AddOrUpdate(value, digits, KeepCache);
	}

	public int CompareTo(Distance other)
		=> Meters.CompareTo(other.Meters);

	public virtual bool Equals(Distance other) => CompareTo(other) == 0;

	public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Meters);
}