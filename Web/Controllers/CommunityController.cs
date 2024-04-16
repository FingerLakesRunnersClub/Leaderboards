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
	public async Task<ViewResult> Admin(uint[] users)
	{
		var vm = await GetMemberships();
		var usersToUpdate = vm.MissingRows.Where(r => users.Contains(r.User.ID)).ToArray();
		var groupsToUpdate = usersToUpdate.SelectMany(r => r.MissingGroups).Distinct();
		var mappingsToAdd = groupsToUpdate.ToDictionary(g => g, g => usersToUpdate.Where(r => r.MissingGroups.Contains(g)).Select(r => r.User.Username).ToArray());
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
		var globalGroup = await groupMembers[0];

		var rows = new List<CommunityAdminViewModel.Row>();
		foreach (var (_, athlete) in athletes.OrderBy(a => a.Value.Name))
		{
			var user = Array.Find(users, u => u.Name == athlete.Name);
			var currentGroups = new List<string>();
			var missingGroups = new List<string>();
			if (globalGroup.Contains(user))
			{
				currentGroups.Add(_config.CommunityGroups[0]);
			}
			else
			{
				missingGroups.Add(_config.CommunityGroups[0]);
			}

			var team = await groupMembers[athlete.Team.Value];
			if (team.Contains(user))
			{
				currentGroups.Add(_config.CommunityGroups[athlete.Team.Value]);
			}
			else
			{
				missingGroups.Add(_config.CommunityGroups[athlete.Team.Value]);
			}

			var row = new CommunityAdminViewModel.Row
			{
				Athlete = athlete,
				User = user,
				CurrentGroups = currentGroups.ToArray(),
				MissingGroups = missingGroups.ToArray()
			};
			rows.Add(row);
		}
		return new CommunityAdminViewModel(rows.ToArray())
		{
			Config = _config
		};
	}
}