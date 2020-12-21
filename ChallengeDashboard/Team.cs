namespace FLRC.ChallengeDashboard
{
    public class Team : Formatted<byte>
    {
        public Team(byte value) : base(value)
        {
        }

        public override string Display => Value == 1 ? "1–19"
            : Value == 7 ? "70+"
            : $"{Value}0–{Value}9";
    }
}