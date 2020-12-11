using System.Text.Json;
using System.Threading.Tasks;

namespace FLRC.ChallengeDashboard
{
    public interface IDataAPI
    {
        Task<JsonElement> GetCourse(uint id);
    }
}