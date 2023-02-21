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
    public abstract class EquipmentBase<T> : EquipmentBase where T : EquipmentBase<T>{
        public static T Instance { get; private set; }

        public EquipmentBase() {
            if (Instance == null) {
                Instance = this as T;
            }
            else {
                YAU.ModLogger.LogError("Attempted to instantiate class " + typeof(T).Name + " when an instance already exists.");
            }
        }
    }

    public abstract class EquipmentBase {
        public EquipmentDef EquipmentDef;
        internal ConfigFile config;
        internal YAUContentPack contentPack;

        public abstract string Name { get; }
        public abstract string PickupDescription { get; }
        public abstract string FullDescription { get; }
        public abstract string Lore { get; }
        public virtual bool IsLunar { get; } = false;
        public abstract float Cooldown { get; }
        public virtual bool CanBeRandomlyTriggered { get; } = true;
        public virtual UnlockableDef Unlockable { get; } = null;
        public abstract GameObject PickupModel { get; }
        public abstract Sprite PickupSprite { get; }
        public virtual bool CanDrop { get; } = true;
        public virtual bool CanAppearSingleplayer { get; } = true;
        public virtual bool CanAppearMultiplayer { get; } = true;
        public virtual ExpansionDef RequiredExpansion { get; } = null;
        public virtual string LangToken => this.GetType().Name;
        /// <summary>Run to check whether this equipment should be enabled</summary>
        public virtual Func<EquipmentBase, bool> EnabledCallback { get; } = DefaultEnabledCallback;

        private static bool DefaultEnabledCallback(EquipmentBase self) {
            ConfigSectionAttribute attribute = self.GetType().GetCustomAttribute<ConfigSectionAttribute>();
            if (attribute != null) {
                bool isValid = self.config.Bind<bool>(attribute.name, "Enabled", true, "Allow this equipment to appear in runs?").Value;
                if (isValid) {
                    return true;
                }
                return false;
            }
            else {
                return true;
            }
        }

        public void Initialize(ConfigFile config, YAUContentPack contentPack, string identifier) {
            this.config = config;
            this.contentPack = contentPack;

            EquipmentDef = ScriptableObject.CreateInstance<EquipmentDef>();
            EquipmentDef.name = LangToken;

            $"{identifier}_EQUIPMENT_{LangToken}_NAME".Add(Name);
            $"{identifier}_EQUIPMENT_{LangToken}_DESC".Add(FullDescription);
            $"{identifier}_EQUIPMENT_{LangToken}_PICKUP".Add(PickupDescription);
            $"{identifier}_EQUIPMENT_{LangToken}_LORE".Add(Lore);

            EquipmentDef.nameToken = $"{identifier}_EQUIPMENT_{LangToken}_NAME";
            EquipmentDef.descriptionToken = $"{identifier}_EQUIPMENT_{LangToken}_DESC";
            EquipmentDef.loreToken = $"{identifier}_EQUIPMENT_{LangToken}_LORE";
            EquipmentDef.pickupToken = $"{identifier}_EQUIPMENT_{LangToken}_PICKUP";

            EquipmentDef.isLunar = IsLunar;
            EquipmentDef.cooldown = Cooldown;
            EquipmentDef.canBeRandomlyTriggered = CanBeRandomlyTriggered;
            EquipmentDef.pickupIconSprite = PickupSprite;
            EquipmentDef.pickupModelPrefab = PickupModel;
            EquipmentDef.canDrop = CanDrop;
            EquipmentDef.appearsInMultiPlayer = CanAppearMultiplayer;
            EquipmentDef.appearsInSinglePlayer = CanAppearSingleplayer;
            EquipmentDef.unlockableDef = Unlockable;
            EquipmentDef.requiredExpansion = RequiredExpansion;

            if (EnabledCallback(this)) {
                contentPack.RegisterScriptableObject(EquipmentDef);
            }

            On.RoR2.EquipmentSlot.PerformEquipmentAction += (orig, self, def) => {
                if (def == EquipmentDef) {
                    return OnEquipmentActivation(self);
                }

                return orig(self, def);
            };

            PostCreation();
        }
 
        public virtual void PostCreation() {

        }

        ///<summary>Called when the equipment is triggered, override this to implement equipment functionality</summary>
        ///<param name="activator">the EquipmentSlot that triggered the activation</param>
        public virtual bool OnEquipmentActivation(EquipmentSlot activator) {
            return false;
        }
    }
}