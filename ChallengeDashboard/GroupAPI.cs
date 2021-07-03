using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace FLRC.ChallengeDashboard
{
    public class GroupAPI : IGroupAPI
    {
        private readonly HttpClient _httpClient;
        private readonly string _url;

        public GroupAPI(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _url = configuration.GetValue<string>("GroupAPI");
        }
        
        public async Task<JsonElement> GetGroups()
        {
            var response = await _httpClient.GetStreamAsync(_url);
            var json = await JsonDocument.ParseAsync(response);
            return json.RootElement;
        }
    }
}