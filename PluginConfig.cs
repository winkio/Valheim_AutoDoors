using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDoors
{
    public class PluginConfig
    {
        #region Properties - General

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

            updateInterval = config.Bind("General", "updateInterval", 1f/16, "Minimum interval between updates (s)");
            disableInCrypt = config.Bind("General", "disableInCrypt", true, "Disables auto doors inside crypts");
        }

        #endregion
    }
}
