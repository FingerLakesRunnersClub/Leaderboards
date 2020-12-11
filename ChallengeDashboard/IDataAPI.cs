using System.Text.Json;
using System.Threading.Tasks;

namespace ChallengeDashboard
{
    public interface IDataAPI
    {
        Task<JsonElement> GetCourse(uint id);
    }
}