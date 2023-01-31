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
        public static List<Player> IsBecomingGhost { get; } = new();

        public static bool IsGhost(Player player) => player.SessionVariables.ContainsKey("IsGhost");

        public static bool Ghostify(Player ply, bool ignoreChecks = false)
        {
            if (!ignoreChecks)
            {
                if (!ply.IsDead || ply.IsOverwatchEnabled)
                    return false;
            }

            IsBecomingGhost.Add(ply);

            ply.SessionVariables["IsGhost"] = true;
            ply.IsNoclipPermitted = true;
            ply.IsGodModeEnabled = true;

            if (!ply.Role.Is(out FpcRole fpcRole))
                return false;

            fpcRole.IsNoclipEnabled = true;

            ply.VoiceChannel = VoiceChat.VoiceChatChannel.Spectator;

            return true;
        }

        public static bool UnGhostify(Player ply, bool ignoreChecks = false)
        {
            if (ply.SessionVariables.ContainsKey("IsGhost"))
                ply.SessionVariables.Remove("IsGhost");

            ply.IsNoclipPermitted = false;

            if (!ply.Role.Is(out FpcRole fpcRole))
                return false;

            fpcRole.IsNoclipEnabled = false;
            ply.IsGodModeEnabled = false;

            ply.VoiceChannel = VoiceChat.VoiceChatChannel.Proximity;

            return true;
        }
    }
}
