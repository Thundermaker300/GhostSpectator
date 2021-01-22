using System.Collections.Generic;

using HarmonyLib;
using NorthwoodLib.Pools;
using Respawning;
using UnityEngine;
using Random = UnityEngine.Random;
using Exiled.API.Features;

namespace GhostSpectator.Patches
{
    [HarmonyPatch(typeof(RespawnTickets), nameof(RespawnTickets.DrawRandomTeam))]
    internal static class DrawRandomTeamPatch
    {
        private static bool Prefix(RespawnTickets __instance, ref SpawnableTeamType __result)
        {
            bool flag = false;
            foreach (KeyValuePair<GameObject, ReferenceHub> allHub in ReferenceHub.GetAllHubs())
            {
                if ((allHub.Value.characterClassManager.CurClass == RoleType.Spectator &&
                     !allHub.Value.serverRoles.OverwatchEnabled) || API.IsGhost(Player.Get(allHub.Value)))
                {
                    flag = true;
                    break;
                }
            }

            if (!flag)
                __result = SpawnableTeamType.None;
            List<SpawnableTeamType> list1 = ListPool<SpawnableTeamType>.Shared.Rent();
            List<SpawnableTeamType> list2 = ListPool<SpawnableTeamType>.Shared.Rent();
            foreach (KeyValuePair<SpawnableTeamType, int> ticket in __instance._tickets)
            {
                if (ticket.Value > 0)
                {
                    for (int index = 0; index < ticket.Value; ++index)
                        list1.Add(ticket.Key);
                }
                else
                {
                    if (ticket.Value == 0 &&
                        RespawnWaveGenerator.SpawnableTeams.TryGetValue(ticket.Key, out var spawnableTeam) &&
                        spawnableTeam.LockUponZero)
                        list2.Add(ticket.Key);
                }
            }

            while (list2.Count > 0)
            {
                __instance._tickets[list2[0]] = -1;
                list2.RemoveAt(0);
            }

            int num = list1.Count == 0 ? 1 : (int) list1[Random.Range(0, list1.Count)];
            ListPool<SpawnableTeamType>.Shared.Return(list1);
            ListPool<SpawnableTeamType>.Shared.Return(list2);
            __result = (SpawnableTeamType) num;
            return false;
        }
    }
}