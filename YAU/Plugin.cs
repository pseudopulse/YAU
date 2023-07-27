using BepInEx;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Reflection;

// yau stuff
using YAU.Attributes;

namespace YAU {
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    
    public class YAU : BaseUnityPlugin {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "pseudopulse";
        public const string PluginName = "YAU";
        public const string PluginVersion = "1.0.0";

        public static BepInEx.Logging.ManualLogSource ModLogger;

        internal static YAUContentPack YAUContent;

        public void Awake() {
            // set logger
            ModLogger = Logger;

            YAUContent = ContentPackManager.CreateContentPack(Assembly.GetExecutingAssembly(), "YAUInternals");

            // collect auto run attributes
            AutoRunCollector.HandleAutoRun();
        }
    }
}