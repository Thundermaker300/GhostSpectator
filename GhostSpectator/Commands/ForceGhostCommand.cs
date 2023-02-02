namespace GhostSpectator.Commands
{
    using System;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ForceGhostCommand : ICommand
    {
        public string Command => "forceghostspectate";

        public string[] Aliases => new[] { "fgs" };

        public string Description => "Turns a player into a ghost.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("gs.forcespectate"))
            {
                response = "Missing permission: gs.force";
                return false;
            }

            if (!Round.InProgress)
            {
                response = "The round is not in progress.";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Missing argument: player";
                return false;
            }

            Player target = Player.Get(arguments.At(0));
            
            if (target is null)
            {
                response = "Invalid player specified.";
                return false;
            }

            if (API.IsGhost(target))
            {
                response = "Targeted player is already a ghost.";
                return true;
            }

            if (target.Role.Type is PlayerRoles.RoleTypeId.Overwatch)
            {
                response = "Targeted player is in overwatch.";
                return false;
            }

            API.Ghostify(target);

            response = $"Done! {target.Nickname} is now a ghost.";
            return true;
        }
    }
}
