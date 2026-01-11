namespace FLRC.Leaderboards.Data;

public interface Identifiable<T>
{
	T ID { get; set; }
}