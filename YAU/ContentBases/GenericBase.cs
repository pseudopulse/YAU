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
    public abstract class GenericBase<T> : GenericBase where T : GenericBase<T>{
        public static T Instance { get; private set; }

        public GenericBase() {
            if (Instance == null) {
                Instance = this as T;
            }
            else {
                YAU.ModLogger.LogError("Attempted to instantiate class " + typeof(T).Name + " when an instance already exists.");
            }
        }
    }

    public abstract class GenericBase {
        public YAUContentPack contentPack;
        public ConfigFile config;

        public virtual void Initialize(YAUContentPack pack, ConfigFile config, string identifier) {
            PostCreation();
        }

        public virtual void PostCreation() {

        }
    }
}