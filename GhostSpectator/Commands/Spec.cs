using System;

using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using CommandSystem;

namespace GhostSpectator.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Spec : ICommand
    {
        public string Command => "spec";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "Switches from ghost to normal spectator mode and vice versa.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("gs.spectate") && GhostSpectator.Singleton.Config.GhostSpecPermission)
            {
                response = "You don't have permission to use that command.";
                return false;
            }
            if (GhostSpectator.Singleton.Config.GhostSpecSwap == false)
            {
                response = "This command is disabled.";
                return false;
            }

            Player ply = Player.Get(((CommandSender) sender).SenderId);
            if (ply == null || ply.IsHost)
            {
                response = "The command speaker is not a player.";
                return false;
            }

            if (API.IsGhost(ply))
            {
                ply.SetRole(RoleType.Spectator);
            }
            else
            {
                if (ply.IsAlive)
                {
                    response = "This command cannot be used to change from an alive player to a ghost.";
                    return false;
                }

                API.GhostPlayer(ply);
            }

            response = "Success";
            return true;
        }
    }
}
