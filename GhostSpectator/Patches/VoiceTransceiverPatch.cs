namespace GhostSpectator.Patches
{
    using HarmonyLib;
    using Mirror;
    using PlayerRoles;
    using PlayerRoles.Voice;
    using VoiceChat;
    using VoiceChat.Networking;

    /// <summary>
    /// Patches voice chat so that ghosts can talk to each other cross-map.
    /// </summary>
    [HarmonyPatch(nameof(VoiceTransceiver), nameof(VoiceTransceiver.ServerReceiveMessage))]
    public class VoiceTransceiverPatch
    {
        public static bool Prefix(NetworkConnection conn, VoiceMessage msg)
        {
            if (msg.SpeakerNull || msg.Speaker.netId != conn.identity.netId || msg.Speaker.roleManager.CurrentRole is not IVoiceRole voiceRole || !voiceRole.VoiceModule.CheckRateLimit())
            {
                return false;
            }

            VcMuteFlags flags = VoiceChatMutes.GetFlags(msg.Speaker);
            if (flags == VcMuteFlags.GlobalRegular || flags == VcMuteFlags.LocalRegular)
            {
                return false;
            }

            if (API.IsGhost(msg.Speaker))
            {
                foreach (var hub in ReferenceHub.AllHubs)
                {
                    if ((hub != msg.Speaker && API.IsGhost(hub)) || hub.roleManager.CurrentRole.RoleTypeId is RoleTypeId.Spectator or RoleTypeId.Overwatch)
                    {
                        msg.Channel = VoiceChatChannel.RoundSummary;
                        hub.connectionToClient.Send(msg);
                    }
                }

                return false;
            }

            return true;
        }
    }
}
