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
        TeleportRoom,
        TeleportSurface,
        SetToSpectator,
    }

    public static class CoinHandler
    {
        public const int CoinAmount = 4;
        public static readonly Vector3 SurfacePosition = Room.Get(RoomType.Surface).Position; // Todo: Add custom position;

        public static ReadOnlyDictionary<GhostCoinType, string> CoinTranslation = new(new Dictionary<GhostCoinType, string>
        {
            { GhostCoinType.TeleportHuman, GhostSpectator.Translations.HumanTeleportCoin },
            { GhostCoinType.TeleportSCP, GhostSpectator.Translations.ScpTeleportCoin },
            { GhostCoinType.TeleportRoom, GhostSpectator.Translations.RoomTeleportCoin },
            { GhostCoinType.TeleportSurface, GhostSpectator.Translations.SurfaceTeleportCoin },
            { GhostCoinType.SetToSpectator, GhostSpectator.Translations.SetToSpectator },
        });

        public static Dictionary<ushort, GhostCoinType> Coins { get; } = new();

        public static void GiveCoins(Player ply)
        {
            for (int i = 0; i < CoinAmount; i++)
            {
                GhostCoinType type = (GhostCoinType)Enum.GetValues(typeof(GhostCoinType)).GetValue(i);

                if (type is GhostCoinType.SetToSpectator && GhostSpectator.Configs.RequirePermission && !ply.CheckPermission("gs.mode"))
                    continue;

                Item coin = ply.AddItem(ItemType.Coin);
                Coins.Add(coin.Serial, type);
            }
        }

        public static void Execute(Player ply, Item coin)
        {
            if (ply is null || coin is null)
                return;

            if (!Coins.TryGetValue(coin.Serial, out GhostCoinType type))
                return;

            List<Player> list = null;
            if (type is GhostCoinType.TeleportHuman)
            {
                list = Player.Get(pl => pl.IsHuman && !API.IsGhost(pl)).ToList();
            }
            else if (type is GhostCoinType.TeleportSCP)
            {
                list = Player.Get(pl => pl.IsScp && !API.IsGhost(pl)).ToList();
            }
            else if (type is GhostCoinType.TeleportRoom)
            {
                if (Warhead.IsDetonated && GhostSpectator.Configs.DisableRoomTeleportAfterNuke)
                {
                    ply.ShowHint(GhostSpectator.Translations.NukeDisable);
                    return;
                }

                var rooms = Room.List.Where(r => r.Type is not RoomType.Pocket).ToList();

                if (!GhostSpectator.Configs.LczTeleportAfterDecon && Map.IsLczDecontaminated)
                {
                    rooms.RemoveAll(r => r.Zone == ZoneType.LightContainment);
                }

                ply.Teleport(rooms.RandomItem());
                return;
            }
            else if (type is GhostCoinType.TeleportSurface)
            {
                ply.Teleport(SurfacePosition);
                return;
            }
            else if (type is GhostCoinType.SetToSpectator)
            {
                ply.Role.Set(PlayerRoles.RoleTypeId.Spectator, SpawnReason.ForceClass);
                return;
            }

            if (list is not null && list.Count > 0)
            {
                Player random = list.ToList().RandomItem();
                ply.Teleport(random);
                ply.ShowHint(GhostSpectator.Translations.PlayerTeleport
                    .Replace("{PLAYER}", random.Nickname)
                    .Replace("{ROLE}", random.Role.Name)
                    .Replace("{ROLECOLOR}", random.Role.Color.ToString()));
            }
            else
            {
                ply.ShowHint(GhostSpectator.Translations.NoPlayerToTeleport);
            }
        }
    }
}
