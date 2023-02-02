namespace GhostSpectator.Patches
{
#pragma warning disable SA1313
#pragma warning disable SA1402
#pragma warning disable SA1649
    using HarmonyLib;
    using Interactables.Interobjects;
    using Interactables.Interobjects.DoorUtils;

    /// <summary>
    /// Contains a method used by every door checking patch.
    /// </summary>
    public static class DoorChecker
    {
        /// <summary>
        /// Returns the info to be returned in the door patch.
        /// </summary>
        /// <param name="ply">The player interacting with the door.</param>
        /// <param name="__result">The result of the patch.</param>
        /// <returns>Whether or not original code executes.</returns>
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

    /// <summary>
    /// Patch for breakable door.
    /// </summary>
    [HarmonyPatch(typeof(BreakableDoor), nameof(BreakableDoor.AllowInteracting))]
    public class BreakableDoorPatch
    {
        internal static bool Prefix(BreakableDoor __instance, ReferenceHub ply, byte colliderId, ref bool __result)
            => DoorChecker.Check(ply, ref __result);
    }

    /// <summary>
    /// Patch for basic door.
    /// </summary>
    [HarmonyPatch(typeof(BasicDoor), nameof(BasicDoor.AllowInteracting))]
    public class BasicDoorPatch
    {
        internal static bool Prefix(BasicDoor __instance, ReferenceHub ply, byte colliderId, ref bool __result)
            => DoorChecker.Check(ply, ref __result);
    }

    /// <summary>
    /// Patch for checkpoint door.
    /// </summary>
    [HarmonyPatch(typeof(CheckpointDoor), nameof(CheckpointDoor.AllowInteracting))]
    public class CheckpointDoorPatch
    {
        internal static bool Prefix(CheckpointDoor __instance, ReferenceHub ply, byte colliderId, ref bool __result)
            => DoorChecker.Check(ply, ref __result);
    }

    /// <summary>
    /// Patch for pryable door.
    /// </summary>
    [HarmonyPatch(typeof(PryableDoor), nameof(PryableDoor.AllowInteracting))]
    public class PryableDoorPatch
    {
        internal static bool Prefix(PryableDoor __instance, ReferenceHub ply, byte colliderId, ref bool __result)
            => DoorChecker.Check(ply, ref __result);
    }

    /// <summary>
    /// Patch for airlock door.
    /// </summary>
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
#pragma warning restore SA1313
#pragma warning restore SA1402
#pragma warning restore SA1649
}
