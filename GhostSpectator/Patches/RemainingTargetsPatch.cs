using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;
using Exiled.API.Features;

namespace GhostSpectator.Patches
{
    [HarmonyPatch(typeof(ScpInterfaces), nameof(ScpInterfaces.remTargs), MethodType.Setter)]
    class RemainingTargetsPatch
    {
        public static bool Prefix(ScpInterfaces __instance, ref int value)
        {
            int count = 0;
            foreach (Player Ply in Player.List)
            {
                if (Ply.IsAlive && !API.IsGhost(Ply) && Ply.Team != Team.SCP && Ply.Team != Team.CHI)
                {
                    count++;
                }
            }
            ScpInterfaces.remTargs = count;
            return false;
        }
    }
}
