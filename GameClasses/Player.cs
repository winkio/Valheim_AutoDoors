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

            if (Input.GetKeyDown(AutoDoorPlugin.Instance.Cfg.EnableKey))
            {
                AutoDoorPlugin.Instance.Cfg.ModEnabled = !AutoDoorPlugin.Instance.Cfg.ModEnabled;
                MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, $"Auto Doors mod {(AutoDoorPlugin.Instance.Cfg.ModEnabled ? "enabled." : "disabled.")}");
            }
            var modEnabled = AutoDoorPlugin.Instance.Cfg.ModEnabled.Equals(true);


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
                        var rangeChange = prevInAutoRange != td.InAutoRange;
                        var modToggleChange = modEnabled != td.lastModState;
                        td.lastModState = modEnabled;

                        if (modToggleChange)
                        {
                            if (!modEnabled)
                            {// mod disabled
                                if (td.IsAutomatic && td.State != 0)
                                {// automatic doors should close
                                    td.SetState(0);
                                }
                            } 
                            else
                            {// mod enabled
                                if (td.IsAutomatic)
                                {// door is automatic
                                    if(td.State == 0 && td.InAutoRange)
                                    {// door is closed and in range so it should open
                                        d.Interact(player, false, false);
                                    } else if(td.State != 0 && !td.InAutoRange)
                                    {// door is open and not in range so it should close
                                        td.SetState(0);
                                    }
                                }
                            }
                        } else if (rangeChange && modEnabled && td.IsAutomatic)
                        {// player has entered or exited interaction range of an automatic door
                            if(td.InAutoRange && td.State == 0)
                            {// should open on approach
                                d.Interact(player, false, false);
                            } 
                            else if(!td.InAutoRange && td.State != 0)
                            {// should close when player walks away
                                td.SetState(0);
                            }
                        }

                    }

                }
            

            }

        }

    }
}
