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
}
