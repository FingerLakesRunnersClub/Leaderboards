using FLRC.Leaderboards.Data.Models;
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

		var aSeries = build.Entity<Series>().ToTable("Series");
		aSeries.HasMany(s => s.Settings).WithOne(s => s.Series);
		aSeries.HasMany(s => s.Features).WithOne(f => f.Series);
		aSeries.HasMany(s => s.Iterations).WithOne(i => i.Series);

		build.Entity<Setting>().ToTable("Settings");
		build.Entity<Feature>().ToTable("Features");

		var anIteration = build.Entity<Iteration>().ToTable("Iterations");
		anIteration.HasMany(i => i.Races).WithMany(c => c.Iterations).UsingEntity<RaceIteration>();
		anIteration.HasMany(i => i.Challenges).WithOne(c => c.Iteration);

		var aRace = build.Entity<Race>().ToTable("Races");
		aRace.HasMany(r => r.Courses).WithOne(c => c.Race);

		var aCourse = build.Entity<Course>().ToTable("Courses");
		aCourse.HasMany(c => c.Results).WithOne(r => r.Course);

		var aChallenge = build.Entity<Challenge>().ToTable("Challenges");
		aChallenge.HasMany(c => c.Courses).WithMany(c => c.Challenges).UsingEntity<ChallengeCourse>();

		var anAthlete = build.Entity<Athlete>().ToTable("Athletes");
		anAthlete.HasMany(a => a.Results).WithOne(r => r.Athlete);
		anAthlete.HasMany(a => a.LinkedAccounts).WithOne(a => a.Athlete);

		build.Entity<LinkedAccount>().ToTable("LinkedAccounts");
	}
}