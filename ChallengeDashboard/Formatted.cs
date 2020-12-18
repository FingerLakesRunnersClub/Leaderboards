namespace FLRC.ChallengeDashboard
{
    public abstract class Formatted<T>
    {
        public Formatted(T value) => Value = value;

        public T Value { get; }
        public abstract string Display { get; }
    }
}