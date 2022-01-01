![GHOSTSPECTATOR VERSION](https://img.shields.io/github/v/release/Thundermaker300/GhostSpectator?include_prereleases&style=for-the-badge)
![GHOSTSPECTATOR LINES](https://img.shields.io/tokei/lines/github/Thundermaker300/GhostSpectator?style=for-the-badge)
![GHOSTSPECTATOR DOWNLOADS](https://img.shields.io/github/downloads/Thundermaker300/GhostSpectator/total?style=for-the-badge)

## Ghost Spectator
An SCP:SL Exiled plugin that turns users into ghosts when they die. Ghosts cannot be seen by alive players (including SCPs), cannot die, and can noclip.

Current Plugin Version: V1.1.7

## REQUIREMENTS
* Exiled: V2.8.0
* SCP:SL Server: V10.2.2

## Config
What ghosts can interact with is not shown on this list but is configurable and will be generated along with the rest of these settings.
| Configuration              | Value Type | Description                                                                                                                                                        |
|----------------------------|------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| ghost\_role                | RoleType   | Determines the role to use for ghosts. Chaos Insurgency is used by default so that ghosts are not targets to SCPs. If you set to an SCP, they can still attack!!   |
| spawn\_message             | string     | Message to show to ghosts upon spawning\. Set to none to disable\.                                                                                                 |
| spawn\_message\_length     | integer    | Sets the length of time in seconds the spawn message is visible\.                                                                                                  |
| ghost\_spec\_swap          | bool       | If set to true, ghosts can swap to spectator mode via the \.spec command in the server console, and vice versa\.                                                   |
| ghost\_spec\_permission    | bool       | If set to true, the gs.spectate permission will be required to use the .spec command. If this is true, ghost_spec_swap should also be true.                        |
| start\_as\_spectator       | bool       | If set to true, players will become spectators when they die. If this is true, ghost_spec_swap should also be true (otherwise the plugin is useless).              |
| give\_ghost\_navigator     | bool       | If set to true, ghosts will be given a workstation that they can drop to teleport to a random door in the map.                                                     |
| navigate\_lcz\_after\_decon| bool       | If set to true, ghosts can navigate to doors in light containment after decontamination has begun\.                                                                |
| remove\_items\_after\_nuke | bool       | If set to true, ghosts will lose items after the nuke has detonated (they will also be teleported to the surface).                                                 |
| navigate\_message          | string     | Set the message to show when a ghost navigates\. Set to none to disable navigate messages\.                                                                        |
| navigate\_fail\_message    | string     | The message to show if the teleport fails\.                                                                                                                        |
| can\_ghosts\_teleport      | bool       | If set to true, ghosts will be given a coin that they can drop to teleport to a random alive player\.                                                              |
| teleport\_blacklist        | List       | A list of roles that CANNOT be teleported to \(eg Scp173, NtfCadet, etc\)\.                                                                                        |
| teleport\_message          | string     | Set the message to show when a ghost teleports\. Set to none to disable teleport messages\.                                                                        |
| teleport\_none\_message    | string     | Set the message to show if a ghost tries to teleport and nobody is alive that can be teleported to\.                                                               |
| trigger\_scps              | bool       | Determines if ghosts can freeze SCP\-173 and trigger SCP\-096\.                                                                                                    |
| role\_strings              | List       | Sets the string for roles in place of \{class\} in the above strings \(for example, replacing Class-D Personnel with DBOI will make it say DBOI in game\)\.        |

## Commands
| Command                         | Permission | Description                                                      |
|---------------------------------|------------|------------------------------------------------------------------|
| ghostspec / gspec <player\(s\)> | gs\.spawn  | Spawns one or more players in as a ghost\.                       |
| .spec                           | N/A        | Changes the speaker from a ghost to a spectator and vice versa\. |

## How to get multiple players
- `[playerid].[playerid].[playerid]` - For multiple players separated by player ids
- `*` - For all players
- `%[role]` - For one role type eg. `%classd` for all Class-D.
- `*[zone]` - For a zone (eg. `*light`, `*entrance`, etc)
