using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandSystem;
using Exiled.Permissions.Extensions;
using Exiled.API.Features;
using Exiled.Events.Handlers;
using MEC;
using Exiled.API.Enums;

using Player = Exiled.API.Features.Player;

namespace GhostSpectator.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    class ghostspec : ICommand
    {
        public string Command { get; set; } = "ghostspectator";

        public string[] Aliases { get; set; }  = { "ghostspec", "gspec" };

        public string Description { get; set; } = "Spawns the targetted player(s) in as a ghost spectator.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("gs.spawn"))
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
