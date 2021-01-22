using System;
using HarmonyLib;
using Exiled.API.Features;
using Interactables.Interobjects.DoorUtils;

namespace GhostSpectator.Patches
{
    [HarmonyPatch(typeof(DoorVariant), nameof(DoorVariant.ServerInteract), typeof(ReferenceHub), typeof(byte))]
    class InteractingDoorPatch
    {
        [HarmonyPriority(Priority.High)]
        public static bool Prefix(PlayerInteract __instance)
        {
            try
            {
                Player Ply = Player.Get(__instance._hub);
                if (Ply == null) return true;

                if (API.IsGhost(Ply) && !GhostSpectator.Singleton.Config.InteractDoors)
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