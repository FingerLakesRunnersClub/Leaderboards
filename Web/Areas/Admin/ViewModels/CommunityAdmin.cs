using FLRC.Leaderboards.Core.Community;
using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web.Areas.Admin.ViewModels;

public sealed record CommunityAdmin
{
	public Iteration Iteration { get; }
	public Row[] MissingRows { get; }
	public Row[] NoUserRows { get; }
	public Row[] SyncedRows { get; }

	public CommunityAdmin(Iteration iteration, Row[] rows)
	{
		Iteration = iteration;
		MissingRows = rows.Where(r => r.User is not null && r.MissingGroups.Length > 0).ToArray();
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