using FLRC.Leaderboards.Core.Athletes;

namespace FLRC.Leaderboards.Core.Community;

public sealed class CommunityAdminViewModel : ViewModel
{
	public override string Title => "Community Admin";

	public Row[] MissingRows { get; }
	public Row[] NoUserRows { get; }
	public Row[] SyncedRows { get; }

	public CommunityAdminViewModel(Row[] rows)
	{
		MissingRows = rows.Where(r => r.User is not null && r.MissingGroups.Length != 0).ToArray();
		NoUserRows = rows.Where(r => r.User is null).ToArray();
		SyncedRows = rows.Where(r => r.User is not null && r.MissingGroups.Length == 0).ToArray();
	}

	public sealed class Row
	{
		public Athlete Athlete { get; init; }
		public User User { get; init; }
		public string[] CurrentGroups { get; init; }
		public string[] MissingGroups { get; init; }
	}
}