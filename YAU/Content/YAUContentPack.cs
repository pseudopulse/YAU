using RoR2.ContentManagement;
using RoR2;
using System.Collections;
using System.Collections.Generic;
using YAU;
using RoR2.Skills;
using RoR2.EntitlementManagement;
using UnityEngine;
using EntityStates;
using RoR2.Projectile;
using System;
using System.Reflection;

namespace YAU.Content {
    public class YAUContentPack : IContentPackProvider {
        private string _identifier;
        private ContentPack pack = new();
        private Assembly assembly;

        // content lists
        private List<ItemDef> itemDefs = new();
        private List<SurvivorDef> survivorDefs = new();
        private List<EquipmentDef> equipmentDefs = new();
        private List<ArtifactDef> artifactDefs = new();
        private List<BuffDef> buffDefs = new();
        private List<UnlockableDef> unlockableDefs = new();
        private List<SkillDef> skillDefs = new();
        private List<EntitlementDef> entitlementDefs = new();
        private List<EliteDef> eliteDefs = new();
        private List<GameEndingDef> gameEndingDefs = new();
        private List<GameObject> bodyPrefabs = new();
        private List<GameObject> masterPrefabs = new();
        private List<GameObject> projectilePrefabs = new();
        private List<GameObject> gameModePrefabs = new();
        private List<SkillFamily> skillFamilies = new();
        private List<Type> entityStates = new();
        private List<ItemTierDef> itemTierDefs = new();
        private List<EffectDef> effectDefs = new();
        private List<GameObject> networkedObjects = new();
        public string identifier {
            get => _identifier;
            set => _identifier = value;
        }

        public YAUContentPack(Assembly assembly) {
            this.assembly = assembly;
            identifier = assembly.GetName().CodeBase;

            ContentManager.collectContentPackProviders += (addContentPackProvider) => {
                addContentPackProvider(this);
            };
        }

        public YAUContentPack(string identifier) {
            this.identifier = identifier;

            ContentManager.collectContentPackProviders += (addContentPackProvider) => {
                addContentPackProvider(this);
            };
        }

        public YAUContentPack(Assembly assembly, string identifier) {
            this.assembly = assembly;
            this.identifier = identifier;

            ContentManager.collectContentPackProviders += (addContentPackProvider) => {
                addContentPackProvider(this);
            };
        }

        internal void RegisterNetworkedObject(GameObject asset) {
            networkedObjects.Add(asset);
        }

        // content load methods

        /// <summary>Adds a ScriptableObject of a valid type to the ContentPack</summary>
        /// <param name="asset">the ScriptableObject to add</param>
        public void RegisterScriptableObject(ScriptableObject asset) {
            if (asset as ItemDef) {
                itemDefs.Add(asset as ItemDef);
            }

            if (asset as SurvivorDef) {
                survivorDefs.Add(asset as SurvivorDef);
            }

            if (asset as BuffDef) {
                buffDefs.Add(asset as BuffDef);
            }

            if (asset as SkillDef) {
                skillDefs.Add(asset as SkillDef);
            }

            if (asset as GameEndingDef) {
                gameEndingDefs.Add(asset as GameEndingDef);
            }

            if (asset as UnlockableDef) {
                unlockableDefs.Add(asset as UnlockableDef);
            }

            if (asset as EliteDef) {
                eliteDefs.Add(asset as EliteDef);
            }

            if (asset as EntitlementDef) {
                entitlementDefs.Add(asset as EntitlementDef);
            }

            if (asset as ItemTierDef) {
                itemTierDefs.Add(asset as ItemTierDef);
            }

            if (asset as EquipmentDef) {
                equipmentDefs.Add(asset as EquipmentDef);
            }
        }

        /// <summary>Adds a GameObject of a valid type to the ContentPack</summary>
        /// <param name="asset">the GameObject to add</param>
        public void RegisterGameObject(GameObject asset) {
            if (asset.GetComponent<CharacterBody>()) {
                bodyPrefabs.Add(asset);
            }

            if (asset.GetComponent<CharacterMaster>()) {
                masterPrefabs.Add(asset);
            }

            if (asset.GetComponent<ProjectileController>()) {
                projectilePrefabs.Add(asset);
            }

            if (asset.GetComponent<Run>()) {
                gameModePrefabs.Add(asset);
            }

            if (asset.GetComponent<EffectComponent>()) {
                EffectDef def = new EffectDef();
                def._prefab = asset;
                def.prefabName = asset.name;
                def.prefabEffectComponent = asset.GetComponent<EffectComponent>();
                effectDefs.Add(def);
            }
        }

        /// <summary>Adds a SkillFamily to the ContentPack</summary>
        /// <param name="asset">the SkillFamily to add</param>
        public void RegisterSkillFamily(SkillFamily asset) {
            skillFamilies.Add(asset);
        }

        /// <summary>Adds an EntityState to the ContentPack</summary>
        public void RegisterEntityState<T>() where T : EntityState {
            entityStates.Add(typeof(T));
        }

        /// <summary>Adds an EntityState to the ContentPack</summary>
        public void RegisterEntityState(Type stateType) {
            if (!stateType.IsAssignableFrom(typeof(EntityState))) {
                throw new Exception("Attempted to register state that wasn't assignable from type EntityState. This isn't allowed");
            }
            else {
                entityStates.Add(stateType);
            }
        }

        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args) {
            pack.artifactDefs.Add(artifactDefs.ToArray());
            pack.itemDefs.Add(itemDefs.ToArray());
            pack.equipmentDefs.Add(equipmentDefs.ToArray());
            pack.survivorDefs.Add(survivorDefs.ToArray());
            pack.buffDefs.Add(buffDefs.ToArray());
            pack.unlockableDefs.Add(unlockableDefs.ToArray());
            pack.skillDefs.Add(skillDefs.ToArray());
            pack.entitlementDefs.Add(entitlementDefs.ToArray());
            pack.eliteDefs.Add(eliteDefs.ToArray());
            pack.gameEndingDefs.Add(gameEndingDefs.ToArray());
            pack.gameModePrefabs.Add(gameModePrefabs.ToArray());
            pack.bodyPrefabs.Add(bodyPrefabs.ToArray());
            pack.masterPrefabs.Add(masterPrefabs.ToArray());
            pack.projectilePrefabs.Add(projectilePrefabs.ToArray());
            pack.skillFamilies.Add(skillFamilies.ToArray());
            pack.entityStateTypes.Add(entityStates.ToArray());
            pack.itemTierDefs.Add(itemTierDefs.ToArray());
            pack.effectDefs.Add(effectDefs.ToArray());
            pack.networkedObjectPrefabs.Add(networkedObjects.ToArray());

            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args) {
            pack.identifier = identifier;
            ContentPack.Copy(pack, args.output);
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator FinalizeAsync(FinalizeAsyncArgs args) {
            args.ReportProgress(1f);
            yield break;
        }
    }
}