using Exiled.API.Enums;
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
using UnityEngine;

namespace GhostSpectator
{
    public class EventHandler
    {
        public static void CheckPlayer(Player ghost, Player ply)
        {
            if (ghost.Role is not FpcRole fpcRole || ply.Role is not FpcRole)
                return;

            if (!API.IsGhost(ghost))
            {
                fpcRole.IsInvisibleFor.Clear();
                return;
            }

            if (/*ply.SessionVariables.ContainsKey("IsGhost")*/ API.IsGhost(ply))
            {
                if (fpcRole.IsInvisibleFor.Contains(ply))
                    fpcRole.IsInvisibleFor.Remove(ply);
            }
            else
            {
                if (ply.CheckPermission("gs.see"))
                    return;

                if (!fpcRole.IsInvisibleFor.Contains(ply))
                    fpcRole.IsInvisibleFor.Add(ply);
            }
        }

        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (!ev.IsAllowed)
                return;

            if (API.IsGhost(ev.Player)) // Remove coins before they drop!!!
                ev.Player.ClearInventory();

            if (ev.Reason is not Exiled.API.Enums.SpawnReason.Died)
                return;

            API.LastDiedPosition[ev.Player] = ev.Player.Position;

            if (GhostSpectator.Configs.SpectatorMode is Mode.SpectatorByDefault)
                return;

            Timing.CallDelayed(0.5f, () =>
            {
                API.Ghostify(ev.Player);
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
                CoinHandler.Execute(ev.Player, ev.Player.CurrentItem);
            }
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
                ply.Teleport(CoinHandler.SurfacePosition); // Todo: Set fixed position on the surface here
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
