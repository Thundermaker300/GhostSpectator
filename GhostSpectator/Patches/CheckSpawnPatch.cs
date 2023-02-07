namespace GhostSpectator.Patches
{
#pragma warning disable SA1313
    using HarmonyLib;
    using Respawning;

    /// <summary>
    /// Allows respawn wave to start when all spectators are ghosts.
    /// </summary>
    [HarmonyPatch(typeof(RespawnManager), nameof(RespawnManager.CheckSpawnable))]
    public class CheckSpawnPatch
    {
        public static bool Prefix(RespawnManager __instance, ReferenceHub ply, ref bool __result)
        {
            if (API.IsGhost(ply))
            {
                __result = true;
                return false;
            }

            return true;
        }
    }
#pragma warning restore SA1313
}
