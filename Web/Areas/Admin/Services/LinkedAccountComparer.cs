using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.Areas.Admin.Services;

public sealed class LinkedAccountComparer : IEqualityComparer<LinkedAccount>
{
	public bool Equals(LinkedAccount x, LinkedAccount y)
		=> x is not null
		   && y is not null
		   && x.Type == y.Type
		   && x.Value == y.Value;

	public int GetHashCode(LinkedAccount obj)
	{
		var hashCode = new HashCode();
		hashCode.Add(obj.Type, StringComparer.InvariantCultureIgnoreCase);
		hashCode.Add(obj.Value, StringComparer.InvariantCultureIgnoreCase);
		return hashCode.ToHashCode();
	}
}