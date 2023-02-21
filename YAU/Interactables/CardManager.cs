using System;
using System.Collections.Generic;
using RoR2;
using UnityEngine;
using UnityEngine.SceneManagement;
using YAU.Attributes;
using System.Linq;

namespace YAU.Interactables {
    public class CardManager {
        private static List<CustomInteractable> interactables = new();
        
        /// <summary>Registers a CustomInteractable to appear in runs</summary>
        /// <param name="interactable">the CustomInteractable to register</param>
        public static void RegisterInteractable(CustomInteractable interactable) {
            interactables.Add(interactable);
        }

        [AutoRun]
        internal static void SetupHooks() {
            On.RoR2.ClassicStageInfo.RebuildCards += ClassicStageInfo_RebuildCards;
        }

        private static void ClassicStageInfo_RebuildCards(On.RoR2.ClassicStageInfo.orig_RebuildCards orig, ClassicStageInfo self) {
            orig(self);
            foreach (CustomInteractable interactable in interactables) {
                if (interactable.validScenes.ToList().Contains(SceneManager.GetActiveScene().name)) {
                    self.interactableCategories.AddCard(0, interactable.directorCard);
                }
            }
        }
    }

    public class CustomInteractable {
        public InteractableSpawnCard spawnCard;
        public DirectorCard directorCard;
        public string[] validScenes;

        /// <summary>a CustomInteractable instance</summary>
        /// <param name="spawnCard">the InteractableSpawnCard</param>
        /// <param name="directorCard">the DirectorCard</param>
        /// <param name="validScenes">an array of valid scenes for this interactable</param>
        public CustomInteractable(InteractableSpawnCard spawnCard, DirectorCard directorCard, string[] validScenes) {
            this.spawnCard = spawnCard;
            this.directorCard = directorCard;
            this.validScenes = validScenes;
        }
    }
}