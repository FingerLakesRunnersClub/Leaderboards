using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Results;

namespace FLRC.Leaderboards.Core.Series;

public sealed class SeriesResult
{
    public Series Series { get; init; }
    public Athlete Athlete { get; init; }
    public Result[] Results { get; init; }
    public Date StartTime { get; init; }
    public Date FinishTime { get; init; }
    public Time RunningTime { get; init; }
    public Time TotalTime { get; init; }
}