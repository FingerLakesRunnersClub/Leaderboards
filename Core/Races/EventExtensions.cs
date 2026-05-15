using FLRC.AgeGradeCalculator;

namespace FLRC.Leaderboards.Core.Races;

public static class EventExtensions
{
	extension(string name)
	{
		public string ToTrackEvent()
			=> name switch
			{
				_ => $"_{name}"
			};

		public string ToFieldEvent()
			=> name switch
			{
				"Discus" => nameof(FieldEvent.DiscusThrow),
				"Turbojav" => nameof(FieldEvent.JavelinThrow),
				_ => name.Replace(" ", "")
			};
	}
}