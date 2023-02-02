namespace GhostSpectator
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Permissions.Extensions;
    using UnityEngine;

    /// <summary>
    /// Type of ghost coin ability.
    /// </summary>
    public enum GhostCoinType
    {
        /// <summary>
        /// Teleport to a random SCP.
        /// </summary>
        TeleportSCP,

        /// <summary>
        /// Teleport to a random human.
        /// </summary>
        TeleportHuman,

        /// <summary>
        /// Teleport to a random other ghost.
        /// </summary>
        TeleportGhost,

        /// <summary>
        /// Teleport to a random room.
        /// </summary>
        TeleportRoom,

        /// <summary>
        /// Teleport to the surface.
        /// </summary>
        TeleportSurface,

        /// <summary>
        /// Swap to spectator.
        /// </summary>
        SetToSpectator,
    }

    /// <summary>
    /// Manages ghost coin abilities.
    /// </summary>
    public static class CoinHandler
    {
        /// <summary>
        /// Fixed surface position to teleport ghosts to.
        /// </summary>
        public static readonly Vector3 SurfacePosition = new Vector3(0, 1001.5f, 5.5f);

        /// <summary>
        /// Gets each coin and its respective translation.
        /// </summary>
        public static ReadOnlyDictionary<GhostCoinType, string> CoinTranslation { get; } = new(new Dictionary<GhostCoinType, string>
        {
            { GhostCoinType.TeleportHuman, GhostSpectator.Translations.HumanTeleportCoin },
            { GhostCoinType.TeleportSCP, GhostSpectator.Translations.ScpTeleportCoin },
            { GhostCoinType.TeleportGhost, GhostSpectator.Translations.GhostTeleportCoin },
            { GhostCoinType.TeleportRoom, GhostSpectator.Translations.RoomTeleportCoin },
            { GhostCoinType.TeleportSurface, GhostSpectator.Translations.SurfaceTeleportCoin },
            { GhostCoinType.SetToSpectator, GhostSpectator.Translations.SetToSpectator },
        });

        /// <summary>
        /// Gets each coin currently in distribution, keyed by <see cref="Item.Serial"/>.
        /// </summary>
        public static Dictionary<ushort, GhostCoinType> Coins { get; } = new();

        /// <summary>
        /// Gives coins to a player.
        /// </summary>
        /// <param name="ply">Player.</param>
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

        /// <summary>
        /// Executes a coin effect.
        /// </summary>
        /// <param name="ply">The player.</param>
        /// <param name="coin">The coin being flipped.</param>
        /// <returns>True if successful.</returns>
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

                Room r = rooms.RandomItem();

                ply.Teleport(r);
                ply.ShowHint(GhostSpectator.Translations.RoomTeleport
                    .Replace("{ROOM}", r.Type.ToString()));

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
