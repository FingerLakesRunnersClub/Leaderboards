using System;
using System.Text.RegularExpressions;

namespace FLRC.ChallengeDashboard
{
    public record FormattedResultType : Formatted<ResultType>
    {
        public FormattedResultType(ResultType value) : base(value)
        {
        }

        public override string Display => Regex.Replace(Enum.GetName(Value) ?? "", "([a-z])([A-Z])", "$1 $2");
    }
}