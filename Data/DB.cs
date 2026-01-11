using Microsoft.EntityFrameworkCore;

namespace FLRC.Leaderboards.Data;

public class DB : DbContext
{
	public DB(DbContextOptions<DB> options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder build)
	{
		base.OnModelCreating(build);
	}
}