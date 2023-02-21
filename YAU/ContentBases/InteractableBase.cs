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
using RoR2.Navigation;
using YAU.Interactables;

namespace YAU.ContentBases {
    public abstract class InteractableBase<T> : InteractableBase where T : InteractableBase<T>{
        public static T Instance { get; private set; }

        public InteractableBase() {
            if (Instance == null) {
                Instance = this as T;
            }
            else {
                YAU.ModLogger.LogError("Attempted to instantiate class " + typeof(T).Name + " when an instance already exists.");
            }
        }
    }

    public abstract class InteractableBase {
        private YAUContentPack contentPack;
        private ConfigFile config;

        public abstract string Name { get; }
        public abstract string Context { get; }
        public virtual string LangToken => Name.ToUpper().RemoveUnsafeCharacters();
        public abstract int CreditCost { get; }
        public abstract int MaxPerStage { get; }
        public abstract string CardName { get; }
        public abstract int Weight { get; }
        public abstract string[] ValidScenes { get; }
        public GameObject InteractablePrefab;
        public InteractableSpawnCard spawnCard;
        public DirectorCard directorCard;
        public virtual bool IsChest { get; }
        public virtual string NameToken => $"INTERACTABLE_{LangToken}_NAME";
        public virtual string ContextToken => $"INTERACTABLE_{LangToken}_CONTEXT";
        public virtual NodeFlags RequiredFlags { get; } = NodeFlags.None;
        public virtual NodeFlags ForbiddenFlags { get; } = NodeFlags.NoChestSpawn;
        public virtual int MinimumStageCompletions { get; } = 0;
        public CustomInteractable customInteractable;

        public void Initialize() {
            LanguageManager.RegisterLanguageToken(NameToken, Name);
            LanguageManager.RegisterLanguageToken(ContextToken, Context);
            Setup();
            CreateSpawnCard();
            CreateDirectorCard();
            customInteractable = new(spawnCard, directorCard, ValidScenes);
            CardManager.RegisterInteractable(customInteractable);
        }

        public virtual void Setup() {

        }

        public virtual void CreateDirectorCard() {
            directorCard = new();
            directorCard.selectionWeight = Weight;
            directorCard.spawnCard = spawnCard;
            directorCard.preventOverhead = false;
            directorCard.minimumStageCompletions = MinimumStageCompletions;
        }

        public virtual void CreateSpawnCard() {
            spawnCard = ScriptableObject.CreateInstance<InteractableSpawnCard>();
            spawnCard.directorCreditCost = CreditCost;
            spawnCard.eliteRules = SpawnCard.EliteRules.Default;
            spawnCard.skipSpawnWhenSacrificeArtifactEnabled = IsChest;
            spawnCard.sendOverNetwork = true;
            spawnCard.occupyPosition = true;
            spawnCard.forbiddenFlags = ForbiddenFlags;
            spawnCard.requiredFlags = RequiredFlags;
            spawnCard.hullSize = HullClassification.Human;
            spawnCard.prefab = InteractablePrefab;
            spawnCard.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
            spawnCard.name = CardName;
        }
    }
}