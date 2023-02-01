﻿using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayerRoles;
using Exiled.API.Enums;
using Exiled.API.Features.Roles;
using UnityEngine;
using MEC;

namespace GhostSpectator
{
    public static class API
    {
        public static List<Player> Ghosts { get; } = new List<Player>();
        public static List<Player> IsBecomingGhost { get; } = new List<Player>();
        public static Dictionary<Player, Vector3> LastDiedPosition { get; } = new();

        public static bool IsGhost(Player player) => Ghosts.Contains(player); //player.SessionVariables.ContainsKey("IsGhost");
        public static bool IsGhost(ReferenceHub player) => IsGhost(Player.Get(player)); //player.SessionVariables.ContainsKey("IsGhost");

        public static bool Ghostify(Player ply, bool ignoreChecks = false)
        {
            if (IsGhost(ply)) return false;

            if (!ignoreChecks)
            {
                if (ply.IsOverwatchEnabled)
                    return false;
            }

            Log.Debug($"Ghosting: {ply.Nickname}");
            IsBecomingGhost.Add(ply);

            ply.Role.Set(RoleTypeId.Tutorial, SpawnReason.ForceClass, RoleSpawnFlags.None);

            if (ply.Role.Is(out FpcRole fpc))
            {
                fpc.IsInvisible = true;
                Timing.CallDelayed(0.25f, () =>
                {
                    if (LastDiedPosition.TryGetValue(ply, out Vector3 pos))
                        ply.Teleport(pos);
                    fpc.IsInvisible = false;
                });
            }

            Ghosts.Add(ply);//ply.SessionVariables["IsGhost"] = true;
            ply.IsNoclipPermitted = true;
            ply.IsGodModeEnabled = true;

            Scp173Role.TurnedPlayers.Add(ply);

            //fpcRole.IsNoclipEnabled = true;

            ply.EnableEffect(EffectType.MovementBoost);
            ply.ChangeEffectIntensity(EffectType.MovementBoost, 50);
            ply.CustomInfo = "GHOST SPECTATOR";
            ply.InfoArea &= ~PlayerInfoArea.Role;

            if (GhostSpectator.Configs.EnableCoins)
                CoinHandler.GiveCoins(ply);

            foreach (var player in Player.List)
                EventHandler.CheckPlayer(ply, player);

            return true;
        }

        public static bool UnGhostify(Player ply, bool ignoreChecks = false)
        {
            if (!IsGhost(ply)) return false;

            Log.Debug($"Unghosting: {ply.Nickname}");
            Ghosts.Remove(ply);//ply.SessionVariables.Remove("IsGhost");

            ply.IsNoclipPermitted = false;

            if (!ply.Role.Is(out FpcRole fpcRole))
                return false;

            fpcRole.IsNoclipEnabled = false;
            ply.IsGodModeEnabled = false;

            Scp173Role.TurnedPlayers.Remove(ply);

            ply.DisableEffect(EffectType.MovementBoost);
            ply.CustomInfo = string.Empty;
            ply.InfoArea |= PlayerInfoArea.Role;

            return true;
        }
    }
}
