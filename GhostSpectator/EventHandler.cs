using System.Collections.Generic;
using System.Linq;
using Random = System.Random;

using Exiled.API.Features;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.Events.EventArgs;
using UnityEngine;
using Interactables.Interobjects.DoorUtils;
using MEC;
using NorthwoodLib.Pools;

namespace GhostSpectator
{
    public class EventHandler
    {
        private GhostSpectator Plugin { get; }
        private static readonly Random Rng = new Random();

        public EventHandler(GhostSpectator plugin) => Plugin = plugin;

        // Spawning
        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            // Determining visibility
            /*foreach (Player Ply in GhostSpectator.Ghosts)
            {
                // TODO: FIX
                if ((ev.NewRole == RoleType.Spectator || API.IsGhost(ev.Player)) && ev.Player.TargetGhostsHashSet.Contains(Ply.Id))
                {
                    ev.Player.TargetGhostsHashSet.Remove(Ply.Id);
                }
                else if (!ev.Player.TargetGhostsHashSet.Contains(Ply.Id))
                {
                    ev.Player.TargetGhostsHashSet.Add(Ply.Id);
                }
            }*/

            if (!API.IsGhost(ev.Player)) return;
            Log.Debug($"Turning {ev.Player.Nickname} into {ev.NewRole}.");
            if (ev.NewRole == RoleType.Spectator)
            {
                ev.Player.ClearInventory();
            }

            if (!GhostSpectator.SpawnPositions.ContainsKey(ev.Player))
            {
                // Teleport to a new spawn point (eliminate potential issues with noclipping directly after spawning)
                if (ev.NewRole != RoleType.Tutorial && ev.NewRole != RoleType.Spectator)
                {
                    Vector3 spawnPoint = ev.NewRole.GetRandomSpawnPoint();
                    Timing.CallDelayed(0.1f, () => { ev.Player.Position = spawnPoint; });
                }
            }

            API.UnGhostPlayer(ev.Player);
        }

        public void OnFinishingRecall(FinishingRecallEventArgs ev)
        {
            if (!API.IsGhost(ev.Target)) return;
            ev.Target.ClearInventory();
            API.UnGhostPlayer(ev.Target);
        }

        public void OnVerified(VerifiedEventArgs ev)
        {
            if (!Round.IsStarted) return;
            CoroutineHandle ch = Timing.RunCoroutine(JoinedWait(ev.Player));
            Timing.CallDelayed(30f, () => { Timing.KillCoroutines(ch); });
        }

        private IEnumerator<float> JoinedWait(Player ply)
        {
            yield return Timing.WaitUntilTrue(() => ply.Role == RoleType.Spectator);
            yield return Timing.WaitForSeconds(1f);
            API.GhostPlayer(ply);
        }

        public void OnDestroying(DestroyingEventArgs ev)
        {
            if (API.IsGhost(ev.Player))
            {
                API.UnGhostPlayer(ev.Player);
            }
        }

