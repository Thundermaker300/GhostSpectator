namespace GhostSpectator
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using MEC;
    using PlayerRoles;
    using UnityEngine;

    /// <summary>
    /// API class for controlling ghosts.
    /// </summary>
    public static class API
    {
        /// <summary>
        /// Gets all players that are becoming ghosts.
        /// </summary>
        public static List<Player> IsBecomingGhost { get; } = new List<Player>();

        /// <summary>
        /// Gets each player and the last place they died.
        /// </summary>
        public static Dictionary<Player, Vector3> LastDiedPosition { get; } = new();

        /// <summary>
        /// Gets since the player was last a ghost. Removed when becoming a ghost.
        /// </summary>
        public static Dictionary<Player, DateTime> TimeSinceGhostLast { get; } = new();

        /// <summary>
        /// Gets a value indicating whether or not the player is a ghost.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <returns>True if they are a ghost.</returns>
        public static bool IsGhost(Player player)
        {
            if (player is null)
                return false;

            return player.SessionVariables.ContainsKey("IsGhost");
        }

        /// <summary>
        /// Gets a value indicating whether or not the player is a ghost.
        /// </summary>
        /// <param name="player">ReferenceHub.</param>
        /// <returns>True if they are a ghost.</returns>
        public static bool IsGhost(ReferenceHub player)
        {
            if (player is null)
                return false;

            return IsGhost(Player.Get(player));
        }

        /// <summary>
        /// Turns a player into a ghost.
        /// </summary>
        /// <param name="ply">The player.</param>
        /// <param name="ignoreChecks">Whether to ignore basic checks.</param>
        /// <returns>Whether or not transformation was successful.</returns>
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

            ply.Role.Set(RoleTypeId.Tutorial, SpawnReason.ForceClass, RoleSpawnFlags.AssignInventory);

            if (ply.Role.Is(out FpcRole fpc))
            {
                fpc.IsInvisible = true;
                Timing.CallDelayed(0.25f, () =>
                {
                    if (Warhead.IsDetonated)
                    {
                        ply.Teleport(CoinHandler.SurfacePosition);
                    }
                    else if (LastDiedPosition.TryGetValue(ply, out Vector3 pos))
                    {
                        ply.Teleport(pos);
                    }
                    else
                    {
                        ply.Teleport(CoinHandler.SurfacePosition);
                    }

                    // fpc.IsInvisible = false;
                });
            }

            if (GhostSpectator.Configs.GhostBroadcast is not null)
                ply.Broadcast(GhostSpectator.Configs.GhostBroadcast);

            if (TimeSinceGhostLast.ContainsKey(ply))
                TimeSinceGhostLast.Remove(ply);

            ply.SessionVariables["IsGhost"] = true;
            ply.IsUsingStamina = false;
            ply.IsNoclipPermitted = true;
            ply.IsGodModeEnabled = true;

            Scp096Role.TurnedPlayers.Add(ply);
            Scp173Role.TurnedPlayers.Add(ply);

            ply.EnableEffect(EffectType.MovementBoost);
            ply.ChangeEffectIntensity(EffectType.MovementBoost, 50);

            ply.CustomInfo = "GHOST SPECTATOR";
            ply.InfoArea &= ~PlayerInfoArea.Role;

            ply.ChangeAppearance(RoleTypeId.Spectator, false);

            /*foreach (Player all in Player.List)
            {
                if (!IsGhost(all))
                {
                    all.Connection.Send(new RoleSyncInfo(ply.ReferenceHub, RoleTypeId.Spectator, all.ReferenceHub));
                }
            }*/

            if (GhostSpectator.Configs.EnableCoins)
                CoinHandler.GiveCoins(ply);

            foreach (var player in Player.List)
                EventHandler.CheckPlayer(ply, player);

            return true;
        }

        /// <summary>
        /// Removes ghost effects.
        /// </summary>
        /// <param name="ply">The player.</param>
        /// <param name="ignoreChecks">Whether to ignore basic checks.</param>
        /// <returns>Whether or not transformation was successful.</returns>
        public static bool UnGhostify(Player ply, bool ignoreChecks = false)
        {
            if (!IsGhost(ply)) return false;

            Log.Debug($"Unghosting: {ply.Nickname}");

            if (TimeSinceGhostLast.ContainsKey(ply))
                TimeSinceGhostLast[ply] = DateTime.UtcNow;
            else
                TimeSinceGhostLast.Add(ply, DateTime.UtcNow);

            if (ply.SessionVariables.ContainsKey("IsGhost"))
                ply.SessionVariables.Remove("IsGhost");

            ply.IsUsingStamina = true;
            ply.IsNoclipPermitted = false;
            ply.IsGodModeEnabled = false;

            Scp096Role.TurnedPlayers.Add(ply);
            Scp173Role.TurnedPlayers.Remove(ply);

            ply.DisableEffect(EffectType.MovementBoost);
            ply.CustomInfo = string.Empty;
            ply.InfoArea |= PlayerInfoArea.Role;

            if (!ply.Role.Is(out FpcRole fpcRole))
                return true;

            fpcRole.IsNoclipEnabled = false;

            return true;
        }
    }
}
