using System.Text.Json;
using System.Threading.Tasks;

namespace FLRC.ChallengeDashboard
{
    public interface IDataAPI
    {
        Task<JsonElement> GetResults(uint id);
    }
}