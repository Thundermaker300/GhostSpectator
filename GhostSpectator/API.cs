using System;
using System.Collections.Generic;
using System.Linq;

using Exiled.API.Features;
using Exiled.API.Enums;
using MEC;
using UnityEngine;

namespace GhostSpectator
{
    public class API
    {
        public static Vector3 FindSpawnPosition(Player ply, DamageHandler info)
        {
            if (ply.Role == RoleType.Scp106 && info.Type == DamageType.Recontainment)
            {
                if (PlayerMovementSync.FindSafePosition(ply.Position, out Vector3 safePos))
                {
                    return safePos;
                }
                else
                {
                    return ply.Position + new Vector3(0, 5, 0);
                }
            }
            else if (ply.CurrentRoom.Type == RoomType.Pocket)
            {
                return new Vector3(0, -1998.67f, 2);
            }
            else if (ply.Role == RoleType.Spectator || ply.Role == RoleType.None)
            {
                return new Vector3(0, 1001, 8);
            }
            else
            {
                return ply.Position;
            }
        }

        public static bool AreAllAlly(List<Player> list)
        {
            bool flag1 = true;
            bool flag2 = true;
            bool flag3 = true;
            foreach (Player ply in list)
            {
                if (ply.Team != Team.CDP && ply.Team != Team.CHI && ply.Team != Team.TUT && ply.Team != Team.RIP)
                {
                    flag1 = false;
                }

                if (ply.Team != Team.SCP && ply.Team != Team.CHI && ply.Team != Team.TUT && ply.Team != Team.RIP)
                {
                    flag2 = false;
                }

                if (ply.Team != Team.MTF && ply.Team != Team.RSC && ply.Team != Team.TUT && ply.Team != Team.RIP)
                {
                    flag3 = false;
                }
            }

            return flag1 || flag2 || flag3;
        }

        public static bool IsGhost(Player ply)
        {
            if (ply == null)
                return false;
            return GhostSpectator.Ghosts.Contains(ply) || ply.TryGetSessionVariable<bool>("IsGhost", out _);
        }

        public static void GhostPlayer(Player ply)
        {
            if (GhostSpectator.Ghosts.Contains(ply)) return;

            ply.SetRole(GhostSpectator.Singleton.Config.GhostRole);
            ply.SessionVariables.Add("IsGhost", true);
            GhostSpectator.Ghosts.Add(ply);

            ply.ReferenceHub.nicknameSync.CustomPlayerInfo = "GHOST";
            ply.ReferenceHub.nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Role;

            Timing.CallDelayed(0.1f, () =>
            {
                ply.NoClipEnabled = true;
                ply.IsGodModeEnabled = true;
                ply.IsInvisible = true;
            });
            /*foreach (Player AlivePly in Player.List.Where(P => P.IsAlive == true && !IsGhost(P)))
            {
                if (!AlivePly.TargetGhostsHashSet.Contains(Ply.Id))
                {
                    AlivePly.TargetGhostsHashSet.Add(Ply.Id);
                }
            }*/
            Timing.CallDelayed(2.2f, () => // Prevent other plugins from giving items to ghosts
            {
                ply.ClearInventory();
                if (GhostSpectator.Singleton.Config.GiveGhostNavigator == true)
                {
                    ply.AddItem(ItemType.Flashlight);
                }

                if (GhostSpectator.Singleton.Config.CanGhostsTeleport == true)
                {
                    ply.AddItem(ItemType.Coin);
                }
            });
            if (!GhostSpectator.Singleton.Config.TriggerScps)
            {
                if (!Scp173.TurnedPlayers.Contains(ply))
                {
                    Scp173.TurnedPlayers.Add(ply);
                }

                if (!Scp096.TurnedPlayers.Contains(ply))
                {
                    Scp096.TurnedPlayers.Add(ply);
                }
            }

            if (GhostSpectator.SpawnPositions.ContainsKey(ply))
            {
                Timing.CallDelayed(0.2f, () =>
                {
                    ply.Position = GhostSpectator.SpawnPositions[ply];
                    GhostSpectator.SpawnPositions.Remove(ply);
                });
            }

            if (GhostSpectator.Singleton.Config.SpawnMessage != "none" &&
                GhostSpectator.Singleton.Config.SpawnMessageLength > 0)
            {
                ply.ClearBroadcasts();
                ply.Broadcast((ushort) GhostSpectator.Singleton.Config.SpawnMessageLength,
                    GhostSpectator.Singleton.Config.SpawnMessage);
            }
        }

        public static void UnGhostPlayer(Player ply)
        {
            if (!GhostSpectator.Ghosts.Contains(ply)) return;

            ply.SessionVariables.Remove("IsGhost");
            GhostSpectator.Ghosts.Remove(ply);

            ply.CustomInfo = string.Empty;
            ply.ReferenceHub.nicknameSync.ShownPlayerInfo |= PlayerInfoArea.Role;

            Timing.CallDelayed(0.1f, () =>
            {
                ply.NoClipEnabled = false;
                ply.IsGodModeEnabled = false;
                ply.IsInvisible = false;
            });
            /*foreach (Player AlivePly in Player.List)
            {
                if (Ply.TargetGhostsHashSet.Contains(AlivePly.Id))
                {
                    Ply.TargetGhostsHashSet.Remove(AlivePly.Id);
                }
            }*/
            if (GhostSpectator.Singleton.Config.TriggerScps) return;
            if (Scp173.TurnedPlayers.Contains(ply))
            {
                Scp173.TurnedPlayers.Remove(ply);
            }

            if (Scp096.TurnedPlayers.Contains(ply))
            {
                Scp096.TurnedPlayers.Remove(ply);
            }
        }

        public static List<Player> GetPlayers(string data)
        {
            if (data == "*")
            {
                return Player.List.ToList();
            }

            if (data.Contains("%"))
            {
                string searchFor = data.Remove(0, 1);
                return !Enum.TryParse(searchFor, true, out RoleType role)
                    ? new List<Player>()
                    : Player.Get(role).ToList();
            }

            if (data.Contains("*"))
            {
                string searchFor = data.Remove(0, 1);
                ZoneType zone = searchFor.ToLower() == "light"
                    ? ZoneType.LightContainment
                    : searchFor.ToLower() == "heavy"
                        ? ZoneType.HeavyContainment
                        : searchFor.ToLower() == "entrance"
                            ? ZoneType.Entrance
                            : searchFor.ToLower() == "surface"
                                ? ZoneType.Surface
                                : ZoneType.Unspecified;
                return zone == ZoneType.Unspecified
                    ? new List<Player>()
                    : Player.List.Where(ply => ply.CurrentRoom.Zone == zone).ToList();
            }

            List<Player> returnValue = new List<Player>();
            string[] ids = data.Split((".").ToCharArray());
            foreach (string id in ids)
            {
                Player ply = Player.Get(id);
                if (ply != null)
                {
                    returnValue.Add(ply);
                }
            }

            return returnValue;
        }
    }
}