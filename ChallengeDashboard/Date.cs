using System;

namespace FLRC.ChallengeDashboard
{
    public class Date : Formatted<DateTimeOffset>
    {
        public Date(DateTimeOffset value) : base(value)
        {
        }
        
        public override string Display => Value.ToString("M/d/yyyy h:mmtt").ToLower();
    }
}