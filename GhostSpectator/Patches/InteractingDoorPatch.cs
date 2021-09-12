using System;
using HarmonyLib;
using Exiled.API.Features;
using Interactables.Interobjects.DoorUtils;

namespace GhostSpectator.Patches
{
    [HarmonyPatch(typeof(DoorVariant), nameof(DoorVariant.ServerInteract), typeof(ReferenceHub), typeof(byte))]
    internal static class InteractingDoorPatch
    {
        [HarmonyPriority(Priority.High)]
        private static bool Prefix(PlayerInteract __instance, ReferenceHub ply, byte colliderId)
        {
            try
            {
                Player player = Player.Get(ply);
                if (player == null) return true;

                return !API.IsGhost(player) || GhostSpectator.Singleton.Config.InteractDoors;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return true;
            }
        }
    }
}