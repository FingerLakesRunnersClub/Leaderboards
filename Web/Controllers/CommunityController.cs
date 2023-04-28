using FLRC.Leaderboards.Core.Community;
using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Data;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public sealed class CommunityController : Controller
{
	private readonly IDataService _dataService;
	private readonly IConfig _config;

	public CommunityController(IDataService dataService, IConfig config)
	{
		_dataService = dataService;
		_config = config;
	}

	[HttpGet]
	public async Task<ViewResult> Admin()
		=> View(await GetMemberships());

	[HttpPost]
	public async Task<ViewResult> Admin(IReadOnlyCollection<uint> users)
	{
		var vm = await GetMemberships();
		var usersToUpdate = vm.Rows.Where(r => r.User is not null && users.Contains(r.User.ID)).ToArray();
		var groupsToUpdate = usersToUpdate.SelectMany(r => r.Missing).Distinct().ToArray();
		var mappingsToAdd = groupsToUpdate.ToDictionary(g => g, g => usersToUpdate.Where(r => r.Missing.Contains(g)).Select(r => r.User.Username).ToArray());
		await _dataService.AddCommunityGroupMembers(mappingsToAdd);
		return await Admin();
	}

	private async Task<CommunityAdminViewModel> GetMemberships()
	{
		var athleteTask = _dataService.GetAthletes();
		var userTask = _dataService.GetCommunityUsers();
		var groupMembers = _config.CommunityGroups
			.ToDictionary(team => team.Key, team => _dataService.GetCommunityGroupMembers(team.Value));

		var athletes = await athleteTask;
		var users = await userTask;
		var global = await groupMembers[0];

		var rows = new List<CommunityAdminViewModel.Row>();
		foreach (var (_, athlete) in athletes.OrderBy(a => a.Value.Name))
		{
			var user = users.FirstOrDefault(u => u.Name == athlete.Name);
			var current = new List<string>();
			var missing = new List<string>();
			if (global.Contains(user))
			{
				current.Add(_config.CommunityGroups[0]);
			}
			else
			{
				missing.Add(_config.CommunityGroups[0]);
			}

			var team = await groupMembers[athlete.Team.Value];
			if (team.Contains(user))
			{
				current.Add(_config.CommunityGroups[athlete.Team.Value]);
			}
			else
			{
				missing.Add(_config.CommunityGroups[athlete.Team.Value]);
			}

			var row = new CommunityAdminViewModel.Row
			{
				Athlete = athlete,
				User = user,
				Current = current,
				Missing = missing
			};
			rows.Add(row);
		}
		return new CommunityAdminViewModel
		{
			Rows = rows,
			Config = _config
		};
	}
}