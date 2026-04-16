using FLRC.AgeGradeCalculator;
using FLRC.Leaderboards.Model;
using EventExtensions = FLRC.Leaderboards.Core.Races.EventExtensions;

namespace FLRC.Leaderboards.Web;

public static class RaceExtensions
{
	extension(Race race)
	{
		public bool IsFieldEvent => Enum.TryParse<FieldEvent>(EventExtensions.ToFieldEvent(race.Name), out _);
		public string EventMetric => race.IsFieldEvent ? race.FieldEventMetric : "Time";
		public string FieldEventMetric => race.Name is "High Jump" or "Pole Vault" ? "Height" : "Distance";
	}
}