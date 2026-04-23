namespace FLRC.Leaderboards.Model;

public sealed class LinkedAccountComparer : IEqualityComparer<LinkedAccount>
{
	public bool Equals(LinkedAccount? x, LinkedAccount? y)
		=> x is not null
		   && y is not null
		   && x.Type == y.Type
		   && x.Value.Equals(y.Value, StringComparison.InvariantCultureIgnoreCase);

	public int GetHashCode(LinkedAccount obj)
	{
		var hashCode = new HashCode();
		hashCode.Add(obj.Type, StringComparer.InvariantCultureIgnoreCase);
		hashCode.Add(obj.Value, StringComparer.InvariantCultureIgnoreCase);
		return hashCode.ToHashCode();
	}
}