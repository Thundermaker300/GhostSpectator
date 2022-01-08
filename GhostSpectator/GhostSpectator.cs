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
        public static readonly List<Player> Ghosts = new List<Player>();
        public static readonly Dictionary<Player, Vector3> SpawnPositions = new Dictionary<Player, Vector3>();
        private EventHandler _handler;
        private Harmony _harmonyPatch;

        public override void OnEnabled()
        {
            // Create Classes
            Singleton = this;
            _handler = new EventHandler(this);

            // Create Events
            // Server
            Events.Server.RespawningTeam += _handler.OnRespawningTeam;
            // Player
            Events.Player.Verified += _handler.OnVerified;
            Events.Player.Destroying += _handler.OnDestroying;
            Events.Player.Dying += _handler.OnDying;
            Events.Player.Died += _handler.OnDied;
            Events.Player.Hurting += _handler.OnHurting;
            Events.Player.ChangingRole += _handler.OnChangingRole;
            Events.Player.DroppingItem += _handler.OnDroppingItem;
            Events.Player.PickingUpItem += _handler.OnPickingUpItem;
            Events.Player.PickingUpAmmo += _handler.OnPickingUpAmmo;
            Events.Player.PickingUpArmor += _handler.OnPickingUpArmor;
            Events.Player.InteractingLocker += _handler.OnInteractingLocker;
            Events.Player.InteractingElevator += _handler.OnInteractingElevator;
            Events.Player.TriggeringTesla += _handler.OnTriggeringTesla;
            Events.Player.OpeningGenerator += _handler.OnOpeningGenerator;
            Events.Player.ClosingGenerator += _handler.OnClosingGenerator;
            Events.Player.IntercomSpeaking += _handler.OnIntercomSpeaking;
            Events.Player.EnteringFemurBreaker += _handler.OnFemurEnter;
            Events.Player.SpawningRagdoll += _handler.OnSpawningRagdoll;
            Events.Player.FailingEscapePocketDimension += _handler.OnFailingEscapePocketDimension;
            Events.Player.Handcuffing += _handler.OnHandcuffing;
            Events.Player.RemovingHandcuffs += _handler.OnRemovingHandcuffs;
            Events.Scp914.UpgradingInventoryItem += _handler.OnUpgradingInventoryItem;
            // SCP-049 FIX
            Events.Scp049.FinishingRecall += _handler.OnFinishingRecall;
            // SCP-914
            Events.Scp914.Activating += _handler.OnActivating;
            Events.Scp914.ChangingKnobSetting += _handler.OnChangingKnobStatus;
            // Workstation
            Events.Player.ActivatingWorkstation += _handler.OnActivatingWorkstation;
            // Warhead
            Events.Warhead.Starting += _handler.OnStarting;
            Events.Warhead.Stopping += _handler.OnStopping;
            Events.Warhead.ChangingLeverStatus += _handler.OnChangingLeverStatus;
            Events.Warhead.Detonated += _handler.OnDetonated;
            // SCP-096
            Events.Scp096.AddingTarget += _handler.OnAddingTarget;
            // SCP-106
            Events.Scp106.Containing += _handler.On106Containing;
            // Other
            Events.Server.EndingRound += _handler.OnEndingRound;

            // Patching
            try
            {
                _harmonyPatch = new Harmony(nameof(GhostSpectator).ToLowerInvariant());
                _harmonyPatch.PatchAll();

                Log.Info("Harmony patching complete.");
            }
            catch (Exception e)
            {
                Log.Error($"Harmony patching failed! {e}");
            }

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            // Create Events
            // Server
            Events.Server.RespawningTeam -= _handler.OnRespawningTeam;
            // Player
            Events.Player.Verified -= _handler.OnVerified;
            Events.Player.Destroying -= _handler.OnDestroying;
            Events.Player.Dying -= _handler.OnDying;
            Events.Player.Died -= _handler.OnDied;
            Events.Player.Hurting -= _handler.OnHurting;
            Events.Player.ChangingRole -= _handler.OnChangingRole;
            Events.Player.DroppingItem -= _handler.OnDroppingItem;
            Events.Player.PickingUpItem -= _handler.OnPickingUpItem;
            Events.Player.PickingUpAmmo -= _handler.OnPickingUpAmmo;
            Events.Player.PickingUpArmor -= _handler.OnPickingUpArmor;
            Events.Player.InteractingLocker -= _handler.OnInteractingLocker;
            Events.Player.InteractingElevator -= _handler.OnInteractingElevator;
            Events.Player.TriggeringTesla -= _handler.OnTriggeringTesla;
            Events.Player.OpeningGenerator -= _handler.OnOpeningGenerator;
            Events.Player.ClosingGenerator -= _handler.OnClosingGenerator;
            Events.Player.IntercomSpeaking -= _handler.OnIntercomSpeaking;
            Events.Player.EnteringFemurBreaker -= _handler.OnFemurEnter;
            Events.Player.SpawningRagdoll -= _handler.OnSpawningRagdoll;
            Events.Player.FailingEscapePocketDimension -= _handler.OnFailingEscapePocketDimension;
            Events.Player.Handcuffing -= _handler.OnHandcuffing;
            Events.Player.RemovingHandcuffs -= _handler.OnRemovingHandcuffs;
            Events.Scp914.UpgradingInventoryItem -= _handler.OnUpgradingInventoryItem;
            // SCP-049 FIX
            Events.Scp049.FinishingRecall -= _handler.OnFinishingRecall;
            // SCP-914
            Events.Scp914.Activating -= _handler.OnActivating;
            Events.Scp914.ChangingKnobSetting -= _handler.OnChangingKnobStatus;
            // Workstation
            Events.Player.ActivatingWorkstation -= _handler.OnActivatingWorkstation;
            // Warhead
            Events.Warhead.Starting -= _handler.OnStarting;
            Events.Warhead.Stopping -= _handler.OnStopping;
            Events.Warhead.ChangingLeverStatus -= _handler.OnChangingLeverStatus;
            Events.Warhead.Detonated -= _handler.OnDetonated;
            // SCP-096
            Events.Scp096.AddingTarget -= _handler.OnAddingTarget;
            // SCP-106
            Events.Scp106.Containing -= _handler.On106Containing;
            // Other
            Events.Server.EndingRound -= _handler.OnEndingRound;

            // Unpatch
            _harmonyPatch.UnpatchAll(_harmonyPatch.Id);

            // Destroy Classes
            Singleton = null;
            _handler = null;

            base.OnDisabled();
        }

        public override string Name => "GhostSpectator";
        public override string Author => "Thunder";
        public override Version Version => new Version(1, 2, 1);
        public override Version RequiredExiledVersion => new Version(4, 2, 2);
        public override PluginPriority Priority => PluginPriority.High;
    }
}