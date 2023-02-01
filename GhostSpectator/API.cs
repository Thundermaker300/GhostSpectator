using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayerRoles;
using Exiled.API.Enums;
using Exiled.API.Features.Roles;
using UnityEngine;

namespace GhostSpectator
{
    public static class API
    {
        public static List<Player> Ghosts { get; } = new();
        public static List<Player> IsBecomingGhost { get; } = new();

        public static bool IsGhost(Player player) => Ghosts.Contains(player); //player.SessionVariables.ContainsKey("IsGhost");

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

            Ghosts.Add(ply);//ply.SessionVariables["IsGhost"] = true;
            ply.IsNoclipPermitted = true;
            ply.IsGodModeEnabled = true;

            if (!ply.Role.Is(out FpcRole fpcRole))
                return false;

            fpcRole.IsNoclipEnabled = true;

            ply.VoiceChannel = VoiceChat.VoiceChatChannel.Spectator;
            ply.CustomInfo = "GHOST SPECTATOR";
            ply.InfoArea &= ~PlayerInfoArea.Role;

            if (GhostSpectator.Configs.EnableCoins)
                CoinHandler.GiveCoins(ply);

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

            ply.VoiceChannel = VoiceChat.VoiceChatChannel.Proximity;
            ply.CustomInfo = string.Empty;
            ply.InfoArea |= PlayerInfoArea.Role;

            return true;
        }
    }
}