        public void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker != null && API.IsGhost(ev.Attacker))
            {
                ev.IsAllowed = false;
            }
        }

        public void OnDying(DyingEventArgs ev)
        {
            if (API.IsGhost(ev.Target))
            {
                ev.Target.ClearInventory();
            }
            else
            {
                GhostSpectator.SpawnPositions[ev.Target] = API.FindSpawnPosition(ev.Target, ev.HitInformation);
            }
        }

        public void OnDied(DiedEventArgs ev)
        {
            if (API.IsGhost(ev.Target)) return;
            Log.Debug($"Turning {ev.Target.Nickname} into ghost.");
            Timing.CallDelayed(0.1f, () => { API.GhostPlayer(ev.Target); });
        }

        public void OnSpawningRagdoll(SpawningRagdollEventArgs ev)
        {
            if (API.IsGhost(ev.Owner))
            {
                ev.IsAllowed = false;
            }
        }

        public void OnRespawningTeam(RespawningTeamEventArgs ev)
        {
            foreach (Player ply in GhostSpectator.Ghosts)
            {
                ev.Players.Add(ply);
            }
        }

        // Preventing ghosts from interacting
        public void OnActivating(ActivatingEventArgs ev)
        {
            // INTERACTING W/ SCP-914
            if (API.IsGhost(ev.Player) && !Plugin.Config.InteractScp914) ev.IsAllowed = false;
        }

        public void OnStarting(StartingEventArgs ev)
        {
            // INTERACTING W/ WARHEAD
            if (API.IsGhost(ev.Player) && !Plugin.Config.StartWarhead) ev.IsAllowed = false;
        }

        public void OnChangingKnobStatus(ChangingKnobSettingEventArgs ev)
        {
            // INTERACTING W/ SCP-914
            if (API.IsGhost(ev.Player) && !Plugin.Config.InteractScp914) ev.IsAllowed = false;
        }

        public void OnChangingLeverStatus(ChangingLeverStatusEventArgs ev)
        {
            if (API.IsGhost(ev.Player) && !Plugin.Config.ToggleWarhead) ev.IsAllowed = false;
        }

        public void OnEndingRound(EndingRoundEventArgs ev)
        {
            List<Player> alivePlayers = Player.List.Where(ply => !API.IsGhost(ply)).ToList();
            ev.IsRoundEnded = API.AreAllAlly(alivePlayers);
        }

        public void OnDroppingItem(DroppingItemEventArgs ev)
        {
            switch (ev.Item.id)
            {
                case ItemType.Coin when Plugin.Config.CanGhostsTeleport && API.IsGhost(ev.Player):
                {
                    // Todo: setting to allow ghosts to tp to each other
                    List<Player> PlysToTeleport = Player.List.Where(p =>
                            p.Team != Team.RIP && !API.IsGhost(p) && !Plugin.Config.TeleportBlacklist.Contains(p.Role))
                        .ToList();
                    if (PlysToTeleport.Count == 0)
                    {
                        ev.Player.ShowHint(Plugin.Config.TeleportNoneMessage);
                    }
                    else
                    {
                        Player chosen = PlysToTeleport.ElementAt(Rng.Next(PlysToTeleport.Count));
                        if (Plugin.Config.TeleportMessage != "none")
                        {
                            ev.Player.ShowHint(
                                Plugin.Config.TeleportMessage.Replace("{name}", chosen.Nickname).Replace("{class}",
                                    $"<color={chosen.RoleColor.ToHex()}>{Plugin.Config.RoleStrings[chosen.Role]}</color>"),
                                3);
                        }

                        ev.Player.Position = chosen.Position + new Vector3(0, 2, 0);
                    }

                    ev.IsAllowed = false;
                    break;
                }
                case ItemType.WeaponManagerTablet when Plugin.Config.GiveGhostNavigator && API.IsGhost(ev.Player):
                {
                    List<DoorVariant> doors;
                    if (Plugin.Config.NavigateLczAfterDecon == false && Map.IsLCZDecontaminated)
                    {
                        doors = ListPool<DoorVariant>.Shared.Rent(Map.Doors.Where(d =>
                        {
                            Vector3 position;
                            return (position = d.transform.position).y < -100 || position.y > 300;
                        }));
                    }
                    else
                    {
                        doors = ListPool<DoorVariant>.Shared.Rent(Map.Doors.ToList());
                    }

                    DoorVariant chosen = doors.ElementAt(Rng.Next(0, doors.Count - 1));
                    if (Plugin.Config.NavigateMessage != "none" && chosen.TryGetComponent(out DoorNametagExtension ext))
                    {
                        ev.Player.ShowHint(Plugin.Config.TeleportMessage.Replace("{name}", ext.GetName), 3);
                    }

                    if (!PlayerMovementSync.FindSafePosition(chosen.transform.position, out Vector3 safePos))
                    {
                        ev.Player.ShowHint(Plugin.Config.NavigateFailMessage, 3);
                    }

                    ev.Player.Position = safePos;
                    ev.IsAllowed = false;
                    ListPool<DoorVariant>.Shared.Return(doors);
                    break;
                }
                default:
                {
                    if (API.IsGhost(ev.Player) && !Plugin.Config.DropItems) ev.IsAllowed = false;
                    break;
                }
            }
        }

        public void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            if (API.IsGhost(ev.Player) && !Plugin.Config.InteractDoors) ev.IsAllowed = false;
        }

        public void OnInteractingElevator(InteractingElevatorEventArgs ev)
        {
            if (API.IsGhost(ev.Player) && !Plugin.Config.InteractElevators) ev.IsAllowed = false;
        }

        public void OnInteractingLocker(InteractingLockerEventArgs ev)
        {
            if (API.IsGhost(ev.Player) && !Plugin.Config.InteractLockers) ev.IsAllowed = false;
        }

        public void OnOpeningGenerator(OpeningGeneratorEventArgs ev)
        {
            if (API.IsGhost(ev.Player) && !Plugin.Config.InteractGenerators) ev.IsAllowed = false;
        }

        public void OnClosingGenerator(ClosingGeneratorEventArgs ev)
        {
            if (API.IsGhost(ev.Player) && !Plugin.Config.InteractGenerators) ev.IsAllowed = false;
        }

        public void OnInsertingGeneratorTablet(InsertingGeneratorTabletEventArgs ev)
        {
            if (API.IsGhost(ev.Player) && !Plugin.Config.InteractGenerators) ev.IsAllowed = false;
        }

        public void OnEjectingGeneratorTablet(EjectingGeneratorTabletEventArgs ev)
        {
            if (API.IsGhost(ev.Player) && !Plugin.Config.InteractGenerators) ev.IsAllowed = false;
        }

        public void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
        {
            if (API.IsGhost(ev.Player) && !Plugin.Config.TriggerTeslas) ev.IsTriggerable = false;
        }

        public void OnIntercomSpeaking(IntercomSpeakingEventArgs ev)
        {
            if (API.IsGhost(ev.Player) && !Plugin.Config.InteractIntercom) ev.IsAllowed = false;
        }

        public void OnActivatingWorkstation(ActivatingWorkstationEventArgs ev)
        {
            if (API.IsGhost(ev.Player) && !Plugin.Config.InteractWorkstation) ev.IsAllowed = false;
        }

        public void OnDeactivatingWorkstation(DeactivatingWorkstationEventArgs ev)
        {
            if (API.IsGhost(ev.Player) && !Plugin.Config.InteractWorkstation) ev.IsAllowed = false;
        }

        public void OnPickingUpItem(PickingUpItemEventArgs ev)
        {
            if (API.IsGhost(ev.Player) && !Plugin.Config.PickupItems) ev.IsAllowed = false;
        }

        public void OnStopping(StoppingEventArgs ev)
        {
            // INTERACTING W/ WARHEAD
            if (API.IsGhost(ev.Player) && !Plugin.Config.StopWarhead) ev.IsAllowed = false;
        }

        public void OnDetonated()
        {
            foreach (Player ply in Player.List.Where(API.IsGhost))
            {
                if (!Plugin.Config.RemoveItemsAfterNuke) continue;
                ply.ClearInventory();
                if (ply.CurrentRoom.Zone != ZoneType.Surface)
                {
                    ply.Position = new Vector3(0, 1003, 7);
                }
            }
        }

        public void OnAddingTarget(AddingTargetEventArgs ev)
        {
            if (API.IsGhost(ev.Target) && !Plugin.Config.TriggerScps) ev.IsAllowed = false;
        }

        public void On106Containing(ContainingEventArgs ev)
        {
            if (API.IsGhost(ev.ButtonPresser) && !Plugin.Config.Contain106) ev.IsAllowed = false;
        }

        public void OnFemurEnter(EnteringFemurBreakerEventArgs ev)
        {
            if (API.IsGhost(ev.Player) && !Plugin.Config.EnterFemurBreaker) ev.IsAllowed = false;
        }

        public void OnFailingEscapePocketDimension(FailingEscapePocketDimensionEventArgs ev)
        {
            if (!API.IsGhost(ev.Player)) return;
            ev.Player.Position = PlayerMovementSync.FindSafePosition(
                Map.Doors.FirstOrDefault(d => d.Type() == DoorType.Scp106Primary).transform.position,
                out Vector3 safePos)
                ? safePos
                : new Vector3(0, 1003, 7);

            ev.IsAllowed = false;
        }
    }
}