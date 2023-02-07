namespace GhostSpectator
{
    using System;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using HarmonyLib;
    using PlayerHandler = Exiled.Events.Handlers.Player;
    using Scp049Handler = Exiled.Events.Handlers.Scp049;
    using Scp914Handler = Exiled.Events.Handlers.Scp914;
    using ServerHandler = Exiled.Events.Handlers.Server;
    using WarheadHandler = Exiled.Events.Handlers.Warhead;

    /// <inheritdoc/>
    public class GhostSpectator : Plugin<Config, Translation>
    {
        private static Harmony harmony;
        private static EventHandler handler;

        /// <summary>
        /// Gets the singleton of the plugin.
        /// </summary>
        public static GhostSpectator Singleton { get; private set; }

        /// <summary>
        /// Gets the plugin's configs.
        /// </summary>
        public static Config Configs => Singleton?.Config;

        /// <summary>
        /// Gets the plugin's translations.
        /// </summary>
        public static Translation Translations => Singleton.Translation;

        /// <summary>
        /// Gets the plugin's <see cref="EventHandler"/>.
        /// </summary>
        public static EventHandler Handler => handler;

        /// <summary>
        /// Gets the plugin's <see cref="HarmonyLib.Harmony"/> instance.
        /// </summary>
        public static Harmony Harmony => harmony;

        /// <inheritdoc/>
        public override string Name => "GhostSpectator";

        /// <inheritdoc/>
        public override string Author => "Thunder";

        /// <inheritdoc/>
        public override Version Version => new Version(2, 0, 0);

        /// <inheritdoc/>
        public override Version RequiredExiledVersion => new Version(6, 0, 0);

        /// <inheritdoc/>
        public override PluginPriority Priority => PluginPriority.High;

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            base.OnEnabled();

            // Create Classes
            Singleton = this;
            handler = new EventHandler();

            // Important Events
            PlayerHandler.ChangingRole += Handler.OnChangingRole;
            PlayerHandler.Dying += Handler.OnDying;
            PlayerHandler.Died += Handler.OnDied;
            PlayerHandler.Spawned += Handler.OnSpawned;
            PlayerHandler.ChangingItem += Handler.OnChangingItem;
            PlayerHandler.FlippingCoin += Handler.OnFlippingCoin;
            PlayerHandler.SpawningRagdoll += Handler.OnSpawningRagdoll;
            PlayerHandler.Hurting += Handler.OnHurting;

            ServerHandler.RestartingRound += Handler.OnRestartingRound;
            ServerHandler.RespawningTeam += Handler.OnRespawningTeam;
            WarheadHandler.Detonated += Handler.OnDetonated;

            // Interaction Disabling
            PlayerHandler.ActivatingGenerator += Handler.GenericGhostDisallow;
            PlayerHandler.ActivatingWarheadPanel += Handler.GenericGhostDisallow;
            PlayerHandler.ActivatingWorkstation += Handler.GenericGhostDisallow;
            PlayerHandler.ClosingGenerator += Handler.GenericGhostDisallow;
            PlayerHandler.DeactivatingWorkstation += Handler.GenericGhostDisallow;
            PlayerHandler.DroppingAmmo += Handler.GenericGhostDisallow;
            PlayerHandler.DroppingItem += Handler.GenericGhostDisallow;
            PlayerHandler.EnteringEnvironmentalHazard += Handler.GenericGhostDisallow;
            PlayerHandler.Escaping += Handler.GenericGhostDisallow;
            PlayerHandler.Handcuffing += Handler.GenericGhostDisallow;
            PlayerHandler.InteractingDoor += Handler.GenericGhostDisallow;
            PlayerHandler.InteractingElevator += Handler.GenericGhostDisallow;
            PlayerHandler.InteractingLocker += Handler.GenericGhostDisallow;
            PlayerHandler.InteractingShootingTarget += Handler.GenericGhostDisallow;
            PlayerHandler.IntercomSpeaking += Handler.GenericGhostDisallow;
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
                harmony = new Harmony(nameof(GhostSpectator).ToLowerInvariant() + "-" + DateTime.UtcNow.Ticks);
                harmony.PatchAll();

                Log.Info("Harmony patching complete.");
            }
            catch (Exception e)
            {
                Log.Error($"Harmony patching failed! {e}");
            }
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            // Important Events
            PlayerHandler.ChangingRole -= Handler.OnChangingRole;
            PlayerHandler.Dying -= Handler.OnDying;
            PlayerHandler.Died -= Handler.OnDied;
            PlayerHandler.Spawned -= Handler.OnSpawned;
            PlayerHandler.ChangingItem -= Handler.OnChangingItem;
            PlayerHandler.FlippingCoin -= Handler.OnFlippingCoin;
            PlayerHandler.SpawningRagdoll -= Handler.OnSpawningRagdoll;
            PlayerHandler.Hurting -= Handler.OnHurting;

            ServerHandler.RestartingRound -= Handler.OnRestartingRound;
            ServerHandler.RespawningTeam -= Handler.OnRespawningTeam;
            WarheadHandler.Detonated -= Handler.OnDetonated;

            // Interaction Disabling
            PlayerHandler.ActivatingGenerator -= Handler.GenericGhostDisallow;
            PlayerHandler.ActivatingWarheadPanel -= Handler.GenericGhostDisallow;
            PlayerHandler.ActivatingWorkstation -= Handler.GenericGhostDisallow;
            PlayerHandler.ClosingGenerator -= Handler.GenericGhostDisallow;
            PlayerHandler.DeactivatingWorkstation -= Handler.GenericGhostDisallow;
            PlayerHandler.DroppingAmmo -= Handler.GenericGhostDisallow;
            PlayerHandler.DroppingItem -= Handler.GenericGhostDisallow;
            PlayerHandler.EnteringEnvironmentalHazard -= Handler.GenericGhostDisallow;
            PlayerHandler.Escaping -= Handler.GenericGhostDisallow;
            PlayerHandler.Handcuffing -= Handler.GenericGhostDisallow;
            PlayerHandler.InteractingDoor -= Handler.GenericGhostDisallow;
            PlayerHandler.InteractingElevator -= Handler.GenericGhostDisallow;
            PlayerHandler.InteractingLocker -= Handler.GenericGhostDisallow;
            PlayerHandler.InteractingShootingTarget -= Handler.GenericGhostDisallow;
            PlayerHandler.IntercomSpeaking -= Handler.GenericGhostDisallow;
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
            handler = null;
            harmony = null;

            base.OnDisabled();
        }
    }
}