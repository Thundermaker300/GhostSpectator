using HarmonyLib;
using Mirror;
using PlayerRoles.Voice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceChat;
using VoiceChat.Networking;

namespace GhostSpectator.Patches
{
    public class VoiceTransceiverPatch
    {
        [HarmonyPatch(nameof(VoiceTransceiver), nameof(VoiceTransceiver.ServerReceiveMessage))]
        public static bool Prefix(NetworkConnection conn, VoiceMessage msg)
        {
            if (msg.SpeakerNull || msg.Speaker.netId != conn.identity.netId || !(msg.Speaker.roleManager.CurrentRole is IVoiceRole voiceRole) || !voiceRole.VoiceModule.CheckRateLimit())
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
                    if (hub.roleManager.CurrentRole is IVoiceRole voiceRole2)
                    {
                        VoiceChatChannel voiceChatChannel2 = voiceRole2.VoiceModule.ValidateReceive(msg.Speaker, VoiceChatChannel.Spectator);
                        if (voiceChatChannel2 != 0)
                        {
                            msg.Channel = voiceChatChannel2;
                            hub.connectionToClient.Send(msg);
                        }
                    }
                }
                return false;
            }

            return true;
        }
    }
}
