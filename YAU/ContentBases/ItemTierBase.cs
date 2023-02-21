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
    public abstract class ItemTierBase<T> : ItemTierBase where T : ItemTierBase<T>{
        public static T Instance { get; private set; }

        public ItemTierBase() {
            if (Instance == null) {
                Instance = this as T;
            }
            else {
                YAU.ModLogger.LogError("Attempted to instantiate class " + typeof(T).Name + " when an instance already exists.");
            }
        }
    }

    public abstract class ItemTierBase {
        public abstract ColorCatalog.ColorIndex ColorIndex { get; }
        public abstract ColorCatalog.ColorIndex DarkColorIndex { get; }
        /// <summary>does the Shrine of Order work on this tier?</summary>
        public virtual bool CanRestack { get; } = true;
        public abstract bool CanScrap { get; } 
        public abstract GameObject DropletPrefab { get; }
        public abstract GameObject HighlightPrefab { get; }
        public abstract ItemTierDef.PickupRules PickupRules { get; }
        public abstract Texture2D BackgroundTexture { get; }
        public virtual bool IsDroppable { get; } = true;
        public abstract string Name { get; }
        public static ItemTierDef tierDef;
        public static ItemTier itemTier => tierDef.tier;
        public void Initialize(YAUContentPack pack) {
            tierDef = ScriptableObject.CreateInstance<ItemTierDef>();
            tierDef.colorIndex = ColorIndex;
            tierDef.darkColorIndex = DarkColorIndex;
            tierDef.canRestack = CanRestack;
            tierDef.canScrap = CanScrap;
            tierDef.pickupRules = PickupRules;
            tierDef.bgIconTexture = BackgroundTexture;
            tierDef.tier = ItemTier.AssignedAtRuntime;
            tierDef.name = Name;
            tierDef.dropletDisplayPrefab = DropletPrefab;
            tierDef.highlightPrefab = HighlightPrefab;
            tierDef.isDroppable = IsDroppable;

            Setup();

            pack.RegisterScriptableObject(tierDef);
        }

        public virtual void Setup() {

        }
    }
}