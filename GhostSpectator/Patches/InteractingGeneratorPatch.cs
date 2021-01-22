using System;
using HarmonyLib;
using Exiled.API.Features;

namespace GhostSpectator.Patches
{
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdUseGenerator))]
    internal static class InteractingGeneratorPatch
    {
        [HarmonyPriority(Priority.High)]
        private static bool Prefix(PlayerInteract __instance, PlayerInteract.Generator079Operations command,
            UnityEngine.GameObject go)
        {
            try
            {
                Player ply = Player.Get(__instance._hub);
                if (ply == null) return true;

                return !API.IsGhost(ply) || GhostSpectator.Singleton.Config.InteractGenerators;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return true;
            }
        }
    }
}