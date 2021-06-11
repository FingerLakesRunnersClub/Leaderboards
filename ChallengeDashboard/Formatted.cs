namespace FLRC.ChallengeDashboard
{
    public abstract record Formatted<T>
    {
        protected Formatted(T value) => Value = value;

        public T Value { get; }
        public abstract string Display { get; }
    }
}