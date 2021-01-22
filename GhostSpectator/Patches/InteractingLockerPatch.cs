using System;
using HarmonyLib;
using Exiled.API.Features;

namespace GhostSpectator.Patches
{
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdUseLocker))]
    internal static class InteractingLockerPatch
    {
        [HarmonyPriority(Priority.High)]
        private static bool Prefix(PlayerInteract __instance, byte lockerId, byte chamberNumber)
        {
            try
            {
                Player ply = Player.Get(__instance._hub);
                if (ply == null) return true;

                return !API.IsGhost(ply) || GhostSpectator.Singleton.Config.InteractLockers;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return true;
            }
        }
    }
}