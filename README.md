## Ghost Spectator
An SCP:SL Exiled plugin that turns users into ghosts when they die. Ghosts cannot be seen by alive players (including SCPs), cannot die, and can noclip.

## Config
| Configuration           | Value Type | Description                                                                                                                                                        |
|-------------------------|------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| spawn\_message          | string     | Message to show to ghosts upon spawning\. Set to none to disable\.                                                                                                 |
| spawn\_message\_length  | integer    | Sets the length of time in seconds the spawn message is visible\.                                                                                                  |
| give\_ghost\_navigator  | bool       | If set to true, ghosts will be given an O5 card that they can drop to teleport to gates/checkpoints \(they can't actually drop it for alive players to pick up\)\. |
| navigate\_message       | string     | Set the message to show when a ghost navigates\. Set to none to disable navigate messages\.                                                                        |
| navigate\_fail\_message | string     | The message to show if the teleport fails\.                                                                                                                        |
| can\_ghosts\_teleport   | bool       | If set to true, ghosts will be given a coin that they can drop to teleport to a random alive player\.                                                              |
| teleport\_message       | string     | Set the message to show when a ghost teleports\. Set to none to disable teleport messages\.                                                                        |
| teleport\_none\_message | string     | Set the message to show if a ghost tries to teleport and nobody is alive that can be teleported to\.                                                               |
| trigger\_scps           | bool       | Determines if ghosts can freeze SCP\-173 and trigger SCP\-096\.                                                                                                    |

## Commands
| Command                         | Permission | Description                                |
|---------------------------------|------------|--------------------------------------------|
| ghostspec / gspec <player\(s\)> | gs\.spawn  | Spawns one or more players in as a ghost\. |

## How to get multiple players
- `[playerid].[playerid].[playerid]` - For multiple players separated by player ids
- `*` - For all players
- `%[role]` - For one role type eg. `%classd` for all Class-D.
- `*[zone]` - For a zone (eg. `*light`, `*entrance`, etc)