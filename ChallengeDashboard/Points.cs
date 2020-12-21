namespace FLRC.ChallengeDashboard
{
    public class Points : Formatted<double>
    {
        public Points(double value) : base(value)
        {
        }

        public override string Display => Value.ToString("F2");
    }
}