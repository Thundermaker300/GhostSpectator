﻿using Exiled.API.Features;
using HarmonyLib;
using Interactables.Interobjects.DoorUtils;

namespace GhostSpectator.Patches
{
    // Required in order for doors to not make denied sound.
    [HarmonyPatch(typeof(DoorVariant), nameof(DoorVariant.AllowInteracting))]
    public class DoorPatch
    {
        public static bool Prefix(DoorVariant __instance, ReferenceHub ply, byte colliderId, ref bool __result)
        {
            Player player = Player.Get(ply);
            if (player is not null && API.IsGhost(player))
            {
                __result = false;
                return false;
            }

            return true;
        }
    }
}
