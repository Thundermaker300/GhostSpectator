using System;
using System.Collections.Generic;

using Exiled.API.Enums;
using Exiled.API.Features;
using UnityEngine;
using HarmonyLib;

using PlayerHandler = Exiled.Events.Handlers.Player;

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
            // Create Classes
            Singleton = this;
            Configs = Config;
            Translations = Translation;
            Handler = new EventHandler();

            PlayerHandler.ChangingRole += Handler.OnChangingRole;
            PlayerHandler.Spawned += Handler.OnSpawned;
            PlayerHandler.ChangingItem += Handler.OnChangingItem;
            PlayerHandler.DroppingItem += Handler.OnDroppingItem;
            PlayerHandler.FlippingCoin += Handler.OnFlippingCoin;

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

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            PlayerHandler.ChangingRole -= Handler.OnChangingRole;
            PlayerHandler.Spawned -= Handler.OnSpawned;
            PlayerHandler.ChangingItem -= Handler.OnChangingItem;
            PlayerHandler.DroppingItem -= Handler.OnDroppingItem;
            PlayerHandler.FlippingCoin -= Handler.OnFlippingCoin;

            // Unpatch
            Harmony.UnpatchAll(Harmony.Id);

            // Destroy Classes
            Singleton = null;
            Handler = null;

            base.OnDisabled();
        }

        public override string Name => "GhostSpectator";
        public override string Author => "Thunder";
        public override Version Version => new Version(2, 0, 0);
        public override Version RequiredExiledVersion => new Version(6, 0, 0);
        public override PluginPriority Priority => PluginPriority.High;
    }
}