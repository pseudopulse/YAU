using System;
using System.Collections.Generic;
using RoR2;
using UnityEngine;
using UnityEngine.SceneManagement;
using YAU.Attributes;
using System.Linq;
using HarmonyLib;

namespace YAU.Helpers {
    public class VoidManager {
        public struct Corruption {
            public ItemDef uncorrupt;
            public List<ItemDef> corrupts;
        }

        private static List<Corruption> corruptions = new();
        
        ///<summary>Registers a TransformationPair for the void corruption of two items</summary>
        ///<param name="uncorrupted">the uncorrupted version of the item</param>
        ///<param name="corrupted">the void equivalent</param>
        public static void AddCorruption(ItemDef uncorrupted, ItemDef corrupted) {
            corruptions.Add(new() {
                uncorrupt = uncorrupted,
                corrupts = new() { corrupted }
            });
        }
        
        ///<summary>Registers a TransformationPair for the void corruption of a group of items</summary>
        ///<param name="uncorrupted">the uncorrupted version of the item</param>
        ///<param name="corrupted">the void equivalents</param>
        public static void AddCorruption(ItemDef uncorrupted, List<ItemDef> corrupted) {
            corruptions.Add(new() {
                uncorrupt = uncorrupted,
                corrupts = corrupted
            });
        }

        [AutoRun]
        internal static void HandleCorruptions() {
            On.RoR2.Items.ContagiousItemManager.Init += (orig) => {
                foreach (Corruption corruption in corruptions) {
                    foreach (ItemDef def in corruption.corrupts) {
                        ItemDef.Pair transformation = new() {
                            itemDef1 = corruption.uncorrupt,
                            itemDef2 = def
                        };

                        ItemCatalog.itemRelationships[DLC1Content.ItemRelationshipTypes.ContagiousItem] = ItemCatalog.itemRelationships[DLC1Content.ItemRelationshipTypes.ContagiousItem].AddToArray(transformation);
                    }
                }
                orig();
            };
        }
    }
}