using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;
using Exiled.API.Features;

namespace GhostSpectator.Patches
{
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdUseLocker))]
    class InteractingLockerPatch
    {
        [HarmonyPriority(Priority.High)]
        public static bool Prefix(PlayerInteract __instance, byte lockerId, byte chamberNumber)
        {
            Player Ply = Player.Get(__instance._hub);
            if (Ply == null) return true;

            if (API.IsGhost(Ply))
            {
                return false;
            }
            return true;
        }
    }
}
