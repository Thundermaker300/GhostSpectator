namespace GhostSpectator.Commands
{
    using System;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using Exiled.Permissions.Extensions;

    [CommandHandler(typeof(ClientCommandHandler))]
    public class GsCommand : ICommand
    {
        public string Command => "ghostspectate";

        public string[] Aliases => new[] { "gs" };

        public string Description => $"Switch between ghost and regular spectating.{(GhostSpectator.Configs.RequirePermission ? " 'gs.mode' permission required." : string.Empty)}";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!Player.TryGet(sender, out Player ply))
            {
                response = "This command must be executed by a player!";
                return false;
            }

            if (GhostSpectator.Configs.RequirePermission && !ply.CheckPermission("gs.mode"))
            {
                response = "Missing permission: gs.mode";
                return false;
            }

            if (!Round.InProgress)
            {
                response = "The round is not in progress.";
                return false;
            }

            if (API.IsGhost(ply))
            {
                ply.Role.Set(PlayerRoles.RoleTypeId.Spectator);
                response = "Set to spectator.";
                return true;
            }
            else if (ply.Role is SpectatorRole)
            {
                API.Ghostify(ply);
                response = "Set to ghost.";
                return true;
            }
            else if (ply.IsOverwatchEnabled)
            {
                response = "You cannot use this command while in overwatch.";
                return false;
            }

            response = "You cannot use this command while you are alive.";
            return false;
        }
    }
}
