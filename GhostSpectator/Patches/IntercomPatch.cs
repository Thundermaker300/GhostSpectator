using HarmonyLib;
using PlayerRoles.Voice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostSpectator.Patches
{
    [HarmonyPatch(typeof(Intercom), nameof(Intercom.CheckRange))]
    public class IntercomPatch
    {
        public static bool Prefix(Intercom __instance, ReferenceHub hub)
        {
            return !API.IsGhost(hub);
        }
    }
}
