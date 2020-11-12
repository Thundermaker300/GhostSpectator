using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;
using UnityEngine;
using PlayableScps;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Mirror;

namespace GhostSpectator.Patches
{
    [HarmonyPatch(typeof(Scp049), nameof(Scp049.BodyCmd_ByteAndGameObject))]
    class Scp049Patch
    {
        [HarmonyPriority(Priority.High)]
        public static bool Prefix(Scp049 __instance, byte num, GameObject go)
        {
            try
            {
                switch (num)
                {
                    case 2:
                        {
                            if (!__instance._interactRateLimit.CanExecute() || go == null)
                                return false;

                            Ragdoll component = go.GetComponent<Ragdoll>();
                            if (component == null)
                                return false;

                            ReferenceHub referenceHub = null;
                            foreach (GameObject player in PlayerManager.players)
                            {
                                ReferenceHub hub = ReferenceHub.GetHub(player);
                                if (hub.queryProcessor.PlayerId == component.owner.PlayerId)
                                {
                                    referenceHub = hub;
                                    break;
                                }
                            }

                            if (referenceHub == null)
                            {
                                return false;
                            }

                            if (!__instance._recallInProgressServer ||
                                referenceHub.gameObject != __instance._recallObjectServer ||
                                __instance._recallProgressServer < 0.85f)
                            {
                                return false;
                            }

                            if (referenceHub.characterClassManager.CurClass != RoleType.Spectator && !API.IsGhost(Player.Get(referenceHub)))
                            {
                                return false;
                            }

                            var ev = new FinishingRecallEventArgs(Player.Get(referenceHub.gameObject), Player.Get(__instance.Hub.gameObject));

                            Exiled.Events.Handlers.Scp049.OnFinishingRecall(ev);

                            if (!ev.IsAllowed)
                                return false;
                            RoundSummary.changed_into_zombies++;
                            referenceHub.characterClassManager.SetClassID(RoleType.Scp0492);
                            referenceHub.GetComponent<PlayerStats>().Health =
                                referenceHub.characterClassManager.Classes.Get(RoleType.Scp0492).maxHP;
                            if (component.CompareTag("Ragdoll"))
                            {
                                NetworkServer.Destroy(component.gameObject);
                            }

                            __instance._recallInProgressServer = false;
                            __instance._recallObjectServer = null;
                            __instance._recallProgressServer = 0f;
                            return false;
                        }

                    default:
                        return true;
                }
            }
            catch (Exception e)
            {
                Log.Error($"StartingAndFinishingRecall: {e}\n{e.StackTrace}");

                return true;
            }
        }
    }
}
