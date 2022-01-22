using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDoors.GameClasses
{
    [HarmonyPatch(typeof(EnvMan), "SetForceEnvironment")]
    public static class EnvMan_SetForceEnvironment_Patch
    {
        private static void Postfix(string ___m_forceEnv)
        {
            if (!AutoDoorPlugin.IsRunning)
                return;

            AutoDoorPlugin.Instance.IsCrypt = ___m_forceEnv.Contains("Crypt");
        }
    }
}
