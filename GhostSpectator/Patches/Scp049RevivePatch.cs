namespace GhostSpectator.Patches
{
    using Exiled.API.Features;
    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp049;
    using static PlayerRoles.PlayableScps.Scp049.Scp049ResurrectAbility;

    /// <summary>
    /// Patches SCP-049 resurrection ability so that ghosts can be revived.
    /// </summary>
    [HarmonyPatch(typeof(Scp049ResurrectAbility), nameof(Scp049ResurrectAbility.ServerValidateAny))]
    public class Scp049RevivePatch
    {
        // Todo: Look into zombies being revived >4 times
        public static bool Prefix(Scp049ResurrectAbility __instance, ref bool __result)
        {
            if (__instance.CurRagdoll == null)
            {
                return false;
            }

            ReferenceHub ownerHub = __instance.CurRagdoll.Info.OwnerHub;
            var ply = Player.Get(ownerHub);

            if (ply is null)
            {
                return false;
            }

            if (API.IsGhost(ply) && __instance.CheckMaxResurrections(ownerHub) == ResurrectError.None)
            {
                __result = true;
                return false;
            }

            return true;
        }
    }
}
