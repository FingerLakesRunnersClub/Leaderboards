using FLRC.Leaderboards.Model;
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
		anIteration.HasMany(i => i.Athletes).WithMany(a => a.Registrations).UsingEntity<IterationRegistration>().Table("IterationRegistration");
		anIteration.HasMany(i => i.Challenges).WithOne(c => c.Iteration);
		anIteration.Ignore(i => i.UltraChallenges);

		var aRace = build.Entity<Race>().Table("Races");
		aRace.HasMany(r => r.Courses).WithOne(c => c.Race);

		var aCourse = build.Entity<Course>().Table("Courses");
		aCourse.HasMany(c => c.Results).WithOne(r => r.Course);

		var aResult = build.Entity<Result>().Table("Results");
		aResult.Property(r => r.Duration).HasConversion(ts => (int)ts.TotalMilliseconds, ms => TimeSpan.FromMilliseconds(ms));
		aResult.Property(r => r.StartTime).HasConversion(t => t.ToUniversalTime(), t => t.ToLocalTime());

		var aChallenge = build.Entity<Challenge>().Table("Challenges");
		aChallenge.HasMany(c => c.Courses).WithMany(c => c.Challenges).UsingEntity<ChallengeCourse>().Table("ChallengeCourses");
		aChallenge.Property(c => c.TimeLimit).HasConversion(t => t != null ? (byte?)t.Value.TotalHours : null, t => t != null ? TimeSpan.FromHours(t.Value) : null);
		aChallenge.HasOne(c => c.Athlete);

		var anAthlete = build.Entity<Athlete>().Table("Athletes");
		anAthlete.HasMany(a => a.Results).WithOne(r => r.Athlete);
		anAthlete.HasMany(a => a.LinkedAccounts).WithOne(a => a.Athlete);
		anAthlete.HasMany(a => a.Challenges).WithMany(c => c.Athletes).UsingEntity<AthleteChallenge>().Table("AthleteChallenges");

		build.Entity<Admin>().Table("Admins");
		build.Entity<LinkedAccount>().Table("LinkedAccounts");

		var aRaceLink = build.Entity<RaceLink>().Table("RaceLinks");
		aRaceLink.HasOne(l => l.Race).WithMany(r => r.Links);

	}
}