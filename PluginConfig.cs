using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AutoDoors
{
    public class PluginConfig
    {
        #region Properties - General

        /// <summary>
        /// Enables the mod
        /// </summary>
        public bool ModEnabled { get => modEnabled.Value; set => modEnabled.BoxedValue = value; }
        ConfigEntry<bool> modEnabled;

        /// <summary>
        /// Press to toggle auto doors mod on/off.
        /// </summary>
        public KeyCode EnableKey { get => enableKey.Value; }
        ConfigEntry<KeyCode> enableKey;

        /// <summary>
        /// Hold while interacting with the door to toggle between auto/manual.
        /// </summary>
        public KeyCode ToggleKey { get => toggleKey.Value; }
        ConfigEntry<KeyCode> toggleKey;

        /// <summary>
        /// Minimum interval between updates (s)
        /// </summary>
        public float UpdateInterval { get => updateInterval.Value; }
        ConfigEntry<float> updateInterval;

        /// <summary>
        /// Disables auto doors inside crypts
        /// </summary>
        public bool DisableInCrypt { get => disableInCrypt.Value; }
        ConfigEntry<bool> disableInCrypt;

        #endregion

        #region Methods

        public void Reload()
        {
            if (!AutoDoorPlugin.HasInstance)
                return;

            var config = AutoDoorPlugin.Instance.Config;

            modEnabled = config.Bind("General", "modEnabled", true, "Enables the mod");
            enableKey = config.Bind("General", "enableKey", KeyCode.F6, "Press to toggle auto doors mod on/off.");
            toggleKey = config.Bind("General", "toggleKey", KeyCode.LeftAlt, "Hold while interacting with the door to toggle between auto/manual.");
            updateInterval = config.Bind("General", "updateInterval", 1f/16, "Minimum interval between updates (s)");
            disableInCrypt = config.Bind("General", "disableInCrypt", true, "Disables auto doors inside crypts");
        }

        #endregion
    }
}
