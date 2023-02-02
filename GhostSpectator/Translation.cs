namespace GhostSpectator
{
    using System.ComponentModel;
    using Exiled.API.Interfaces;

#pragma warning disable SA1600
    public class Translation : ITranslation
    {
        [Description("Coin descriptions")]
        public string HumanTeleportCoin { get; set; } = "Flip this coin to teleport to a random human!";

        public string ScpTeleportCoin { get; set; } = "Flip this coin to teleport to a random SCP!";

        public string GhostTeleportCoin { get; set; } = "Flip this coin to teleport to another ghost!";

        public string RoomTeleportCoin { get; set; } = "Flip this coin to teleport to a random room!";

        public string SurfaceTeleportCoin { get; set; } = "Flip this coin to teleport to the surface!";

        public string SetToSpectator { get; set; } = "Flip this coin to turn back into a spectator!";

        [Description("Teleport results")]
        public string PlayerTeleport { get; set; } = "You teleported to <b>{PLAYER}</b>, who is a <b><color={ROLECOLOR}>{ROLE}</color></b>.";

        public string NoPlayerToTeleport { get; set; } = "No players that could be teleported to were found!";

        public string RoomTeleport { get; set; } = "Room: <b>{ROOM}</b>";

        public string NukeDisable { get; set; } = "This coin cannot be used after the nuke has detonated.";

    }
#pragma warning restore SA1600
}
