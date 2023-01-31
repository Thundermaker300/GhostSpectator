using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace GhostSpectator
{
    public class EventHandler
    {
        public void CheckPlayer(Player ghost, Player ply)
        {
            if (ghost.Role is not FpcRole fpcRole || ply.Role is not FpcRole)
                return;

            if (ply.SessionVariables.ContainsKey("IsGhost"))
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
            if (!ev.IsAllowed || ev.Reason is not Exiled.API.Enums.SpawnReason.Died)
                return;

            Vector3 pos = ev.Player.Position;

            Timing.CallDelayed(0.5f, () =>
            {
                ev.Player.Role.Set(RoleTypeId.Tutorial, SpawnReason.ForceClass, RoleSpawnFlags.None);

                if (ev.Player.Role.Is(out FpcRole fpc))
                {
                    fpc.IsInvisible = true;
                    Timing.CallDelayed(1f, () =>
                    {
                        fpc.IsInvisible = false;
                    });
                }

                ev.Player.Position = pos;

                API.Ghostify(ev.Player);

                foreach (var player in Player.List)
                    CheckPlayer(ev.Player, player);
            });
        }

        public void OnSpawned(SpawnedEventArgs ev)
        {
            if (ev.Player is null || ev.Player.Role.SpawnReason is PlayerRoles.RoleChangeReason.Destroyed)
                return;

            if (API.IsBecomingGhost.Contains(ev.Player))
            {
                API.IsBecomingGhost.Remove(ev.Player);
                return;
            }

            foreach (var ghost in Player.Get(API.IsGhost))
            {
                CheckPlayer(ghost, ev.Player);
            }

            API.UnGhostify(ev.Player);
        }
    }
}
