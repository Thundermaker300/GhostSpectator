using Exiled.API.Features;
using HarmonyLib;
using PlayerRoles.PlayableScps.Scp049;

namespace GhostSpectator.Patches
{
    [HarmonyPatch(typeof(Scp049ResurrectAbility), nameof(Scp049ResurrectAbility.ServerValidateAny))]
    public class Scp049RevivePatch
    {
        // Todo: Fix for SCP-049-2 revives
        public static bool Prefix(Scp049ResurrectAbility __instance, ref bool __result)
        {
            if (__instance.CurRagdoll == null)
            {
                return false;
            }

            var ply = Player.Get(__instance.CurRagdoll.Info.OwnerHub);

            if (API.IsGhost(ply))
            {
                __result = true;
                return false;
            }

            return true;
        }
    }
}
