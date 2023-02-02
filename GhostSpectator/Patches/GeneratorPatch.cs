namespace GhostSpectator.Patches
{
    using HarmonyLib;
    using MapGeneration.Distributors;

    /// <summary>
    /// Patches generators so that ghosts cannot interact with them.
    /// </summary>
    [HarmonyPatch(typeof(Scp079Generator), nameof(Scp079Generator.ServerInteract))]
    public class GeneratorPatch
    {
        public static bool Prefix(Scp079Generator __instance, ReferenceHub ply, byte colliderId)
        {
            return !API.IsGhost(ply);
        }
    }
}
