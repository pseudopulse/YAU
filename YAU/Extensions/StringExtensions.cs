using System;
using System.Collections.Generic;
using RoR2;
using YAU;
using UnityEngine.AddressableAssets;
using YAU.Language;

namespace YAU.Extensions.Text {
    public static class StringExtensions {
        
        /// <summary>Adds a string-value pair to the ROR2 language strings</summary>
        /// <param name="text">the value when the string is searched</param>
        public static void Add(this string str, string text) {
            LanguageManager.RegisterLanguageToken(str, text);
        }
        
        /// <summary>Attempts to load the string through AddressableAssets</summary>
        /// <returns>the asset if it was found, otherwise returns default(T)</returns>
        public static T Load<T>(this string str) {
            try {
                T asset = Addressables.LoadAssetAsync<T>(str).WaitForCompletion();
                return asset;
            }
            catch {
                YAU.ModLogger.LogError($"Failed to load asset {str} of type {typeof(T).ToString()}. Returned default(T) instead.");
                return default(T);
            }
        }

        /// <summary>
        /// Filters unsafe characters from a string. Default characters: \n ' (whitespace) ! ` - () {} | @ . \
        /// </summary>
        /// <returns>the filtered string</returns>
        public static string RemoveUnsafeCharacters(this string str) {
            string[] unsafeCharacters = { "\n", "'", " ", "!", "`", "&", "-", ")", "(", "{", "}", "|", "@", "<", ">", ".", "\\"};
            string filtered = str;

            foreach (string c in unsafeCharacters) {
                filtered = filtered.Replace(c, "");
            }

            return filtered;
        }

        /// <summary>
        /// Filters unsafe characters from a string.
        /// </summary>
        /// <param name="unsafeChars">an array of characters to filter out</param>
        /// <returns>the filtered string</returns>
        public static string RemoveUnsafeCharacters(this string str, string[] unsafeChars) {
            string filtered = str;

            foreach (string c in unsafeChars) {
                filtered = filtered.Replace(c, "");
            }

            return filtered;
        }

        /// <summary>
        /// Uses a list of pre-defined matches to attempt to automatically format a string
        /// </summary>
        /// <returns>the formatted string</returns>
        public static string AutoFormat(this string str) {
            return Formatter.FormatString(str);
        }
    }   

    internal class Formatter {
        internal struct Format {
            public string formatted;
            public List<string> matching;
        }

        private static List<Format> formats = new() {
            new Format { // healing
                formatted = "<style=cIsHealing>%TEXT%</style>",
                matching = new() {
                    "heal", "health", "barrier", "temporary barrier"
                }
            },
            new Format { // stack left
                formatted = "<style=cStack>%TEXT%",
                matching = new() { "(" }
            },
            new Format { // stack right
                formatted = "%TEXT%</style>",
                matching = new() { ")" }
            },
            new Format { // utility
                formatted = "<style=cIsUtility>%TEXT%</style>",
                matching = new() {
                    "shield", "gain", "regenerating", "speed", "boost", "buffs", "increase", "armor"
                }
            },
            new Format { // damage
                formatted = "<style=cIsDamage>%TEXT%</style>",
                matching = new() {
                    "damage", 
                }
            }
        };

        internal static string FormatString(string str) {
            foreach (Format format in formats) {
                foreach (string match in format.matching) {
                    string replace = format.formatted.Replace("%TEXT%", match);
                    str = str.Replace(match, replace);
                }
            }

            return str;
        }
    }
}