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
            Singleton = new GhostSpectator();
            Handler = new EventHandler(this);

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
            Events.Player.EnteringFemurBreaker += Handler.OnFemurEnter;

            Events.Player.SpawningRagdoll += Handler.OnSpawningRagdoll;
            /// SCP-049 FIX
            Events.Scp049.FinishingRecall += Handler.OnFinishingRecall;
            /// SCP-914
            Events.Scp914.Activating += Handler.OnActivating;
            Events.Scp914.ChangingKnobSetting += Handler.OnChangingKnobStatus;
            /// Workstation
            Events.Player.ActivatingWorkstation += Handler.OnActivatingWorkstation;
            Events.Player.DeactivatingWorkstation += Handler.OnDeactivatingWorkstation;
            /// Warhead
            Events.Warhead.Starting += Handler.OnStarting;
            Events.Warhead.Stopping += Handler.OnStopping;
            Events.Warhead.ChangingLeverStatus += Handler.OnChangingLeverStatus;
            /// SCP-106
            Events.Scp106.Containing += Handler.On106Containing;
            /// Other (nothing here rn)

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
            Events.Player.EnteringFemurBreaker -= Handler.OnFemurEnter;

            Events.Player.SpawningRagdoll -= Handler.OnSpawningRagdoll;
            /// SCP-049 FIX
            Events.Scp049.FinishingRecall -= Handler.OnFinishingRecall;
            /// SCP-914
            Events.Scp914.Activating -= Handler.OnActivating;
            Events.Scp914.ChangingKnobSetting -= Handler.OnChangingKnobStatus;
            /// Workstation
            Events.Player.ActivatingWorkstation -= Handler.OnActivatingWorkstation;
            Events.Player.DeactivatingWorkstation -= Handler.OnDeactivatingWorkstation;
            /// Warhead
            Events.Warhead.Starting -= Handler.OnStarting;
            Events.Warhead.Stopping -= Handler.OnStopping;
            Events.Warhead.ChangingLeverStatus -= Handler.OnChangingLeverStatus;
            /// SCP-106
            Events.Scp106.Containing -= Handler.On106Containing;
            /// Other (nothing here rn)

            // Unpatch
            HarmonyPatch.UnpatchAll(PatchId);

            // Destroy Classes
            Singleton = null;
            Handler = null;
        }

        public override string Name => "GhostSpectator";
        public override string Author => "Thunder";
        public override Version Version => new Version(1, 1, 0);
        public override Version RequiredExiledVersion => new Version(2, 1, 18);
        public override PluginPriority Priority => PluginPriority.High;
    }
}
