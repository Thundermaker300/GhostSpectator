using System.Collections.Generic;
using System.Linq;
using Random = System.Random;

using Exiled.API.Features;
using Exiled.Events.EventArgs;
using UnityEngine;
using MEC;

namespace GhostSpectator
{
    class EventHandler
    {
        private static Random rng = new Random();
        // Spawning
        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (API.IsGhost(ev.Player))
            {
                // Do not un-ghost them or clear inventory if they are becoming a ghost.
                if (!GhostSpectator.SpawnPositions.ContainsKey(ev.Player))
                {
                    // Clear items from ghost so they don't drop them if they become a spectator.
                    ev.Player.ClearInventory();
                    // Remove ghost effects
                    API.UnGhostPlayer(ev.Player);
                    // Teleport to a new spawn point (eliminate potential issues with noclipping directly after spawning)
                    if (ev.NewRole != RoleType.Tutorial && ev.NewRole != RoleType.Spectator)
                    {
                        Vector3 spawnPoint = Map.GetRandomSpawnPoint(ev.NewRole);
                        Timing.CallDelayed(0.2f, () =>
                        {
                            ev.Player.Position = spawnPoint;
                        });
                    }
                }
            }
        }

        public void Joined(JoinedEventArgs ev)
        {
            API.GhostPlayer(ev.Player);
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
            GhostSpectator.SpawnPositions[ev.Target] = API.FindSpawnPosition(ev.Target, ev.HitInformation);
        }

        public void OnDied(DiedEventArgs ev)
        {
            Timing.CallDelayed(0.2f, () =>
            {
                API.GhostPlayer(ev.Target);
            });
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
            if (API.IsGhost(ev.Player) && !GhostSpectator.Singleton.Config.InteractScp914) ev.IsAllowed = false;
        }

        public void OnStarting(StartingEventArgs ev)
        {
            // INTERACTING W/ WARHEAD
            if (API.IsGhost(ev.Player) && !GhostSpectator.Singleton.Config.StartWarhead) ev.IsAllowed = false;
        }

        public void OnChangingKnobStatus(ChangingKnobSettingEventArgs ev)
        {
            // INTERACTING W/ SCP-914
            if (API.IsGhost(ev.Player) && !GhostSpectator.Singleton.Config.InteractScp914) ev.IsAllowed = false;
        }

        public void OnChangingLeverStatus(ChangingLeverStatusEventArgs ev)
        {
            if (API.IsGhost(ev.Player) && !GhostSpectator.Singleton.Config.ToggleWarhead) ev.IsAllowed = false;
        }

        public void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (ev.Item.id == ItemType.Coin && GhostSpectator.Singleton.Config.CanGhostsTeleport == true && API.IsGhost(ev.Player))
            {
                // Todo: setting to allow ghosts to tp to each other
                List<Player> PlysToTeleport = Player.List.Where(p => p.Team != Team.RIP && p.Team != Team.TUT).ToList();
                if (PlysToTeleport.Count == 0)
                {
                    ev.Player.ShowHint(GhostSpectator.Singleton.Config.TeleportNoneMessage);
                }
                else
                {
                    int index = rng.Next(0, PlysToTeleport.Count() - 1);
                    Player Chosen = PlysToTeleport.ElementAt(index);
                    if (GhostSpectator.Singleton.Config.TeleportMessage != "none")
                    {
                        ev.Player.ShowHint(GhostSpectator.Singleton.Config.TeleportMessage.Replace("{name}", Chosen.Nickname).Replace("{class}", $"<color={Chosen.RoleColor.ToHex()}>{API.RoleInfo[Chosen.Role.ToString()]}</color>"), 3);
                    }
                    ev.Player.Position = Chosen.Position + new Vector3(0, 2, 0);
                }
                ev.IsAllowed = false;
            }
            else if (ev.Item.id == ItemType.KeycardO5 && GhostSpectator.Singleton.Config.GiveGhostNavigator == true && API.IsGhost(ev.Player))
            {
                List<Door> Doors = Map.Doors.ToList();
                Door chosen = Doors.ElementAt(rng.Next(0, Doors.Count - 1));
                if (GhostSpectator.Singleton.Config.NavigateMessage != "none")
                {
                    ev.Player.ShowHint(GhostSpectator.Singleton.Config.TeleportMessage.Replace("{name}", chosen.DoorName), 3);
                }
                if (!PlayerMovementSync.FindSafePosition(chosen.transform.position, out Vector3 safePos))
                {
                    ev.Player.ShowHint(GhostSpectator.Singleton.Config.NavigateFailMessage, 3);
                }
                ev.Player.Position = safePos;
                ev.IsAllowed = false;
            }
            else
            {
                if (API.IsGhost(ev.Player) && !GhostSpectator.Singleton.Config.DropItems) ev.IsAllowed = false;
            }
        }

        public void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            if (API.IsGhost(ev.Player) && !GhostSpectator.Singleton.Config.InteractDoors) ev.IsAllowed = false;
        }

        public void OnInteractingElevator(InteractingElevatorEventArgs ev)
        {
            if (API.IsGhost(ev.Player) && !GhostSpectator.Singleton.Config.InteractElevators) ev.IsAllowed = false;
        }

        public void OnInteractingLocker(InteractingLockerEventArgs ev)
        {
            if (API.IsGhost(ev.Player) && !GhostSpectator.Singleton.Config.InteractLockers) ev.IsAllowed = false;
        }

        public void OnOpeningGenerator(OpeningGeneratorEventArgs ev)
        {
            if (API.IsGhost(ev.Player) && !GhostSpectator.Singleton.Config.InteractGenerators) ev.IsAllowed = false;
        }

        public void OnClosingGenerator(ClosingGeneratorEventArgs ev)
        {
            if (API.IsGhost(ev.Player) && !GhostSpectator.Singleton.Config.InteractGenerators) ev.IsAllowed = false;
        }

        public void OnInsertingGeneratorTablet(InsertingGeneratorTabletEventArgs ev)
        {
            if (API.IsGhost(ev.Player) && !GhostSpectator.Singleton.Config.InteractGenerators) ev.IsAllowed = false;
        }

        public void OnEjectingGeneratorTablet(EjectingGeneratorTabletEventArgs ev)
        {
            if (API.IsGhost(ev.Player) && !GhostSpectator.Singleton.Config.InteractGenerators) ev.IsAllowed = false;
        }

        public void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
        {
            if (API.IsGhost(ev.Player) && !GhostSpectator.Singleton.Config.TriggerTeslas) ev.IsTriggerable = false;
        }

        public void OnIntercomSpeaking(IntercomSpeakingEventArgs ev)
        {
            if (API.IsGhost(ev.Player) && !GhostSpectator.Singleton.Config.InteractIntercom) ev.IsAllowed = false;
        }

        public void OnPickingUpItem(PickingUpItemEventArgs ev)
        {
            if (API.IsGhost(ev.Player) && !GhostSpectator.Singleton.Config.PickupItems) ev.IsAllowed = false;
        }

        public void OnStopping(StoppingEventArgs ev)
        {
            // INTERACTING W/ WARHEAD
            if (API.IsGhost(ev.Player) && !GhostSpectator.Singleton.Config.StopWarhead) ev.IsAllowed = false;
        }

        public void On106Containing(ContainingEventArgs ev)
        {
            if (API.IsGhost(ev.ButtonPresser))
            {
                ev.IsAllowed = false;
            }
        }
    }
}
