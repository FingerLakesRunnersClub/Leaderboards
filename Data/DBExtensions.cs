using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FLRC.Leaderboards.Data;

public static class DBExtensions
{
	extension<T>(EntityTypeBuilder<T> entity) where T : class
	{
		public EntityTypeBuilder<T> Table(string name) => entity.ToTable(name.ToLowerInvariant());

		public void HasEnum<F>(Expression<Func<T, F>> field) where F : struct, Enum
			=> entity.Property(field)
				.HasConversion(
					f => f.ToString(),
					v => Enum.Parse<F>(v)
				);
	}
}