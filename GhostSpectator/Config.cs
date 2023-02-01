using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostSpectator
{
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
    }
}
