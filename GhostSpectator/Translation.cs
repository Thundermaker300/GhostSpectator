using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostSpectator
{
    public class Translation : ITranslation
    {
        [Description("Coin descriptions")]
        public string HumanTeleportCoin { get; set; } = "Flip this coin to teleport to a random human!";
        public string ScpTeleportCoin { get; set; } = "Flip this coin to teleport to a random SCP!";
        public string RoomTeleportCoin { get; set; } = "Flip this coin to teleport to a random room!";

        [Description("Teleport results")]
        public string PlayerTeleport { get; set; } = "You teleported to <b>{PLAYER}</b>, who is a <color={ROLECOLOR}>{ROLE}</color>.";
        public string NoPlayerToTeleport { get; set; } = "No players that could be teleported to were found!";
    }
}
