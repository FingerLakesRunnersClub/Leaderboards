using System.Collections.Generic;
using System.Threading.Tasks;

namespace FLRC.Leaderboards.Core;

public interface IGroupAPI
{
	Task<IDictionary<string, IEnumerable<uint>>> GetGroups();
}