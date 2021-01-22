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

        public string Description { get; } = "Spawns the targeted player(s) in as a ghost spectator.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("gs.spawn"))
            {
                response = "Access denied.";
                return false;
            }

            List<Player> plys = API.GetPlayers(arguments.At(0));
            int affected = 0;
            foreach (Player ply in plys)
            {
                if (API.IsGhost(ply)) continue;
                API.GhostPlayer(ply);
                affected++;
            }

            response = $"Done! The request affected {affected} players.";
            return true;
        }
    }
}