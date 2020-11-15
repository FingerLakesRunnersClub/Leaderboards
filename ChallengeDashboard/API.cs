using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChallengeDashboard
{
    public class API
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseURL;

        public API(HttpClient httpClient, string baseURL)
        {
            _httpClient = httpClient;
            _baseURL = baseURL;
        }

        public async Task<Course> GetCourse(uint id)
        {
            var url = $"{_baseURL}/results?raceid={id}";
            var response = await _httpClient.GetStreamAsync(url);
            var json = await JsonDocument.ParseAsync(response);
            var root = json.RootElement;
            var distances = root.GetProperty("Distances");
            return new Course
            {
                ID = root.GetProperty("RaceId").GetUInt32(),
                Name = root.GetProperty("Name").GetString(),
                Type = root.GetProperty("SportType").GetString(),
                Distance = distances.GetArrayLength() > 0
                    ? string.Join(", ", distances.EnumerateArray().Select(r => r.GetProperty("Name").GetString()))
                    : null
            };
        }
    }
}