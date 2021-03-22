using System;

namespace FLRC.ChallengeDashboard
{
    public class Date : Formatted<DateTimeOffset>
    {
        public Date(DateTimeOffset value) : base(value)
        {
        }
        
        private static readonly string TimeZoneName = !OperatingSystem.IsWindows()
            ? "America/New_York"
            : "Eastern Standard Time";
        private static readonly TimeZoneInfo TimeZone = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneName);

        public override string Display => TimeZoneInfo.ConvertTime(Value, TimeZone).ToString("M/d/yyyy h:mmtt").ToLower();
    }
}