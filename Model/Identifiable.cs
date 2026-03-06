namespace FLRC.Leaderboards.Model;

public interface Identifiable<T> where T : IComparable, IComparable<T>
{
	T ID { get; set; }
}