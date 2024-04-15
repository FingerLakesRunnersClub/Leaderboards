using FLRC.Leaderboards.Core.Athletes;

namespace FLRC.Leaderboards.Core.Community;

public sealed class CommunityAdminViewModel : ViewModel
{
	public override string Title => "Community Admin";

	public IReadOnlyCollection<Row> Rows { get; init; }
	public IEnumerable<Row> MissingRows => Rows.Where(r => r.User is not null && r.MissingGroups.Count != 0);
	public IEnumerable<Row> NoUserRows => Rows.Where(r => r.User is null);
	public IEnumerable<Row> SyncedRows => Rows.Where(r => r.User is not null && r.MissingGroups.Count == 0);


	public sealed class Row
	{
		public Athlete Athlete { get; init; }
		public User User { get; init; }
		public IReadOnlyCollection<string> CurrentGroups { get; init; }
		public IReadOnlyCollection<string> MissingGroups { get; init; }
	}
}