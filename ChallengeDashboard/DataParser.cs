using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using FLRC.AgeGradeCalculator;

namespace FLRC.ChallengeDashboard
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
                    ? ParseDistance(distances.EnumerateArray().Select(d => d.GetProperty("Name").GetString()).First())
                    : 0,
                Results = results.GetArrayLength() > 0
                    ? ParseResults(results)
                    : null
            };
        }
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
                    return digits * 1609.344;
            }

            return digits;
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
                    Category = ParseCategory(result.GetProperty("Gender").GetString())
                });

            return athletes[id];
        }

        private static Category? ParseCategory(string value)
            => Enum.TryParse<Category>(value, true, out var category)
                ? category
                : (Category?)null;

        private static DateTime? ParseStart(string value)
            => value != null ? DateTime.Parse(value) : (DateTime?)null;
    }
}