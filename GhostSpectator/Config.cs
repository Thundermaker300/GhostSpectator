﻿using System.Collections.Generic;
using System.ComponentModel;

using Exiled.API.Interfaces;

namespace GhostSpectator
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool DebugMessagesEnabled { get; set; } = false;

        [Description(
            "Determines the role to use for ghosts. Chaos Insurgency is used by default so that ghosts are not targets to SCPs. If you set to an SCP, they can still attack!!")]
        public RoleType GhostRole { get; set; } = RoleType.ChaosRifleman;

        [Description("Message to show to ghosts upon spawning. Set to none to disable.")]
        public string SpawnMessage { get; set; } =
            "<size=25><color=#5EFF29>You are a ghost!\n- You can <b>noclip</b> by pressing I.\n- You <b>cannot</b> interact with doors, elevators, generators, etc.\n- Drop your <b>Weapon Manager Tablet</b> to teleport to a random door.\n- Drop your <b>Coin</b> to teleport to a random player.\n- You cannot enrage 096 or freeze 173.</color></size>";

        [Description("Sets the length of time in seconds the spawn message is visible.")]
        public int SpawnMessageLength { get; set; } = 10;

        [Description(
            "If set to true, ghosts can swap to spectator mode via the .spec command in the server console, and vice versa.")]
        public bool GhostSpecSwap { get; set; } = false;
        [Description("If set to true, the gs.spectate permission will be required to use the .spec command. If this is true, ghost_spec_swap should also be true.")]
        public bool GhostSpecPermission { get; set; } = false;
        [Description("If set to true, players will become spectators when they die, rather than ghosts. If this is true, ghost_spec_swap should also be true so that players can choose to become ghosts.")]
        public bool StartAsSpectator { get; set; } = false;

        [Description(
            "If set to true, ghosts will be given a weapon manager tablet that they can drop to teleport to a random door in the map.")]
        public bool GiveGhostNavigator { get; set; } = true;

        [Description(
            "If set to true, ghosts can navigate to doors in light containment after decontamination has begun.")]
        public bool NavigateLczAfterDecon { get; set; } = false;

        [Description(
            "If set to true, ghosts will lose their items after the nuke has detonated (they will also be teleported to the surface).")]
        public bool RemoveItemsAfterNuke { get; set; } = true;

        [Description("Set the message to show when a ghost navigates. Set to none to disable navigate messages.")]
        public string NavigateMessage { get; set; } = "You've navigated to <b>{name}</b>.";

        [Description("The message to show if the teleport fails.")]
        public string NavigateFailMessage { get; set; } = "Unable to teleport, please try again.";

        [Description(
            "If set to true, ghosts will be given a coin that they can drop to teleport to a random alive player.")]
        public bool CanGhostsTeleport { get; set; } = true;

        [Description("A list of roles that CANNOT be teleported to (eg Scp173, NtfCadet, etc).")]
        public List<RoleType> TeleportBlacklist { get; set; } = new List<RoleType> {RoleType.Tutorial};

        [Description("Set the message to show when a ghost teleports. Set to none to disable teleport messages.")]
        public string TeleportMessage { get; set; } = "You teleported to <b>{name}</b> who is a <b>{class}</b>.";

        [Description(
            "Set the message to show if a ghost tries to teleport and nobody is alive that can be teleported to.")]
        public string TeleportNoneMessage { get; set; } = "There is nobody alive to teleport to.";

        [Description("Determines if ghosts can freeze SCP-173 and trigger SCP-096.")]
        public bool TriggerScps { get; set; } = false;

        [Description("Set each of the following to true to determine whether or not ghosts can interact with them.")]
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
        public bool InteractWorkstation { get; set; } = false;
        public bool Contain106 { get; set; } = false;
        public bool EnterFemurBreaker { get; set; } = false;
        public bool Disarm { get; set; } = false;
        public bool FreeDisarmed { get; set; } = false;

        [Description(
            "Sets the string for roles in place of {class} in the above strings (for example, replacing Class-D Personnel with DBOI will make it say DBOI in game).")]
        public Dictionary<RoleType, string> RoleStrings { get; set; } = new Dictionary<RoleType, string>
        {
            [RoleType.ClassD] = "Class-D Personnel",
            [RoleType.Scientist] = "Scientist",
            [RoleType.FacilityGuard] = "Facility Guard",
            [RoleType.NtfCaptain] = "NTF Captain",
            [RoleType.NtfPrivate] = "NTF Private",
            [RoleType.NtfSergeant] = "NTF Sergeant",
            [RoleType.NtfSpecialist] = "NTF Specialist",
            [RoleType.ChaosRifleman] = "Chaos Rifleman",
            [RoleType.ChaosRepressor] = "Chaos Repressor",
            [RoleType.ChaosMarauder] = "Chaos Marauder",
            [RoleType.ChaosConscript] = "Chaos Conscript",
            [RoleType.Scp049] = "SCP-049",
            [RoleType.Scp0492] = "SCP-049-2",
            [RoleType.Scp079] = "SCP-079",
            [RoleType.Scp096] = "SCP-096",
            [RoleType.Scp106] = "SCP-106",
            [RoleType.Scp173] = "SCP-173",
            [RoleType.Scp93953] = "SCP-939-53",
            [RoleType.Scp93989] = "SCP-939-89",
            [RoleType.Tutorial] = "Tutorial",
        };
    }
}