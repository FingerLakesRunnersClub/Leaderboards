using System;
using System.Globalization;

namespace FLRC.ChallengeDashboard
{
    public class Date : Formatted<DateTime>
    {
        public Date(DateTime value) : base(value)
        {
        }

        public override string Display => Value.ToString("M/d/yyyy h:mmtt").ToLower();
    }
}