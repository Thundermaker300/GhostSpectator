using System;
using System.Collections.Generic;

using Exiled.API.Enums;
using Exiled.API.Features;
using UnityEngine;
using HarmonyLib;

using PlayerHandler = Exiled.Events.Handlers.Player;
using WarheadHandler = Exiled.Events.Handlers.Warhead;
using Scp049Handler = Exiled.Events.Handlers.Scp049;
using Scp914Handler = Exiled.Events.Handlers.Scp914;
using ServerHandler = Exiled.Events.Handlers.Server;
using System.Reflection;

namespace GhostSpectator
{
    public class GhostSpectator : Plugin<Config, Translation>
    {
        public static GhostSpectator Singleton { get; private set; }
        public static Config Configs { get; private set; }
        public static Translation Translations { get; private set; }
        public static EventHandler Handler { get; private set; }
        public static Harmony Harmony { get; private set; }

        public override void OnEnabled()
        {
            base.OnEnabled();

            // Create Classes
            Singleton = this;
            Configs = Config;
            Translations = Translation;
            Handler = new EventHandler();

            // Important Events
            PlayerHandler.ChangingRole += Handler.OnChangingRole;
            PlayerHandler.Spawned += Handler.OnSpawned;
            PlayerHandler.ChangingItem += Handler.OnChangingItem;
            PlayerHandler.FlippingCoin += Handler.OnFlippingCoin;

            ServerHandler.RespawningTeam += Handler.OnRespawningTeam;

            // Interaction Disabling
            PlayerHandler.ActivatingGenerator += Handler.GenericGhostDisallow;
            PlayerHandler.ActivatingWarheadPanel += Handler.GenericGhostDisallow;
            PlayerHandler.ActivatingWorkstation += Handler.GenericGhostDisallow;
            PlayerHandler.ClosingGenerator += Handler.GenericGhostDisallow;
            PlayerHandler.DeactivatingWorkstation += Handler.GenericGhostDisallow;
            PlayerHandler.DroppingAmmo += Handler.GenericGhostDisallow;
            PlayerHandler.DroppingItem += Handler.GenericGhostDisallow;
            PlayerHandler.DroppingNothing += Handler.GenericGhostDisallow;
            PlayerHandler.EnteringEnvironmentalHazard += Handler.GenericGhostDisallow;
            PlayerHandler.Escaping += Handler.GenericGhostDisallow;
            PlayerHandler.Handcuffing += Handler.GenericGhostDisallow;
            PlayerHandler.InteractingDoor += Handler.GenericGhostDisallow;
            PlayerHandler.InteractingElevator += Handler.GenericGhostDisallow;
            PlayerHandler.InteractingLocker += Handler.GenericGhostDisallow;
            PlayerHandler.InteractingShootingTarget += Handler.GenericGhostDisallow;
            PlayerHandler.IntercomSpeaking += Handler.GenericGhostDisallow;
            //PlayerHandler.MakingNoise += Handler.GenericGhostDisallow;
            PlayerHandler.OpeningGenerator += Handler.GenericGhostDisallow;
            PlayerHandler.PlayerDamageWindow += Handler.GenericGhostDisallow;
            PlayerHandler.PickingUpItem += Handler.GenericGhostDisallow;
            PlayerHandler.ReloadingWeapon += Handler.GenericGhostDisallow;
            PlayerHandler.RemovingHandcuffs += Handler.GenericGhostDisallow;
            PlayerHandler.SearchingPickup += Handler.GenericGhostDisallow;
            PlayerHandler.Shooting += Handler.GenericGhostDisallow;
            PlayerHandler.TriggeringTesla += Handler.GenericGhostDisallow;
            PlayerHandler.UnlockingGenerator += Handler.GenericGhostDisallow;

            WarheadHandler.ChangingLeverStatus += Handler.GenericGhostDisallow;
            WarheadHandler.Starting += Handler.GenericGhostDisallow;
            WarheadHandler.Stopping += Handler.GenericGhostDisallow;

            Scp049Handler.FinishingRecall += Handler.OnFinishingRecall;

            Scp914Handler.Activating += Handler.GenericGhostDisallow;
            Scp914Handler.ChangingKnobSetting += Handler.GenericGhostDisallow;

            Exiled.Events.Handlers.Scp096.AddingTarget += Handler.OnAddingTarget;

            // Patching
            try
            {
                Harmony = new Harmony(nameof(GhostSpectator).ToLowerInvariant() + "-" + DateTime.UtcNow.Ticks);
                Harmony.PatchAll();

                Log.Info("Harmony patching complete.");
            }
            catch (Exception e)
            {
                Log.Error($"Harmony patching failed! {e}");
            }
        }

