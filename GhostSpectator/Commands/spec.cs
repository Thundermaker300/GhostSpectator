using System;

using Exiled.API.Features;
using CommandSystem;

namespace GhostSpectator.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class spec : ICommand
    {
        public string Command => "spec";

        public string[] Aliases => new string[] { };

        public string Description => "Switches from ghost to normal spectator mode.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (GhostSpectator.Singleton.Config.GhostSpecSwap == false)
            {
                response = "This command is disabled.";
                return false;
            }
            Player Ply = Player.Get(((CommandSender)sender).Nickname);
            if (Ply == null)
            {
                response = "The command speaker is not a player.";
                return false;
            }
            if (API.IsGhost(Ply))
            {
                Ply.SetRole(RoleType.Spectator);
            }
            else
            {
                API.GhostPlayer(Ply);
            }
            response = "Success";
            return true;
        }
    }
}
