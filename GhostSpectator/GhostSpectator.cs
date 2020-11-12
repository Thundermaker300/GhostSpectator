using System;
using System.Collections.Generic;

using Exiled.API.Enums;
using Exiled.API.Features;
using UnityEngine;
using HarmonyLib;

using Events = Exiled.Events.Handlers;

namespace GhostSpectator
{
    public class GhostSpectator : Plugin<Config>
    {
        public static GhostSpectator Singleton;
        public static List<Player> Ghosts = new List<Player> { };
        public static Dictionary<Player, Vector3> SpawnPositions = new Dictionary<Player, Vector3> { };
        private EventHandler Handler;
        private Harmony HarmonyPatch;
        private string PatchId = nameof(GhostSpectator).ToLowerInvariant();
        public override void OnEnabled()
        {
            base.OnEnabled();

            // Create Classes
            Singleton = this;
            Handler = new EventHandler();

            // Create Events
            /// Server
            Events.Server.RespawningTeam += Handler.OnRespawningTeam;
            /// Player
            Events.Player.Dying += Handler.OnDying;
            Events.Player.Died += Handler.OnDied;
            Events.Player.ChangingRole += Handler.OnChangingRole;
            Events.Player.DroppingItem += Handler.OnDroppingItem;
            Events.Player.PickingUpItem += Handler.OnPickingUpItem;
            Events.Player.InteractingElevator += Handler.OnInteractingElevator;
            Events.Player.TriggeringTesla += Handler.OnTriggeringTesla;
            Events.Player.IntercomSpeaking += Handler.OnIntercomSpeaking;
            //// Generators
            Events.Player.OpeningGenerator += Handler.OnOpeningGenerator;
            Events.Player.ClosingGenerator += Handler.OnClosingGenerator;
            Events.Player.InsertingGeneratorTablet += Handler.OnInsertingGeneratorTablet;
            Events.Player.EjectingGeneratorTablet += Handler.OnEjectingGeneratorTablet;
            /// SCP-914
            Events.Scp914.Activating += Handler.OnActivating;
            Events.Scp914.ChangingKnobSetting += Handler.OnChangingKnobStatus;
            /// Warhead
            Events.Warhead.Starting += Handler.OnStarting;
            Events.Warhead.Stopping += Handler.OnStopping;
            Events.Warhead.ChangingLeverStatus += Handler.OnChangingLeverStatus;
            /// Other
            Events.Scp106.Containing += Handler.On106Containing;

            // Patching
            try
            {
                HarmonyPatch = new Harmony(PatchId);
                Harmony.DEBUG = true;
                HarmonyPatch.PatchAll();

                Log.Info("Harmony patching complete.");
            }
            catch (Exception e)
            {
                Log.Error($"Harmony patching failed! {e}");
            }
        }

        public override void OnDisabled()
        {
            base.OnDisabled();

            // Create Events
            /// Server
            Events.Server.RespawningTeam -= Handler.OnRespawningTeam;
            /// Player
            Events.Player.Dying -= Handler.OnDying;
            Events.Player.Died -= Handler.OnDied;
            Events.Player.ChangingRole -= Handler.OnChangingRole;
            Events.Player.DroppingItem -= Handler.OnDroppingItem;
            Events.Player.PickingUpItem -= Handler.OnPickingUpItem;
            Events.Player.InteractingElevator -= Handler.OnInteractingElevator;
            Events.Player.TriggeringTesla -= Handler.OnTriggeringTesla;
            Events.Player.IntercomSpeaking -= Handler.OnIntercomSpeaking;
            //// Generators
            Events.Player.OpeningGenerator -= Handler.OnOpeningGenerator;
            Events.Player.ClosingGenerator -= Handler.OnClosingGenerator;
            Events.Player.InsertingGeneratorTablet -= Handler.OnInsertingGeneratorTablet;
            Events.Player.EjectingGeneratorTablet -= Handler.OnEjectingGeneratorTablet;
            /// SCP-914
            Events.Scp914.Activating -= Handler.OnActivating;
            Events.Scp914.ChangingKnobSetting -= Handler.OnChangingKnobStatus;
            /// Warhead
            Events.Warhead.Starting -= Handler.OnStarting;
            Events.Warhead.Stopping -= Handler.OnStopping;
            Events.Warhead.ChangingLeverStatus -= Handler.OnChangingLeverStatus;

            // Unpatch
            HarmonyPatch.UnpatchAll(PatchId);

            // Destroy Classes
            Singleton = null;
            Handler = null;
        }

        public override string Name => "GhostSpectator";
        public override string Author => "Thunder";
        public override Version Version => new Version(1, 0, 0);
        public override PluginPriority Priority => PluginPriority.High;
    }
}
