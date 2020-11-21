using System;
using HarmonyLib;
using Exiled.API.Features;

namespace GhostSpectator.Patches
{
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdUseGenerator))]
    class InteractingGeneratorPatch
    {
        [HarmonyPriority(Priority.High)]
        public static bool Prefix(PlayerInteract __instance, PlayerInteract.Generator079Operations command, UnityEngine.GameObject go)
        {
            try
            {
                Player Ply = Player.Get(__instance._hub);
                if (Ply == null) return true;

                if (API.IsGhost(Ply) && !GhostSpectator.Singleton.Config.InteractGenerators)
                {
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return true;
            }
        }
    }
}
