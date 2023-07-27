using RoR2;
using System.Collections.Generic;
using YAU;
using System;
using YAU.Attributes;

namespace YAU.Language {
    public class LanguageManager {
        private static Dictionary<string, string> languageTokens = new();

        [AutoRun]
        internal static void Initialize() {
            On.RoR2.Language.GetLocalizedStringByToken += Language_GetLocalizedStringByToken;
            On.RoR2.Language.TokenIsRegistered += Language_TokenIsRegistered;
        }
        
        /// <summary>Adds a key-value pair to the RoR2 language pairs.</summary>
        /// <param name="key">the key</param>
        /// <param name="value">the value</param>
        public static void RegisterLanguageToken(string key, string value) {
            if (languageTokens.ContainsKey(key)) {
                YAU.ModLogger.LogError($"Attempted to add key {key} when the same key was already present.");
                return;
            }

            languageTokens.Add(key, value);
        }

        // hooks

        private static string Language_GetLocalizedStringByToken(On.RoR2.Language.orig_GetLocalizedStringByToken orig, RoR2.Language self, string token) {
            if (languageTokens.ContainsKey(token)) {
                return languageTokens[token];
            }
            else {
                return orig(self, token);
            }
        }

        private static bool Language_TokenIsRegistered(On.RoR2.Language.orig_TokenIsRegistered orig, RoR2.Language self, string token) {
            if (languageTokens.ContainsKey(token)) {
                return true;
            }
            return orig(self, token);
        }
    }
}