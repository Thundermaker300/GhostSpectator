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
        public static Vector3 FindSpawnPosition(Player Ply, PlayerStats.HitInfo info)
        {
            if (Ply.Role == RoleType.Scp106 && info.GetDamageType() == DamageTypes.RagdollLess)
            {
                return Ply.Position + new Vector3(0, 5, 0);
            }
            else if (Ply.CurrentRoom.Type == RoomType.Pocket)
            {
                return Map.GetRandomSpawnPoint(RoleType.Scp106);
            }
            else
            {
                return Ply.Position;
            }
        }

        public static bool AreAllAlly(List<Player> List)
        {
            bool flag1 = true;
            bool flag2 = true;
            bool flag3 = true;
            foreach (Player Ply in List)
            {
                if (Ply.Team != Team.CDP && Ply.Team != Team.CHI && Ply.Team != Team.TUT && Ply.Team != Team.RIP)
                {
                    flag1 = false;
                }
                if (Ply.Team != Team.SCP && Ply.Team != Team.CHI && Ply.Team != Team.TUT && Ply.Team != Team.RIP)
                {
                    flag2 = false;
                }
                if (Ply.Team != Team.MTF && Ply.Team != Team.RSC && Ply.Team != Team.TUT && Ply.Team != Team.RIP)
                {
                    flag3 = false;
                }
            }
            return (flag1 || flag2 || flag3);
        }

        public static bool IsGhost(Player Ply) => GhostSpectator.Ghosts.Contains(Ply);

        public static void GhostPlayer(Player Ply)
        {
            if (GhostSpectator.Ghosts.Contains(Ply)) return;

            Ply.SetRole(GhostSpectator.Singleton.Config.GhostRole);
            Ply.ClearInventory();
            GhostSpectator.Ghosts.Add(Ply);

            Timing.CallDelayed(0.1f, () =>
            {
                Ply.NoClipEnabled = true;
                Ply.IsGodModeEnabled = true;
                Ply.IsInvisible = true;
            });
            if (GhostSpectator.Singleton.Config.GiveGhostNavigator == true)
            {
                Ply.Inventory.AddNewItem(ItemType.WeaponManagerTablet);
            }
            if (GhostSpectator.Singleton.Config.CanGhostsTeleport == true)
            {
                Ply.Inventory.AddNewItem(ItemType.Coin);
            }
            if (GhostSpectator.Singleton.Config.TriggerScps == false)
            {
                if (!Scp173.TurnedPlayers.Contains(Ply))
                {
                    Scp173.TurnedPlayers.Add(Ply);
                }
                if (!Scp096.TurnedPlayers.Contains(Ply))
                {
                    Scp096.TurnedPlayers.Add(Ply);
                }
            }
            if (GhostSpectator.SpawnPositions.ContainsKey(Ply))
            {
                Timing.CallDelayed(0.2f, () =>
                {
                    Ply.Position = GhostSpectator.SpawnPositions[Ply];
                    GhostSpectator.SpawnPositions.Remove(Ply);
                });
            }
            if (GhostSpectator.Singleton.Config.SpawnMessage != "none" && GhostSpectator.Singleton.Config.SpawnMessageLength > 0)
            {
                Ply.ClearBroadcasts();
                Ply.Broadcast((ushort)GhostSpectator.Singleton.Config.SpawnMessageLength, GhostSpectator.Singleton.Config.SpawnMessage);
            }
        }

        public static void UnGhostPlayer(Player Ply)
        {
            if (!GhostSpectator.Ghosts.Contains(Ply)) return;

            GhostSpectator.Ghosts.Remove(Ply);

            Timing.CallDelayed(0.1f, () =>
            {
                Ply.NoClipEnabled = false;
                Ply.IsGodModeEnabled = false;
                Ply.IsInvisible = false;
            });
            if (GhostSpectator.Singleton.Config.TriggerScps == false)
            {
                if (Scp173.TurnedPlayers.Contains(Ply))
                {
                    Scp173.TurnedPlayers.Remove(Ply);
                }
                if (Scp096.TurnedPlayers.Contains(Ply))
                {
                    Scp096.TurnedPlayers.Remove(Ply);
                }
            }
        }

        public static List<Player> GetPlayers(string data)
        {
            if (data == "*")
            {
                return Player.List.ToList();
            }
            else if (data.Contains("%"))
            {
                string searchFor = data.Remove(0, 1);
                if (!Enum.TryParse(searchFor, true, out RoleType role))
                {
                    return new List<Player> { };
                }
                return Player.List.Where(Ply => Ply.Role == role).ToList();
            }
            else if (data.Contains("*"))
            {
                string searchFor = data.Remove(0, 1);
                ZoneType zone = (searchFor.ToLower() == "light" ? ZoneType.LightContainment : (searchFor.ToLower() == "heavy" ? ZoneType.HeavyContainment : (searchFor.ToLower() == "entrance" ? ZoneType.Entrance : (searchFor.ToLower() == "surface" ? ZoneType.Surface : ZoneType.Unspecified))));
                if (zone == ZoneType.Unspecified)
                {
                    return new List<Player> { };
                }
                return Player.List.Where(Ply => Ply.CurrentRoom.Zone == zone).ToList();
            }
            else
            {
                List<Player> returnValue = new List<Player> { };
                string[] IDs = data.Split((".").ToCharArray());
                foreach (string id in IDs)
                {
                    Player Ply = Player.Get(id);
                    if (Ply != null)
                    {
                        returnValue.Add(Ply);
                    }
                }
                return returnValue;
            }
        }
    }
}
