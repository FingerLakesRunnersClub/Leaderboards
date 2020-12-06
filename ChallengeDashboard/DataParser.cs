using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace ChallengeDashboard
{
    public static class DataParser
    {
        public static Course ParseCourse(JsonElement json)
        {
            var distances = json.GetProperty("Distances");
            var results = json.GetProperty("Racers");

            return new Course
            {
                ID = json.GetProperty("RaceId").GetUInt32(),
                Name = json.GetProperty("Name").GetString(),
                Type = json.GetProperty("SportType").GetString(),
                Distance = distances.GetArrayLength() > 0
                    ? string.Join(", ", distances.EnumerateArray().Select(d => d.GetProperty("Name").GetString()))
                    : null,
                Results = results.GetArrayLength() > 0
                    ? ParseResults(results)
                    : null
            };
        }

        private static IEnumerable<Result> ParseResults(JsonElement results)
            => results.EnumerateArray().Where(r => r.GetProperty("Finished").GetByte() == 1).Select(r => new Result
            {
                Athlete = ParseAthlete(r),
                StartTime = ParseStart(r.GetProperty("StartTime").GetString()),
                Duration = TimeSpan.FromSeconds(r.GetProperty("RaceTime").GetDouble())
            });

        private static readonly IDictionary<uint, Athlete> athletes = new Dictionary<uint, Athlete>();
        private static Athlete ParseAthlete(JsonElement result)
        {
            var id = result.GetProperty("UserId").GetUInt32();
            if (!athletes.ContainsKey(id))
                athletes.Add(id, new Athlete
                {
                    ID = id,
                    Name = result.GetProperty("Name").GetString(),
                    Age = result.GetProperty("Age").GetByte(),
                    Category = result.GetProperty("Gender").GetString()
                });

            return athletes[id];
        }

        private static DateTime? ParseStart(string value)
            => value != null ? DateTime.Parse(value) : (DateTime?)null;
    }
}