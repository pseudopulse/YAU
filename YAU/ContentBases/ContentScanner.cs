using System;
using YAU;
using YAU.Content;
using System.Linq;
using RoR2;
using BepInEx.Configuration;
using System.Reflection;
using System.Collections.Generic;

namespace YAU.ContentBases {
    public static class ContentScanner {
        /// <summary>Scans an assembly for classes inheriting from ContentBases and initializes them</summary>
        /// <param name="assembly">the assembly to scan</param>
        /// <param name="pack">the YAUContentPack to add to</param>
        /// <param name="config">the ConfigFile to pass to the ContentBase</param>
        public static void ScanContent(Assembly assembly, YAUContentPack pack, ConfigFile config) {
            // ItemBase
            IEnumerable<Type> itemTypes = assembly.GetTypes().Where(x => !x.IsAbstract && x.IsSubclassOf(typeof(ItemBase)));

            foreach (Type itemType in itemTypes) {
                ItemBase item = (ItemBase)Activator.CreateInstance(itemType);
                item.Initialize(pack, config);
            }
        }
    }
}