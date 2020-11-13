using System;
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
            try
            {
                Player Ply = Player.Get(__instance._hub);
                if (Ply == null) return true;

                if (API.IsGhost(Ply))
                {
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return true;
            }
        }
    }
}
