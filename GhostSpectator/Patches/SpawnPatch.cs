using System.Collections.Generic;

using HarmonyLib;
using NorthwoodLib.Pools;
using Respawning;
using UnityEngine;
using Exiled.API.Features;
using System.Linq;
using System;

namespace GhostSpectator.Patches
{
	[HarmonyPatch(typeof(RespawnManager), nameof(RespawnManager.Spawn))]
	class SpawnPatch
	{
		[HarmonyPriority(Priority.High)]
		public static bool Prefix(RespawnManager __instance)
		{
			SpawnableTeam spawnableTeam;
			if (!RespawnWaveGenerator.SpawnableTeams.TryGetValue(__instance.NextKnownTeam, out spawnableTeam) || __instance.NextKnownTeam == SpawnableTeamType.None)
			{
				ServerConsole.AddLog("Fatal error. Team '" + __instance.NextKnownTeam + "' is undefined.", ConsoleColor.Red);
				return false;
			}
			List<ReferenceHub> list = Player.List.Where(Ply => (Ply.IsDead && !Ply.IsOverwatchEnabled) || API.IsGhost(Ply)).Select(Ply => Ply.ReferenceHub).ToList();
			if (__instance._prioritySpawn)
			{
				list = (from item in list
						orderby item.characterClassManager.DeathTime
						select item).ToList<ReferenceHub>();
			}
			else
			{
				list.ShuffleList();
			}
			int num = RespawnTickets.Singleton.GetAvailableTickets(__instance.NextKnownTeam);
			if (num == 0)
			{
				num = 5;
				RespawnTickets.Singleton.GrantTickets(SpawnableTeamType.ChaosInsurgency, 5, true);
			}
			int num2 = Mathf.Min(num, spawnableTeam.MaxWaveSize);
			while (list.Count > num2)
			{
				list.RemoveAt(list.Count - 1);
			}
			list.ShuffleList();
			List<ReferenceHub> list2 = ListPool<ReferenceHub>.Shared.Rent();
			foreach (ReferenceHub referenceHub in list)
			{
				try
				{
					RoleType classid = spawnableTeam.ClassQueue[Mathf.Min(list2.Count, spawnableTeam.ClassQueue.Length - 1)];
					referenceHub.characterClassManager.SetPlayersClass(classid, referenceHub.gameObject, false, false);
					list2.Add(referenceHub);
					ServerLogs.AddLog(ServerLogs.Modules.ClassChange, string.Concat(new string[]
					{
						"Player ",
						referenceHub.LoggedNameFromRefHub(),
						" respawned as ",
						classid.ToString(),
						"."
					}), ServerLogs.ServerLogType.GameEvent, false);
				}
				catch (Exception ex)
				{
					if (referenceHub != null)
					{
						ServerLogs.AddLog(ServerLogs.Modules.ClassChange, "Player " + referenceHub.LoggedNameFromRefHub() + " couldn't be spawned. Err msg: " + ex.Message, ServerLogs.ServerLogType.GameEvent, false);
					}
					else
					{
						ServerLogs.AddLog(ServerLogs.Modules.ClassChange, "Couldn't spawn a player - target's ReferenceHub is null.", ServerLogs.ServerLogType.GameEvent, false);
					}
				}
			}
			if (list2.Count > 0)
			{
				ServerLogs.AddLog(ServerLogs.Modules.ClassChange, string.Concat(new object[]
				{
					"RespawnManager has successfully spawned ",
					list2.Count,
					" players as ",
					__instance.NextKnownTeam.ToString(),
					"!"
				}), ServerLogs.ServerLogType.GameEvent, false);
				RespawnTickets.Singleton.GrantTickets(__instance.NextKnownTeam, -list2.Count * spawnableTeam.TicketRespawnCost, false);
				Respawning.NamingRules.UnitNamingRule unitNamingRule;
				if (Respawning.NamingRules.UnitNamingRules.TryGetNamingRule(__instance.NextKnownTeam, out unitNamingRule))
				{
					string text;
					unitNamingRule.GenerateNew(__instance.NextKnownTeam, out text);
					foreach (ReferenceHub referenceHub2 in list2)
					{
						referenceHub2.characterClassManager.NetworkCurSpawnableTeamType = (byte)__instance.NextKnownTeam;
						referenceHub2.characterClassManager.NetworkCurUnitName = text;
					}
					SCP_2536_Controller.singleton.TotalSpawnedPlayersByUnitName.Add(text, list2.Count);
					unitNamingRule.PlayEntranceAnnouncement(text);
				}
				RespawnEffectsController.ExecuteAllEffects(RespawnEffectsController.EffectType.UponRespawn, __instance.NextKnownTeam);
			}
			ListPool<ReferenceHub>.Shared.Return(list2);
			__instance.NextKnownTeam = SpawnableTeamType.None;

			return false;
		}
	}
}
