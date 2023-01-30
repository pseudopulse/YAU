using System;
using UnityEngine;
using RoR2;
using YAU.Content;
using BepInEx.Configuration;
using YAU.Extensions.Text;
using YAU.Language;
using System.Collections.Generic;
using System.Linq;
using RoR2.ExpansionManagement;

namespace YAU.ContentBases {
    public abstract class ItemBase<T> : ItemBase where T : ItemBase<T>{
        public static T Instance { get; private set; }

        public ItemBase() {
            if (Instance == null) {
                Instance = this as T;
            }
            else {
                YAU.ModLogger.LogError("Attempted to instantiate class " + typeof(T).Name + " when an instance already exists.");
            }
        }
    }

    public abstract class ItemBase {
        private YAUContentPack contentPack;
        private ConfigFile config;
        public abstract string Name { get; }
        public virtual string TokenName => Name.ToUpper().RemoveUnsafeCharacters();
        public virtual string ConfigSafeName => Name;
        public abstract string FullDescription { get; }
        public abstract string PickupDescription { get; } 
        public abstract string Lore { get; }
        public abstract Sprite Icon { get; }
        public abstract ItemTier ItemTier { get; }
        public virtual ItemTierDef ItemTierDef { get; } = null;
        public abstract Enum[] ItemTags { get; }
        public abstract GameObject PickupModelPrefab { get; }
        public virtual UnlockableDef Unlockable { get; } = null;
        public virtual bool CanRemove { get; } = true;
        public virtual ExpansionDef RequiredExpansion { get; } = null;
        public ItemDef ItemDef;

        public void Initialize(YAUContentPack pack, ConfigFile config) {
            this.contentPack = pack;
            this.config = config;

            ItemDef item = ScriptableObject.CreateInstance<ItemDef>();
            item.pickupIconSprite = Icon;
            item.pickupModelPrefab = PickupModelPrefab;
            if (ItemTierDef) {
                item._itemTierDef = ItemTierDef;
            }
            else {
                #pragma warning disable
                item.deprecatedTier = ItemTier;
                #pragma warning restore
            }
            item.canRemove = CanRemove;

            string nameToken = $"ITEM_{TokenName}_NAME";
            string descToken = $"ITEM_{TokenName}_DESC";
            string pickupToken = $"ITEM_{TokenName}_PICKUP";
            string loreToken = $"ITEM_{TokenName}_LORE";

            LanguageManager.RegisterLanguageToken(nameToken, Name);
            LanguageManager.RegisterLanguageToken(descToken, FullDescription);
            LanguageManager.RegisterLanguageToken(pickupToken, PickupDescription);
            LanguageManager.RegisterLanguageToken(loreToken, Lore);

            item.pickupToken = pickupToken;
            item.nameToken = nameToken;
            item.descriptionToken = descToken;
            item.loreToken = loreToken;
            
            List<ItemTag> tags = new();
            foreach (Enum tag in ItemTags) {
                tags.Add((ItemTag)tag);
            }

            item.tags = tags.ToArray();
            item.requiredExpansion = RequiredExpansion;

            if (config.Bind<bool>($"Items: {ConfigSafeName}", "Enabled", true, "Allow this item to appear in runs?").Value) {
                contentPack.RegisterScriptableObject(item);
            }
        }
    }
}