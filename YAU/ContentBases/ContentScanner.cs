using System;
using YAU;
using YAU.Content;
using System.Linq;
using RoR2;
using BepInEx.Configuration;
using System.Reflection;
using System.Collections.Generic;
using YAU.Enumeration;

namespace YAU.ContentBases {
    public static class ContentScanner {
        /// <summary>Scans an assembly for classes inheriting from ContentBases and initializes them</summary>
        /// <param name="assembly">the assembly to scan</param>
        /// <param name="pack">the YAUContentPack to add to</param>
        /// <param name="config">the ConfigFile to pass to the ContentBase</param>
        public static void ScanContent(Assembly assembly, YAUContentPack pack, ConfigFile config) {
            string identifier = pack.identifier.ToUpper();
            ScanTypes<ItemBase>(assembly, x => x.Initialize(pack, config, identifier));
            ScanTypes<SkillBase>(assembly, x => x.Initialize(pack, identifier));
            ScanTypes<InteractableBase>(assembly, x => x.Initialize());
            ScanTypes<ItemTierBase>(assembly, x => x.Initialize(pack));
            ScanTypes<EquipmentBase>(assembly, x => x.Initialize(config, pack, identifier));
        }

        internal static void ScanTypes<T>(Assembly assembly, Action<T> action) {
            IEnumerable<Type> types = assembly.GetTypes().Where(x => !x.IsAbstract && x.IsSubclassOf(typeof(T)));

            foreach (Type type in types) {
                T instance = (T)Activator.CreateInstance(type);
                action(instance);
            }
        }
    }
}