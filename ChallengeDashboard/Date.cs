using System;

namespace FLRC.ChallengeDashboard
{
    public class Date : Formatted<DateTime>
    {
        public Date(DateTime value) : base(value)
        {
        }

        public override string Display => Value.ToLocalTime().ToString("M/d/yyyy h:mmtt").ToLower();
    }   
}