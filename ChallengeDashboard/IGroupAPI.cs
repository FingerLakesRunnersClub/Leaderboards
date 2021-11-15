using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace FLRC.ChallengeDashboard;

public interface IGroupAPI
{
	Task<IDictionary<string, IEnumerable<uint>>> GetGroups();
}
