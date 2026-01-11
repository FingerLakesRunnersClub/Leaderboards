using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FLRC.Leaderboards.Data;

public static class DBExtensions
{
	public static EntityTypeBuilder<T> Table<T>(this EntityTypeBuilder<T> entity, string name) where T : class
		=> entity.ToTable(name.ToLowerInvariant());

	public static void HasEnum<T, F>(this EntityTypeBuilder<T> entity, Expression<Func<T, F>> field) where T : class where F : struct, Enum
		=> entity.Property(field)
			.HasConversion(
				f => f.ToString(),
				v => Enum.Parse<F>(v)
			);
}