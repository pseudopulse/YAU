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
    }   
}