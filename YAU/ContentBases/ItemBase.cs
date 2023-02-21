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
using YAU.AddressableUtils;
using YAU.Attributes;
using System.Reflection;

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
        public virtual string TokenName => this.GetType().Name;
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
        /// <summary>Run to check whether this item should be enabled</summary>
        public virtual Func<ItemBase, bool> EnabledCallback { get; } = DefaultEnabledCallback;

        private static bool DefaultEnabledCallback(ItemBase self) {
            ConfigSectionAttribute attribute = self.GetType().GetCustomAttribute<ConfigSectionAttribute>();
            if (attribute != null) {
                bool isValid = self.config.Bind<bool>(attribute.name, "Enabled", true, "Allow this item to appear in runs?").Value;
                if (isValid) {
                    return true;
                }
                return false;
            }
            else {
                return true;
            }
        }

        public void Initialize(YAUContentPack pack, ConfigFile config, string identifier) {
            this.contentPack = pack;
            this.config = config;

            ItemDef item = ScriptableObject.CreateInstance<ItemDef>();
            item.name = TokenName;
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

            string nameToken = $"{identifier}_ITEM_{TokenName}_NAME";
            string descToken = $"{identifier}_ITEM_{TokenName}_DESC";
            string pickupToken = $"{identifier}_ITEM_{TokenName}_PICKUP";
            string loreToken = $"{identifier}_ITEM_{TokenName}_LORE";

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

            ItemDef = item;

            if (EnabledCallback(this)) {
                contentPack.RegisterScriptableObject(item);
            }

            PostCreation();
        }

        public virtual void PostCreation() {

        }

        public bool HasItem(CharacterBody body) {
            return body.inventory ? body.inventory.GetItemCount(ItemDef) > 0 : false;
        }

        public bool HasItem(CharacterMaster master) {
            return master.inventory ? master.inventory.GetItemCount(ItemDef) > 0 : false;
        }

        public GameObject CreateIconCube(Sprite icon) {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            GameObject parent = new GameObject(ConfigSafeName + "_IconCube").CreatePrefab(ConfigSafeName + "_IconCube");
            cube.transform.SetParent(parent.transform);
            Material mat = new(AddressableUtils.Assets.Shader.HGStandard);
            mat.mainTexture = icon.texture;
            cube.GetComponent<MeshRenderer>().material = mat;
            cube.transform.localScale *= 2;
            return parent;
        }

        public GameObject CreateIconCube(Texture icon) {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            GameObject parent = new GameObject(ConfigSafeName + "_IconCube").CreatePrefab(ConfigSafeName + "_IconCube");
            cube.transform.SetParent(parent.transform);
            Material mat = new(AddressableUtils.Assets.Shader.HGStandard);
            mat.mainTexture = icon;
            cube.GetComponent<MeshRenderer>().material = mat;
            cube.transform.localScale *= 2;
            return parent;
        }

        public GameObject CreateIconPlane(Sprite icon) {
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            GameObject parent = new GameObject(ConfigSafeName + "_IconPlane").CreatePrefab(ConfigSafeName + "_IconPlane");
            plane.transform.SetParent(parent.transform);
            Material mat = new(AddressableUtils.Assets.Shader.HGStandard);
            mat.mainTexture = icon.texture;
            plane.GetComponent<MeshRenderer>().material = mat;
            plane.transform.localScale *= 2;
            plane.transform.localRotation = Quaternion.Euler(-90, 0, 0);
            return parent;
        }

        public GameObject CreateIconPlane(Texture icon) {
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            GameObject parent = new GameObject(ConfigSafeName + "_IconPlane").CreatePrefab(ConfigSafeName + "_IconPlane");
            plane.transform.SetParent(parent.transform);
            Material mat = new(AddressableUtils.Assets.Shader.HGStandard);
            mat.mainTexture = icon;
            plane.GetComponent<MeshRenderer>().material = mat;
            plane.transform.localScale *= 2;
            plane.transform.localRotation = Quaternion.Euler(-90, 0, 0);
            return parent;
        }
    }
}