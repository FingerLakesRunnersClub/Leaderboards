using FLRC.Leaderboards.Core.Athletes;

namespace FLRC.Leaderboards.Core.Community;

public sealed class CommunityAdminViewModel : ViewModel
{
	public override string Title => "Community Admin";

	public Row[] Rows { get; init; }
	public Row[] MissingRows => Rows.Where(r => r.User is not null && r.MissingGroups.Length != 0).ToArray();
	public Row[] NoUserRows => Rows.Where(r => r.User is null).ToArray();
	public Row[] SyncedRows => Rows.Where(r => r.User is not null && r.MissingGroups.Length == 0).ToArray();


	public sealed class Row
	{
		public Athlete Athlete { get; init; }
		public User User { get; init; }
		public string[] CurrentGroups { get; init; }
		public string[] MissingGroups { get; init; }
	}
}