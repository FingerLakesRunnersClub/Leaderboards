using FLRC.Leaderboards.Core.Athletes;

namespace FLRC.Leaderboards.Core.Community;

public sealed class CommunityAdminViewModel : ViewModel
{
	public override string Title => "Community Admin";

	public IReadOnlyCollection<Row> Rows { get; init; }

	public sealed class Row
	{
		public Athlete Athlete { get; init; }
		public User User { get; init; }
		public IReadOnlyCollection<string> Current { get; init; }
		public IReadOnlyCollection<string> Missing { get; init; }
	}
}