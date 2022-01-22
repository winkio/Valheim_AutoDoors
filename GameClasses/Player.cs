using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using Logger = BepInEx.Logging.Logger;

namespace AutoDoors.GameClasses
{
    [HarmonyPatch(typeof(Player), "Update")]
    public static class Player_Update_Patch
    {
        private static void Postfix(Player __instance)
        {
            if (!AutoDoorPlugin.IsRunning)
                return;

            var player = Player.m_localPlayer;
            if (player != __instance)
                return;

            bool validPlayer = player != null && !player.IsDead();
            var timeNow = DateTime.UtcNow;
            bool updateIntervalPassed = (timeNow - AutoDoorPlugin.Instance.LastUpdate).TotalSeconds >= AutoDoorPlugin.Instance.Cfg.UpdateInterval;

            if (AutoDoorPlugin.Instance.IsActive && validPlayer && updateIntervalPassed)
            {
                AutoDoorPlugin.Instance.LastUpdate = timeNow;

                var radius = player.m_maxInteractDistance;
                var rsq = radius * radius;

                // remove any invalid doors
                AutoDoorPlugin.Instance.TrackedDoors.RemoveAll(td => !td.IsValid);

                // check doors in range
                foreach (var td in AutoDoorPlugin.Instance.TrackedDoors)
                {
                    if (!td.Update())
                        continue;

                    var obj = UnityEngine.Object.FindObjectFromInstanceID(td.Id);
                    if (obj is Door d)
                    {
                        var prevInAutoRange = td.InAutoRange;
                        var dsq = Vector3.SqrMagnitude(d.transform.position - player.transform.position);
                        td.InAutoRange = dsq <= rsq;
                        if (td.InAutoRange)
                        {
                            if (!td.IsManual)
                            {
                                if (td.State == 0)
                                {
                                    if (!td.IsAutoOpened)
                                    {
                                        d.Interact(player, false, false);
                                    }
                                    else
                                    {
                                        td.IsManual = true;
                                        //AutoDoorPlugin.InstanceLogger.LogInfo($"winkio.autodoors - door {td.Id} is now manual 1");
                                    }
                                }
                                else
                                {
                                    if (!prevInAutoRange)
                                    {
                                        td.IsManual = true;
                                        //AutoDoorPlugin.InstanceLogger.LogInfo($"winkio.autodoors - door {td.Id} is now manual 2");
                                    }
                                    else
                                    {
                                        td.IsAutoOpened = true;
                                    }
                                }
                            }
                        }
                        else if (prevInAutoRange)
                        {
                            if (!td.IsManual)
                            {
                                td.SetState(0);
                                //AutoDoorPlugin.InstanceLogger.LogInfo($"winkio.autodoors - auto close {td.Id}");
                            }
                            else
                            {
                                td.IsManual = false;
                            }
                            td.IsAutoOpened = false;
                        }
                    }
                }

            }

        }

    }
}
