namespace FLRC.Leaderboards.Core.Races;

public record Distance(string Value) : Formatted<string>(Value), IComparable<Distance>
{
	public const double MetersPerMile = 1609.344;
	public const double MetersPerMarathon = 42_195;
	public const double MilesPerMarathon = MetersPerMarathon / MetersPerMile;

	public virtual double Meters { get; } = ParseDistance(Value);
	public double Miles => Meters / MetersPerMile;


	public override string Display => Value;

	protected static double ParseDistance(string value)
	{
		if (value.Contains("marathon", StringComparison.InvariantCultureIgnoreCase))
			return value.Contains("half", StringComparison.InvariantCultureIgnoreCase)
				? MetersPerMarathon / 2
				: MetersPerMarathon;

		var split = Patterns.Distance().Match(value).Groups;
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