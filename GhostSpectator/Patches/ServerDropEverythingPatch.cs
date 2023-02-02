using HarmonyLib;
using InventorySystem;

namespace GhostSpectator.Patches
{
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
