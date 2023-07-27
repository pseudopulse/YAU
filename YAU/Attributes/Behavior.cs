using System.Reflection;
using System.Collections;
using System;
using System.Linq;
using BepInEx.Configuration;
using HG.Reflection;
using System.Collections.Generic;
using RoR2;
using RoR2.Items;

namespace YAU.Attributes {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ItemBehaviorAttribute : SearchableAttribute {
        public ItemDef item;
        public string name;
        public ItemBehaviorAttribute(string name) {
            this.name = name;
        }
    }

    public class ItemBehaviorAttributeHandler {
        private static List<ItemBehaviorAttribute> instances = new();
        [AutoRun]
        internal static void Hook() {
            On.RoR2.ItemCatalog.SetItemDefs += (orig, self) => {
                orig(self);
                Initialize();
            };

            On.RoR2.CharacterBody.OnInventoryChanged += (orig, self) => {
                orig(self);
                foreach (ItemIndex index in self.inventory.itemAcquisitionOrder) {
                    ItemDef def = ItemCatalog.GetItemDef(index);
                    if (LinkedBehaviorExists(def, out ItemBehaviorAttribute attr)) {
                        int c = self.inventory.GetItemCount(index);
                        MethodInfo method = typeof(CharacterBody).GetMethod(nameof(CharacterBody.AddItemBehavior));
                        Type t = (attr.target as TypeInfo).AsType();
                        method = method.MakeGenericMethod(t);
                        method.Invoke(self, new object[] { c });
                    }
                }
            };
        }
        internal static void Initialize() {
            List<ItemBehaviorAttribute> instancesL = new();
            SearchableAttribute.GetInstances<ItemBehaviorAttribute>(instancesL);

            foreach (ItemBehaviorAttribute attr in instancesL) {
                if (!(attr.target as TypeInfo).IsSubclassOf(typeof(CharacterBody.ItemBehavior))) {
                    YAU.ModLogger.LogDebug("Target isn't assignable from ItemBehavior, skipping.");
                    continue;
                }
                ItemDef def = ItemCatalog.GetItemDef(ItemCatalog.FindItemIndex(attr.name));
                if (!def) {
                    YAU.ModLogger.LogDebug("Couldn't find ItemDef, skipping.");
                    continue;
                }
                attr.item = def;
                instances.Add(attr);
            }
        }

        private static bool LinkedBehaviorExists(ItemDef item, out ItemBehaviorAttribute attribute) {
            foreach (ItemBehaviorAttribute attr in instances) {
                if (attr.item == item) {
                    attribute = attr;
                    return true;
                }
            }
            attribute = null;
            return false;
        }
    }
}