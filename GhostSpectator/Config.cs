namespace GhostSpectator
{
    using System.ComponentModel;
    using Exiled.API.Interfaces;

    /// <summary>
    /// The mode at which the plugin operates.
    /// </summary>
    public enum Mode
    {
        /// <summary>
        /// Players will be ghosts by default (default setting).
        /// </summary>
        GhostByDefault,

        /// <summary>
        /// Players will be spectators by default.
        /// </summary>
        SpectatorByDefault,
    }

#pragma warning disable SA1600
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        public bool Debug { get; set; } = false;

        [Description("If set to true, ghosts will be given coins that each have their own abilities (such as teleporting to players, rooms, etc).")]
        public bool EnableCoins { get; set; } = true;

        [Description("If set to true, ghosts can teleport to LCZ after decontamination. No effect if coins are disabled.")]
        public bool LczTeleportAfterDecon { get; set; } = false;

        [Description("If set to true, ghosts' teleport room coin will be disabled when the nuke is detonated.")]
        public bool DisableRoomTeleportAfterNuke { get; set; } = true;

        [Description("If set to true, the 'gs.mode' permission will be required to swap between Ghost Spectating and regular spectating.")]
        public bool RequirePermission { get; set; } = false;

        [Description("Determines how the plugin will operate. Options: GhostByDefault, SpectatorByDefault")]
        public Mode SpectatorMode { get; set; } = Mode.GhostByDefault;

        public Exiled.API.Features.Broadcast GhostBroadcast { get; set; } = new("You are a ghost! You can noclip, cannot die, and have various abilities in your inventory!", 10, true);
    }
#pragma warning restore SA1600
}
