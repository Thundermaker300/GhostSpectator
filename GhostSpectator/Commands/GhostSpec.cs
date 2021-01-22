using System;
using System.Collections.Generic;

using CommandSystem;
using Exiled.Permissions.Extensions;
using Player = Exiled.API.Features.Player;

namespace GhostSpectator.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class GhostSpec : ICommand
    {
        public string Command { get; } = "ghostspectator";

        public string[] Aliases { get; } = {"ghostspec", "gspec"};

        public string Description { get; } = "Spawns the targetted player(s) in as a ghost spectator.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender) sender).CheckPermission("gs.spawn"))
            {
                response = "Access denied.";
                return false;
            }

            List<Player> Plys = API.GetPlayers(arguments.At(0));
            int affected = 0;
            foreach (Player Ply in Plys)
            {
                if (!API.IsGhost(Ply))
                {
                    API.GhostPlayer(Ply);
                    affected++;
                }
            }

            response = $"Done! The request affected {affected} players.";
            return true;
        }
    }
}