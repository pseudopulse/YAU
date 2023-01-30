using System.Reflection;
using System.Collections;
using System;
using System.Collections.Generic;
using RoR2;
using HarmonyLib;
using System.Linq;

namespace YAU.Attributes {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    internal sealed class CorruptibleAttribute : Attribute {
        public ItemDef corruption;
        public CorruptibleAttribute(ItemDef corruption) {
            this.corruption = corruption;
        }
    }

    internal sealed class CorruptibleCollector {
        private static Dictionary<ItemDef, List<ItemDef>> CorruptionPairs = new();
        internal static void CollectCorruptions() {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            List<Type> types = new();
            foreach (Assembly assembly in assemblies) {
                types = types.Concat(assembly.GetTypes()).ToList();
            }

            foreach (Type type in types) {
                foreach (PropertyInfo info in type.GetProperties((BindingFlags)(-1))) {
                    if (!info.GetGetMethod().IsStatic) {
                        YAU.ModLogger.LogError(info.Name + " attempted to use the Corruptible attribute, but wasn't static");
                        return;
                    }

                    if (info.GetValue(null) is ItemDef) {
                        // pass
                    }
                    else {
                        YAU.ModLogger.LogError(info.Name + " attempted to use the Corruptible attribute, but wasn't of type ItemDef");
                        return;
                    }
                    List<CorruptibleAttribute> attrs = info.GetCustomAttributes<CorruptibleAttribute>().ToList();
                    ItemDef def = info.GetValue(null) as ItemDef;
                    foreach (CorruptibleAttribute attr in attrs) {
                        if (CorruptionPairs.ContainsKey(def)) {
                            CorruptionPairs[def].Add(attr.corruption);
                        }
                        else {
                            CorruptionPairs.Add(def, new() { attr.corruption });
                        }
                    }
                }

                foreach (FieldInfo info in type.GetFields((BindingFlags)(-1))) {
                    if (!info.IsStatic) {
                        YAU.ModLogger.LogError(info.Name + " attempted to use the Corruptible attribute, but wasn't static");
                        return;
                    }

                    if (info.GetValue(null) is ItemDef) {
                        // pass
                    }
                    else {
                        YAU.ModLogger.LogError(info.Name + " attempted to use the Corruptible attribute, but wasn't of type ItemDef");
                        return;
                    }
                    List<CorruptibleAttribute> attrs = info.GetCustomAttributes<CorruptibleAttribute>().ToList();
                    ItemDef def = info.GetValue(null) as ItemDef;
                    foreach (CorruptibleAttribute attr in attrs) {
                        if (CorruptionPairs.ContainsKey(def)) {
                            CorruptionPairs[def].Add(attr.corruption);
                        }
                        else {
                            CorruptionPairs.Add(def, new() { attr.corruption });
                        }
                    }
                }
            }
        }

        [AutoRun]
        internal static void Initialize() {
            On.RoR2.ItemCatalog.SetItemRelationships += ItemCatalog_SetItemRelationships;
        }

        private static void ItemCatalog_SetItemRelationships(On.RoR2.ItemCatalog.orig_SetItemRelationships orig, ItemRelationshipProvider[] providers) {
            orig(providers);
            CollectCorruptions();
            foreach (KeyValuePair<ItemDef, List<ItemDef>> pair in CorruptionPairs) {
                foreach (ItemDef def in pair.Value) {
                    ItemDef.Pair itemPair = new ItemDef.Pair {
                        itemDef1 = pair.Key,
                        itemDef2 = def
                    };

                    ItemCatalog.itemRelationships[DLC1Content.ItemRelationshipTypes.ContagiousItem] = ItemCatalog.itemRelationships[DLC1Content.ItemRelationshipTypes.ContagiousItem].AddToArray(itemPair);
                }
            }
        }
    }
}