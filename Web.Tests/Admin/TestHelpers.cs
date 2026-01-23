using FLRC.Leaderboards.Data;
using Microsoft.EntityFrameworkCore;

namespace FLRC.Leaderboards.Web.Tests.Admin;

public static class TestHelpers
{
	public static DB CreateDB()
		=> new(new DbContextOptionsBuilder<DB>()
			.UseInMemoryDatabase(DateTime.UtcNow.ToFileTimeUtc().ToString()).Options);
}