using Exiled.API.Features;
using HarmonyLib;
using MapGeneration.Distributors;
using PlayerRoles.Voice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostSpectator.Patches
{
    [HarmonyPatch(typeof(Scp079Generator), nameof(Scp079Generator.ServerInteract))]
    public class GeneratorPatch
    {
        public static bool Prefix(Scp079Generator __instance, ReferenceHub ply, byte colliderId)
        {
            return !API.IsGhost(ply);
        }
    }
}
