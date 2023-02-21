using RoR2.ContentManagement;
using RoR2;
using System.Collections.Generic;
using YAU;
using System.Reflection;

namespace YAU.Content {
    public class ContentPackManager {
        private static List<YAUContentPack> contentPacks = new();

        /// <summary>Creates an instance of YAUContentPack</summary>
        /// <param name="assembly">the assembly to check types from (for auto asset loading), assembly name is used as default identifier</param>
        /// <returns>the new YAUContentPack</returns>
        public static YAUContentPack CreateContentPack(Assembly assembly) {
            YAUContentPack pack = new YAUContentPack(assembly);
            YAU.ModLogger.LogInfo("Created content pack for assembly: " + assembly.FullName);
            contentPacks.Add(pack);
            return pack;
        }

        /// <summary>Creates an instance of YAUContentPack</summary>
        /// <param name="identifier">the identifier for the content pack</param>
        /// <returns>the new YAUContentPack</returns>
        public static YAUContentPack CreateContentPack(string identifier) {
            YAUContentPack pack = new YAUContentPack(identifier);
            YAU.ModLogger.LogInfo("Created content pack: " + identifier);
            contentPacks.Add(pack);
            return pack;
        }

        /// <summary>Creates an instance of YAUContentPack</summary>
        /// <param name="assembly">the assembly to check types from (for auto asset loading)</param>
        /// <param name="identifier">the identifier for the content pack, overrides the default of using assembly name</param>
        /// <returns>the new YAUContentPack</returns>
        public static YAUContentPack CreateContentPack(Assembly assembly, string identifier) {
            YAUContentPack pack = new YAUContentPack(assembly, identifier);
            YAU.ModLogger.LogInfo($"Created content pack {identifier} for assembly {assembly.FullName}");
            contentPacks.Add(pack);
            return pack;
        }   
    }
}