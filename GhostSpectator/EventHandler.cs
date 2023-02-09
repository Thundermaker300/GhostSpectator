namespace GhostSpectator
{
    using System;
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Interfaces;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Scp049;
    using Exiled.Events.EventArgs.Scp096;
    using Exiled.Events.EventArgs.Server;
    using Exiled.Permissions.Extensions;
    using MEC;
    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp049;
    using UnityEngine;

    /// <summary>
    /// Main event handler.
    /// </summary>
    public class EventHandler
    {
        /// <summary>
        /// Sets visibility settings.
        /// </summary>
        /// <param name="ghost">The ghost.</param>
        /// <param name="ply">The player.</param>
        public static void CheckPlayer(Player ghost, Player ply)
        {
            if (ghost.Role is not FpcRole fpcRole)
                return;

            if (!API.IsGhost(ghost))
            {
                fpcRole.IsInvisibleFor.Clear();
                return;
            }

            if (/*ply.SessionVariables.ContainsKey("IsGhost")*/ API.IsGhost(ply) || ply.CheckPermission("gs.see") || ply.Role.Type is RoleTypeId.Spectator or RoleTypeId.Overwatch)
            {
                if (fpcRole.IsInvisibleFor.Contains(ply))
                    fpcRole.IsInvisibleFor.Remove(ply);
            }
            else
            {
                if (!fpcRole.IsInvisibleFor.Contains(ply))
                    fpcRole.IsInvisibleFor.Add(ply);
            }
        }

        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (!ev.IsAllowed)
                return;

            // Remove coins before they drop!!!
            if (API.IsGhost(ev.Player) && ev.Reason != Exiled.API.Enums.SpawnReason.Destroyed)
            {
                ev.Player.ClearInventory();
            }

            // Refresh all visibility (because why not)
            foreach (var player in Player.List)
            {
                if (!API.IsGhost(player))
                    continue;

                foreach (var player2 in Player.List)
                {
                    if (player == player2)
                        continue;

                    CheckPlayer(player, player2);
                }
            }
        }

        public void OnDying(DyingEventArgs ev)
        {
            if (!ev.IsAllowed) return;
            API.LastDiedPosition[ev.Player] = ev.Player.Position;
        }

        public void OnDied(DiedEventArgs ev)
        {
            if (GhostSpectator.Configs.SpectatorMode is Mode.SpectatorByDefault)
                return;

            bool wasZombie = ev.TargetOldRole is RoleTypeId.Scp0492;

            Timing.CallDelayed(wasZombie ? 5f : 0.5f, () =>
            {
                if (ev.Player.IsDead)
                {
                    API.Ghostify(ev.Player);
                    if (wasZombie)
                    {
                        Timing.CallDelayed(0.5f, () =>
                        {
                            Scp049ResurrectAbility.DeadZombies.Add(ev.Player.ReferenceHub.netId);
                        });
                    }
                }
            });
        }

        public void OnSpawned(SpawnedEventArgs ev)
        {
            if (ev.Player is null || ev.Player.Role.SpawnReason is PlayerRoles.RoleChangeReason.Destroyed)
                return;

            if (API.IsBecomingGhost.Contains(ev.Player) && ev.Player.Role.Type is RoleTypeId.Tutorial)
            {
                API.IsBecomingGhost.Remove(ev.Player);
                return;
            }

            API.UnGhostify(ev.Player);

            foreach (var ghost in Player.List)
            {
                CheckPlayer(ghost, ev.Player);
            }
        }

        public void OnChangingItem(ChangingItemEventArgs ev)
        {
            if (API.IsGhost(ev.Player) && ev.IsAllowed && ev.NewItem is not null && ev.NewItem.Type is ItemType.Coin && CoinHandler.Coins.TryGetValue(ev.NewItem.Serial, out GhostCoinType type))
            {
                var translation = CoinHandler.CoinTranslation[type];
                ev.Player.ShowHint(translation, 5f);
            }
        }

        public void OnFlippingCoin(FlippingCoinEventArgs ev)
        {
            if (API.IsGhost(ev.Player) && ev.IsAllowed)
            {
                ev.IsAllowed = CoinHandler.Execute(ev.Player, ev.Player.CurrentItem);
            }
        }

        public void OnSpawningRagdoll(SpawningRagdollEventArgs ev)
        {
            if (!ev.IsAllowed || ev.Player is null) return;

            if (API.IsGhost(ev.Player))
                ev.IsAllowed = false;
            else if (API.TimeSinceGhostLast.TryGetValue(ev.Player, out DateTime dt) && (DateTime.UtcNow - dt).TotalSeconds < 3)
                ev.IsAllowed = false;
        }

        public void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker is not null && API.IsGhost(ev.Attacker))
            {
                ev.IsAllowed = false;
            }
        }

        public void OnRestartingRound()
        {
            API.IsBecomingGhost.Clear();
            API.TimeSinceGhostLast.Clear();

            CoinHandler.Coins.Clear();
        }

        public void OnRespawningTeam(RespawningTeamEventArgs ev)
        {
            ev.Players.AddRange(Player.Get(API.IsGhost));
        }

        public void OnDetonated()
        {
            if (!GhostSpectator.Configs.DisableRoomTeleportAfterNuke)
                return;

            foreach (Player ply in Player.Get(API.IsGhost))
            {
                ply.Teleport(CoinHandler.SurfacePosition);
            }
        }

        // Deny ghost actions
        public void GenericGhostDisallow(IPlayerEvent ev)
        {
            if (ev is not IDeniableEvent deny)
            {
                Log.Error($"Event {ev.GetType().Name} cannot be used as a generic ghost disallow event. Show this error message to plugin developer.");
                return;
            }

            if (API.IsGhost(ev.Player))
            {
                deny.IsAllowed = false;
            }
        }

        public void OnFinishingRecall(FinishingRecallEventArgs ev)
        {
            if (!ev.IsAllowed)
                return;

            Vector3 ragdollPosition = ev.Ragdoll.Position + Vector3.up;

            Timing.CallDelayed(0.25f, () =>
            {
                if (ev.Target.Role == RoleTypeId.Scp0492)
                {
                    ev.Target.Teleport(ragdollPosition);
                }
            });
        }

        public void OnAddingTarget(AddingTargetEventArgs ev)
        {
            if (API.IsGhost(ev.Target))
                ev.IsAllowed = false;
        }
    }
}
