using System.Collections.Generic;
using System.Linq;
using Random = System.Random;

using Exiled.API.Features;
using Exiled.Events.EventArgs;
using UnityEngine;
using MEC;

namespace GhostSpectator
{
    public class EventHandler
    {
        public GhostSpectator Plugin { get; private set; }
        private static Random rng = new Random();

        public EventHandler(GhostSpectator plugin) => Plugin = plugin;

        // Spawning
        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (API.IsGhost(ev.Player))
            {
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
                        Vector3 spawnPoint = Map.GetRandomSpawnPoint(ev.NewRole);
                        Timing.CallDelayed(0.1f, () =>
                        {
                            ev.Player.Position = spawnPoint;
                        });
                    }
                }
                API.UnGhostPlayer(ev.Player);
            }
        }

        public void OnFinishingRecall(FinishingRecallEventArgs ev)
        {
            if (API.IsGhost(ev.Target))
            {
                ev.Target.ClearInventory();
                API.UnGhostPlayer(ev.Target);
            }
        }

        public void Joined(JoinedEventArgs ev)
        {
            if (Round.IsStarted)
            {
                Timing.CallDelayed(0.1f, () =>
                {
                    if (ev.Player.Role == RoleType.Spectator)
                    {
                        API.GhostPlayer(ev.Player);
                    }
                });
            }
        }

        public void Left(LeftEventArgs ev)
        {
            if (API.IsGhost(ev.Player))
            {
                API.UnGhostPlayer(ev.Player);
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
            if (!API.IsGhost(ev.Target))
            {
                Log.Debug($"Turning {ev.Target.Nickname} into ghost.");
                Timing.CallDelayed(0.1f, () =>
                {
                    API.GhostPlayer(ev.Target);
                });
            }
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
            foreach (Player Ply in GhostSpectator.Ghosts)
            {
                ev.Players.Add(Ply);
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
            List<Player> AlivePlayers = Player.List.Where(Ply => !API.IsGhost(Ply)).ToList();
            ev.IsRoundEnded = API.AreAllAlly(AlivePlayers); ;
        }

        public void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (ev.Item.id == ItemType.Coin && Plugin.Config.CanGhostsTeleport == true && API.IsGhost(ev.Player))
            {
                // Todo: setting to allow ghosts to tp to each other
                List<Player> PlysToTeleport = Player.List.Where(p => p.Team != Team.RIP && !API.IsGhost(p) && !Plugin.Config.TeleportBlacklist.Contains(p.Role)).ToList();
                if (PlysToTeleport.Count == 0)
                {
                    ev.Player.ShowHint(Plugin.Config.TeleportNoneMessage);
                }
                else
                {
                    Player Chosen = PlysToTeleport.ElementAt(rng.Next(PlysToTeleport.Count));
                    if (Plugin.Config.TeleportMessage != "none")
                    {
                        ev.Player.ShowHint(Plugin.Config.TeleportMessage.Replace("{name}", Chosen.Nickname).Replace("{class}", $"<color={Chosen.RoleColor.ToHex()}>{Plugin.Config.RoleStrings[Chosen.Role]}</color>"), 3);
                    }
                    ev.Player.Position = Chosen.Position + new Vector3(0, 2, 0);
                }
                ev.IsAllowed = false;
            }
            else if (ev.Item.id == ItemType.WeaponManagerTablet && Plugin.Config.GiveGhostNavigator == true && API.IsGhost(ev.Player))
            {
                List<Door> Doors;
                if (Plugin.Config.NavigateLczAfterDecon == false && Map.IsLCZDecontaminated)
                {
                    Doors = Map.Doors.Where(d => d.transform.position.y < -100 || d.transform.position.y > 300).ToList();
                }
                else
                {
                    Doors = Map.Doors.ToList();
                }
                Door chosen = Doors.ElementAt(rng.Next(0, Doors.Count - 1));
                if (Plugin.Config.NavigateMessage != "none")
                {
                    ev.Player.ShowHint(Plugin.Config.TeleportMessage.Replace("{name}", chosen.DoorName), 3);
                }
                if (!PlayerMovementSync.FindSafePosition(chosen.transform.position, out Vector3 safePos))
                {
                    ev.Player.ShowHint(Plugin.Config.NavigateFailMessage, 3);
                }
                ev.Player.Position = safePos;
                ev.IsAllowed = false;
            }
            else
            {
                if (API.IsGhost(ev.Player) && !Plugin.Config.DropItems) ev.IsAllowed = false;
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

        public void On106Containing(ContainingEventArgs ev)
        {
            if (API.IsGhost(ev.ButtonPresser) && !Plugin.Config.Contain106) ev.IsAllowed = false;
        }
        public void OnFemurEnter(EnteringFemurBreakerEventArgs ev)
        {
            if (API.IsGhost(ev.Player) && !Plugin.Config.EnterFemurBreaker) ev.IsAllowed = false;
        }
    }
}
