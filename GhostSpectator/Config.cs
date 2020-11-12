﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Exiled.API.Interfaces;

namespace GhostSpectator
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        [Description("Message to show to ghosts upon spawning. Set to none to disable.")]
        public string SpawnMessage { get; set; } = "<size=25><color=#5EFF29>You are a ghost!\n- You can <b>noclip</b> by pressing I.\n- Drop your <b>O5 Keycard</b> to teleport to a random door.\n- Drop your <b>Coin</b> to teleport to a random player.\n- You cannot enrage 096 or freeze 173.</color></size>";
        [Description("Sets the length of time in seconds the spawn message is visible.")]
        public int SpawnMessageLength { get; set; } = 10;
        [Description("If set to true, ghosts will be given an O5 card that they can drop to teleport to gates/checkpoints (they can't actually drop it for alive players to pick up).")]
        public bool GiveGhostNavigator { get; set; } = true;
        [Description("Set the message to show when a ghost navigates. Set to none to disable navigate messages.")]
        public string NavigateMessage { get; set; } = "You've navigated to <b>{name}</b>.";
        [Description("The message to show if the teleport fails.")]
        public string NavigateFailMessage { get; set; } = "Unable to teleport, please try again.";
        [Description("If set to true, ghosts will be given a coin that they can drop to teleport to a random alive player.")]
        public bool CanGhostsTeleport { get; set; } = true;
        [Description("Set the message to show when a ghost teleports. Set to none to disable teleport messages.")]
        public string TeleportMessage { get; set; } = "You teleported to <b>{name}</b> who is a <b>{class}</b>.";
        [Description("Determines if ghosts can freeze SCP-173, trigger SCP-096, and show up on SCP-939's vision.")]
        public string TeleportNoneMessage { get; set; } = "There is nobody alive to teleport to.";
        [Description("Set each of the following to true to determine whether or not ghosts can interact with them.")]
        public bool TriggerScps { get; set; } = false;
        [Description("Set the message to show if a ghost tries to teleport and nobody is alive that can be teleported to.")]
        public bool PickupItems { get; set; } = false;
        public bool DropItems { get; set; } = false;
        public bool InteractDoors { get; set; } = false;
        public bool InteractElevators { get; set; } = false;
        public bool InteractGenerators { get; set; } = false;
        public bool InteractLockers { get; set; } = false;
        public bool TriggerTeslas { get; set; } = false;
        public bool StartWarhead { get; set; } = false;
        public bool StopWarhead { get; set; } = false;
        public bool ToggleWarhead { get; set; } = false;
        public bool InteractScp914 { get; set; } = false;
        public bool InteractIntercom { get; set; } = false;
    }
}
