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
    [HarmonyPatch(typeof(Door), "UpdateState")]
    public static class Door_UpdateState_Patch
    {
        private static void Postfix(ref Door __instance, ZNetView ___m_nview)
        {
            if (!AutoDoorPlugin.IsRunning)
                return;

            if (AutoDoorPlugin.Instance.IsActive && __instance.m_keyItem == null)
            {
                var id = __instance.GetInstanceID();
                if (!AutoDoorPlugin.Instance.TrackedDoors.Any(td => td.Id == id))
                    AutoDoorPlugin.Instance.TrackedDoors.Add(new TrackedDoor(id, ___m_nview));
            }
        }
    }

    [HarmonyPatch(typeof(Door), "GetHoverText")]
    static class Door_GetHoverText_Patch
    {
        static void Postfix(Door __instance, ref string __result, ZNetView ___m_nview)
        {
            if (!AutoDoorPlugin.IsRunning || ___m_nview.GetZDO() == null)
            {
                return;
            }

            var id = __instance.GetInstanceID();
            TrackedDoor foundDoor = AutoDoorPlugin.Instance.TrackedDoors.FirstOrDefault(td => td.Id == id);
            if (foundDoor.IsValid)
            {
                __result += Localization.instance.Localize($"\n[<color=yellow><b>{AutoDoorPlugin.Instance.Cfg.ToggleKey}</b></color>+<color=yellow><b>$KEY_Use</b></color>] to set {(foundDoor.IsAutomatic ? "manual" : "automatic")}");
            }
        }
    }

    [HarmonyPatch(typeof(Door), "Interact")]
    public static class Door_InteractState_Patch
    {
        static bool Prefix(Door __instance, ZNetView ___m_nview, Humanoid character)
        {

            if (!AutoDoorPlugin.IsRunning || !___m_nview.GetZDO().IsValid() || !(character is Player) || !Input.GetKey(AutoDoorPlugin.Instance.Cfg.ToggleKey))
            {
                return true;
            }

            if (AutoDoorPlugin.Instance.IsActive && __instance.m_keyItem == null)
            {
                var id = __instance.GetInstanceID();
                TrackedDoor foundDoor = AutoDoorPlugin.Instance.TrackedDoors.FirstOrDefault(td => td.Id == id);
                if (foundDoor.IsValid)
                {
                    foundDoor.IsAutomatic = !foundDoor.IsAutomatic;
                    AutoDoorPlugin.InstanceLogger.LogInfo("Door set to " + (foundDoor.IsAutomatic ? "auto" : "manual"));
                }
                return false;
            }

            return true;
        }
    }
}
