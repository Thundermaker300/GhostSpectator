namespace GhostSpectator.Patches
{
    using HarmonyLib;
    using InventorySystem;

    /// <summary>
    /// Patches inventory so that ghosts cannot drop their coins on death.
    /// </summary>
    [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerDropEverything))]
    public class ServerDropEverythingPatch
    {
        public static bool Prefix(Inventory inv)
        {
            ReferenceHub owner = inv._hub;
            return !API.IsGhost(owner);
        }
    }
}
