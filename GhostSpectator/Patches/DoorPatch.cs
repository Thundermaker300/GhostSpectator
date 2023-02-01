using Exiled.API.Features;
using HarmonyLib;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;

namespace GhostSpectator.Patches
{
    public static class DoorChecker
    {
        public static bool Check(ReferenceHub ply, ref bool __result)
        {
            if (ply is null)
                return true;

            if (API.IsGhost(ply))
            {
                __result = false;
                return false;
            }

            return true;
        }
    }

    // Required in order for doors to not make denied sound.
    // POV: You can't patch abstract methods
    // this is stupid
    [HarmonyPatch(typeof(BreakableDoor), nameof(BreakableDoor.AllowInteracting))]
    public class BreakableDoorPatch
    {
        internal static bool Prefix(BreakableDoor __instance, ReferenceHub ply, byte colliderId, ref bool __result)
            => DoorChecker.Check(ply, ref __result);
    }

    [HarmonyPatch(typeof(BasicDoor), nameof(BasicDoor.AllowInteracting))]
    public class BasicDoorPatch
    {
        internal static bool Prefix(BasicDoor __instance, ReferenceHub ply, byte colliderId, ref bool __result)
            => DoorChecker.Check(ply, ref __result);
    }

    [HarmonyPatch(typeof(CheckpointDoor), nameof(CheckpointDoor.AllowInteracting))]
    public class CheckpointDoorPatch
    {
        internal static bool Prefix(CheckpointDoor __instance, ReferenceHub ply, byte colliderId, ref bool __result)
            => DoorChecker.Check(ply, ref __result);
    }


    [HarmonyPatch(typeof(PryableDoor), nameof(PryableDoor.AllowInteracting))]
    public class PryableDoorPatch
    {
        internal static bool Prefix(PryableDoor __instance, ReferenceHub ply, byte colliderId, ref bool __result)
            => DoorChecker.Check(ply, ref __result);
    }

    [HarmonyPatch(typeof(AirlockController), nameof(AirlockController.OnDoorAction))]
    public class AirlockPatch
    {
        internal static bool Prefix(DoorVariant door, DoorAction action, ReferenceHub ply)
        {
            if (ply is not null && API.IsGhost(ply))
                return false;

            return true;
        }
    }
}
