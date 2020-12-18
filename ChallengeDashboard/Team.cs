namespace FLRC.ChallengeDashboard
{
    public class Team : Formatted<byte>
    {
        public Team(byte value) : base(value)
        {
        }

        public override string Display => $"{Value}0–{Value}9";
    }
}