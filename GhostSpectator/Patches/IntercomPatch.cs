namespace GhostSpectator.Patches
{
    using HarmonyLib;
    using PlayerRoles.Voice;

    /// <summary>
    /// Patches the intercom so that ghosts cannot interact with it.
    /// </summary>
    [HarmonyPatch(typeof(Intercom), nameof(Intercom.CheckRange))]
    public class IntercomPatch
    {
        public static bool Prefix(Intercom __instance, ReferenceHub hub)
        {
            if (hub is null)
                return false;

            return !API.IsGhost(hub);
        }
    }
}
