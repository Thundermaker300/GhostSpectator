using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Permissions.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace GhostSpectator
{
    public enum GhostCoinType
    {
        TeleportSCP,
        TeleportHuman,
        TeleportGhost,
        TeleportRoom,
        TeleportSurface,
        SetToSpectator,
    }

    public static class CoinHandler
    {
        public static readonly Vector3 SurfacePosition = new Vector3(0, 1001.5f, 5.5f);

        public static ReadOnlyDictionary<GhostCoinType, string> CoinTranslation = new(new Dictionary<GhostCoinType, string>
        {
            { GhostCoinType.TeleportHuman, GhostSpectator.Translations.HumanTeleportCoin },
            { GhostCoinType.TeleportSCP, GhostSpectator.Translations.ScpTeleportCoin },
            { GhostCoinType.TeleportGhost, GhostSpectator.Translations.GhostTeleportCoin },
            { GhostCoinType.TeleportRoom, GhostSpectator.Translations.RoomTeleportCoin },
            { GhostCoinType.TeleportSurface, GhostSpectator.Translations.SurfaceTeleportCoin },
            { GhostCoinType.SetToSpectator, GhostSpectator.Translations.SetToSpectator },
        });

        public static Dictionary<ushort, GhostCoinType> Coins { get; } = new();

        public static void GiveCoins(Player ply)
        {
            foreach (var item in CoinTranslation)
            {
                GhostCoinType type = item.Key;

                if (type is GhostCoinType.SetToSpectator && GhostSpectator.Configs.RequirePermission && !ply.CheckPermission("gs.mode"))
                    continue;

                Item coin = ply.AddItem(ItemType.Coin);
                Coins.Add(coin.Serial, type);
            }
        }

        public static bool Execute(Player ply, Item coin)
        {
            if (ply is null || coin is null)
                return false;

            if (!Coins.TryGetValue(coin.Serial, out GhostCoinType type))
                return false;

            List<Player> list = null;
            if (type is GhostCoinType.TeleportHuman)
            {
                list = Player.Get(pl => pl.IsHuman && !API.IsGhost(pl)).ToList();
            }
            else if (type is GhostCoinType.TeleportSCP)
            {
                list = Player.Get(pl => pl.IsScp && !API.IsGhost(pl)).ToList();
            }
            else if (type is GhostCoinType.TeleportGhost)
            {
                list = Player.Get(pl => API.IsGhost(pl) && pl != ply).ToList();
            }
            else if (type is GhostCoinType.TeleportRoom)
            {
                if (Warhead.IsDetonated && GhostSpectator.Configs.DisableRoomTeleportAfterNuke)
                {
                    ply.ShowHint(GhostSpectator.Translations.NukeDisable);
                    return false;
                }

                var rooms = Room.List.Where(r => r.Type is not RoomType.Pocket).ToList();

                if (!GhostSpectator.Configs.LczTeleportAfterDecon && Map.IsLczDecontaminated)
                {
                    rooms.RemoveAll(r => r.Zone == ZoneType.LightContainment);
                }

                ply.Teleport(rooms.RandomItem());
                return true;
            }
            else if (type is GhostCoinType.TeleportSurface)
            {
                ply.Teleport(SurfacePosition);
                return true;
            }
            else if (type is GhostCoinType.SetToSpectator)
            {
                ply.Role.Set(PlayerRoles.RoleTypeId.Spectator, SpawnReason.ForceClass);
                return false;
            }

            if (list is not null && list.Count > 0)
            {
                Player random = list.ToList().RandomItem();
                ply.Teleport(random);
                ply.ShowHint(GhostSpectator.Translations.PlayerTeleport
                    .Replace("{PLAYER}", random.Nickname)
                    .Replace("{ROLE}", random.Role.Name)
                    .Replace("{ROLECOLOR}", random.Role.Color.ToHex()));
            }
            else
            {
                ply.ShowHint(GhostSpectator.Translations.NoPlayerToTeleport);
            }

            return list.Count > 0;
        }
    }
}
