using FLRC.Leaderboards.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FLRC.Leaderboards.Data;

public sealed class DB(DbContextOptions<DB> options) : DbContext(options)
{
	protected override void OnModelCreating(ModelBuilder build)
	{
		base.OnModelCreating(build);

		var aSeries = build.Entity<Series>().Table("Series");
		aSeries.HasMany(s => s.Settings).WithOne(s => s.Series);
		aSeries.HasMany(s => s.Features).WithOne(f => f.Series);
		aSeries.HasMany(s => s.Iterations).WithOne(i => i.Series);

		build.Entity<Setting>().Table("Settings").HasKey(s => new { s.SeriesID, Name = s.Key });
		build.Entity<Feature>().Table("Features").HasKey(s => new { s.SeriesID, Name = s.Key });

		var anIteration = build.Entity<Iteration>().Table("Iterations");
		anIteration.HasMany(i => i.Races).WithMany(c => c.Iterations).UsingEntity<RaceIteration>().Table("RaceIterations");
		anIteration.HasMany(i => i.Challenges).WithOne(c => c.Iteration);

		var aRace = build.Entity<Race>().Table("Races");
		aRace.HasMany(r => r.Courses).WithOne(c => c.Race);

		var aCourse = build.Entity<Course>().Table("Courses");
		aCourse.HasMany(c => c.Results).WithOne(r => r.Course);

		var aResult = build.Entity<Result>().Table("Results");
		aResult.Property(r => r.Duration).HasConversion(ts => (int)ts.TotalMilliseconds, ms => TimeSpan.FromMilliseconds(ms));

		var aChallenge = build.Entity<Challenge>().Table("Challenges");
		aChallenge.HasMany(c => c.Courses).WithMany(c => c.Challenges).UsingEntity<ChallengeCourse>();

		var anAthlete = build.Entity<Athlete>().Table("Athletes");
		anAthlete.HasMany(a => a.Results).WithOne(r => r.Athlete);
		anAthlete.HasMany(a => a.LinkedAccounts).WithOne(a => a.Athlete);

		build.Entity<LinkedAccount>().Table("LinkedAccounts");
	}
}