        public override void OnDisabled()
        {
            // Important Events
            PlayerHandler.ChangingRole -= Handler.OnChangingRole;
            PlayerHandler.Spawned -= Handler.OnSpawned;
            PlayerHandler.ChangingItem -= Handler.OnChangingItem;
            PlayerHandler.FlippingCoin -= Handler.OnFlippingCoin;

            ServerHandler.RespawningTeam -= Handler.OnRespawningTeam;

            // Interaction Disabling
            PlayerHandler.ActivatingGenerator -= Handler.GenericGhostDisallow;
            PlayerHandler.ActivatingWarheadPanel -= Handler.GenericGhostDisallow;
            PlayerHandler.ActivatingWorkstation -= Handler.GenericGhostDisallow;
            PlayerHandler.ClosingGenerator -= Handler.GenericGhostDisallow;
            PlayerHandler.DeactivatingWorkstation -= Handler.GenericGhostDisallow;
            PlayerHandler.DroppingAmmo -= Handler.GenericGhostDisallow;
            PlayerHandler.DroppingItem -= Handler.GenericGhostDisallow;
            PlayerHandler.DroppingNothing -= Handler.GenericGhostDisallow;
            PlayerHandler.EnteringEnvironmentalHazard -= Handler.GenericGhostDisallow;
            PlayerHandler.Escaping -= Handler.GenericGhostDisallow;
            PlayerHandler.Handcuffing -= Handler.GenericGhostDisallow;
            PlayerHandler.InteractingDoor -= Handler.GenericGhostDisallow;
            PlayerHandler.InteractingElevator -= Handler.GenericGhostDisallow;
            PlayerHandler.InteractingLocker -= Handler.GenericGhostDisallow;
            PlayerHandler.InteractingShootingTarget -= Handler.GenericGhostDisallow;
            PlayerHandler.IntercomSpeaking -= Handler.GenericGhostDisallow;
            //PlayerHandler.MakingNoise -= Handler.GenericGhostDisallow;
            PlayerHandler.OpeningGenerator -= Handler.GenericGhostDisallow;
            PlayerHandler.PlayerDamageWindow -= Handler.GenericGhostDisallow;
            PlayerHandler.PickingUpItem -= Handler.GenericGhostDisallow;
            PlayerHandler.ReloadingWeapon -= Handler.GenericGhostDisallow;
            PlayerHandler.RemovingHandcuffs -= Handler.GenericGhostDisallow;
            PlayerHandler.SearchingPickup -= Handler.GenericGhostDisallow;
            PlayerHandler.Shooting -= Handler.GenericGhostDisallow;
            PlayerHandler.TriggeringTesla -= Handler.GenericGhostDisallow;
            PlayerHandler.UnlockingGenerator -= Handler.GenericGhostDisallow;

            WarheadHandler.ChangingLeverStatus -= Handler.GenericGhostDisallow;
            WarheadHandler.Starting -= Handler.GenericGhostDisallow;
            WarheadHandler.Stopping -= Handler.GenericGhostDisallow;

            Scp049Handler.FinishingRecall -= Handler.OnFinishingRecall;

            Scp914Handler.Activating -= Handler.GenericGhostDisallow;
            Scp914Handler.ChangingKnobSetting -= Handler.GenericGhostDisallow;

            Exiled.Events.Handlers.Scp096.AddingTarget -= Handler.OnAddingTarget;

            // Unpatch
            Harmony.UnpatchAll(Harmony.Id);

            // Destroy Classes
            Singleton = null;
            Handler = null;
            Harmony = null;

            base.OnDisabled();
        }

        public override string Name => "GhostSpectator";
        public override string Author => "Thunder";
        public override Version Version => new Version(2, 0, 0);
        public override Version RequiredExiledVersion => new Version(6, 0, 0);
        public override PluginPriority Priority => PluginPriority.High;
    }
}