using FLRC.Leaderboards.Core;

namespace FLRC.Leaderboards.Web.Areas.Admin;

public sealed class ViewModel<T>(string title, T data) : ViewModel
{
	public override string Title => title;

	public T Data => data;
}