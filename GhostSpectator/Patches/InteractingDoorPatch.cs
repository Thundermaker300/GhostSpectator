using HarmonyLib;
using Exiled.API.Features;

namespace GhostSpectator.Patches
{
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdOpenDoor))]
    class InteractingDoorPatch
    {
        [HarmonyPriority(Priority.High)]
        public static bool Prefix(PlayerInteract __instance)
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