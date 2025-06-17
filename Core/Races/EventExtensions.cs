using FLRC.AgeGradeCalculator;

namespace FLRC.Leaderboards.Core.Races;

public static class EventExtensions
{
	public static string ToTrackEvent(this string name)
		=> name switch
		{
			_ => $"_{name}"
		};

	public static string ToFieldEvent(this string name)
		=> name switch
		{
			"Discus" => nameof(FieldEvent.DiscusThrow),
			"Turbojav" => nameof(FieldEvent.JavelinThrow),
			_ => name.Replace(" ", "")
		};
}