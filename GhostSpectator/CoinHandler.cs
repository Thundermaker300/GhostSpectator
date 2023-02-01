using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GhostSpectator
{
    public enum GhostCoinType
    {
        TeleportSCP,
        TeleportHuman,
        TeleportRoom,
    }

    public static class CoinHandler
    {
        public const int CoinAmount = 3;

        public static ReadOnlyDictionary<GhostCoinType, string> CoinTranslation = new(new Dictionary<GhostCoinType, string>
        {
            { GhostCoinType.TeleportHuman, GhostSpectator.Translations.HumanTeleportCoin },
            { GhostCoinType.TeleportSCP, GhostSpectator.Translations.ScpTeleportCoin },
            { GhostCoinType.TeleportRoom, GhostSpectator.Translations.RoomTeleportCoin },
        });

        public static Dictionary<ushort, GhostCoinType> Coins { get; } = new();

        public static void GiveCoins(Player ply)
        {
            for (int i = 0; i < CoinAmount; i++)
            {
                Item coin = ply.AddItem(ItemType.Coin);
                Coins.Add(coin.Serial, (GhostCoinType)Enum.GetValues(typeof(GhostCoinType)).GetValue(i));
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
