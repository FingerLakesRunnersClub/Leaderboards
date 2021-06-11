using System;

namespace FLRC.ChallengeDashboard
{
    public record Date : Formatted<DateTime>
    {
        public Date(DateTime value) : base(value)
        {
        }

        public override string Display => Value.ToLocalTime().ToString("M/d/yyyy h:mmtt").ToLower();

        public DateTime Week => new DateTime(Value.Year, 1, 1).AddDays(Math.Floor((Value.DayOfYear - 2) / 7.0) * 7 + 1);
    }
}