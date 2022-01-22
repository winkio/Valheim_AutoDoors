using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AutoDoors
{
    [BepInPlugin("winkio.autodoors", "Auto Doors", "1.0.0")]
    public class AutoDoorPlugin : BaseUnityPlugin
    {
        #region Properties - Static

        /// <summary>
        /// Plugin instance
        /// </summary>
        public static AutoDoorPlugin Instance { get; private set; }

        /// <summary>
        /// Flag for it the plugin has a current instance
        /// </summary>
        public static bool HasInstance { get => Instance != null; }

        /// <summary>
        /// Flag for it the plugin has a current instance and is enabled
        /// </summary>
        public static bool IsRunning { get => Instance != null && Instance.enabled; }

        /// <summary>
        /// Logger
        /// </summary>
        public static ManualLogSource InstanceLogger { get => Instance.Logger; }

        #endregion

        #region Properties

        /// <summary>
        /// Mod config
        /// </summary>
        public PluginConfig Cfg { get; private set; } = new PluginConfig();

        /// <summary>
        /// Is the player currently in a crypt
        /// </summary>
        public bool IsCrypt { get; set; }

        /// <summary>
        /// Is the mod actively running (either not in a crypt or disable in crypt is set to false)
        /// </summary>
        public bool IsActive { get => !Cfg.DisableInCrypt || !IsCrypt; }

        /// <summary>
        /// Time of last update 
        /// </summary>
        public DateTime LastUpdate { get; set; }

        /// <summary>
        /// List of tracked doors 
        /// </summary>
        public List<TrackedDoor> TrackedDoors { get; private set; } = new List<TrackedDoor>();

        /// <summary>
        /// List of Ids of auto doors in range 
        /// </summary>
        public List<int> AutoDoorIds { get; private set; } = new List<int>();

        /// <summary>
        /// List of Ids of manual doors in range 
        /// </summary>
        public List<int> ManualDoorIds { get; private set; } = new List<int>();

        #endregion

        #region Methods

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {Info.Metadata.GUID} loading...");

            // set static instance
            Instance = this;

            // reload config
            Cfg.Reload();

            // reset door lists
            TrackedDoors.Clear();
            AutoDoorIds.Clear();
            ManualDoorIds.Clear();

            // create and apply patches
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);

            Logger.LogInfo($"Plugin {Info.Metadata.GUID} loading complete!");
        }

        #endregion
    }
}
