using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Exiled.API.Features;

namespace GhostSpectator.Extensions
{
    public static class PlayerGhostExtensions
    {
        public static bool CanSee(this Player Ply, Player OtherPlayer)
        {
            if (API.IsGhost(OtherPlayer) && !API.IsGhost(Ply)) return false;
            return true;
        }
    }
}
