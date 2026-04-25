using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Areas.Admin.ViewModels;
using FLRC.Leaderboards.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Areas.Admin.Controllers;

[Area(nameof(Admin))]
[Authorize(nameof(Admin))]
public sealed class CommunityController(IIterationManager iterationManager, ICommunityManager communityManager, IConfig config) : Controller
{
	[HttpGet]
	public async Task<ViewResult> Index()
		=> View(await GetMemberships());

	[HttpPost]
	public async Task<ViewResult> Index(uint[] users)
	{
		var vm = await GetMemberships();
		var usersToUpdate = vm.Data.MissingRows.Where(r => users.Contains(r.User.ID)).ToArray();
		var groupsToUpdate = usersToUpdate.SelectMany(r => r.MissingGroups).Distinct();
		var mappingsToAdd = groupsToUpdate.ToDictionary(g => g, g => usersToUpdate.Where(r => r.MissingGroups.Contains(g)).Select(r => r.User.Username).ToArray());

		await communityManager.AddCommunityGroupMembers(mappingsToAdd);
		return await Index();
	}

	private async Task<ViewModel<CommunityAdmin>> GetMemberships()
	{
		var iteration = await iterationManager.ActiveIteration();
		var athletes = iteration.Athletes.OrderBy(a => a.Name).ToArray();

		var users = await communityManager.GetCommunityUsers();
		var groupMembers = config.CommunityGroups
			.ToDictionary(team => team.Key, async team => await communityManager.GetCommunityGroupMembers(team.Value));

		var globalGroup = await groupMembers[0];

		var rows = new List<CommunityAdmin.Row>();
		foreach (var athlete in athletes)
		{
			var user = Array.Find(users, u => athlete.LinkedAccounts.Any(a => a.Type == LinkedAccount.Keys.Discourse && a.Value == u.ID.ToString()));
			var currentGroups = new List<string>();
			var missingGroups = new List<string>();
			if (globalGroup.Contains(user))
			{
				currentGroups.Add(config.CommunityGroups[0]);
			}
			else
			{
				missingGroups.Add(config.CommunityGroups[0]);
			}

			var team = athlete.Team(iteration);
			var teamMembers = await groupMembers[team.Value];
			if (teamMembers.Contains(user))
			{
				currentGroups.Add(config.CommunityGroups[team.Value]);
			}
			else
			{
				missingGroups.Add(config.CommunityGroups[team.Value]);
			}

			var row = new CommunityAdmin.Row
			{
				Athlete = athlete,
				User = user,
				CurrentGroups = currentGroups.ToArray(),
				MissingGroups = missingGroups.ToArray()
			};
			rows.Add(row);
		}

		var data = new CommunityAdmin(iteration, rows.ToArray());
		return new ViewModel<CommunityAdmin>("Community Admin", data);
	}
}