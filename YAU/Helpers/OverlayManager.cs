using System;
using System.Collections.Generic;
using RoR2;
using UnityEngine;
using UnityEngine.SceneManagement;
using YAU.Attributes;
using System.Linq;

namespace YAU.Helpers {
    public class OverlayManager {
        public struct Overlay {
            ///<summary>the material to overlay</summary>
            public Material material;
            ///<summary>the conditions to meet before applying the overlay</summary>
            public Func<CharacterModel, bool> action;
        }

        internal static List<Overlay> overlays = new();
        
        ///<summary>adds an overlay to the list of overlays</summary>
        ///<param name="overlay">the overlay to add</param>
        public static void AddOverlay(Overlay overlay) {
            overlays.Add(overlay);
        }

        ///<summary>creates an overlay and adds it to the list of overlays</summary>
        ///<param name="mat">the overlay material</param>
        ///<param name="action">the conditions for the overlay to be applied</param>
        public static void AddOverlay(Material mat, Func<CharacterModel, bool> action) {
            Overlay overlay = new Overlay {
                material = mat,
                action = action
            };

            overlays.Add(overlay);
        }

        [AutoRun]
        internal static void Setup() {
            On.RoR2.CharacterModel.UpdateOverlays += HandleOverlays;
        }

        private static void HandleOverlays(On.RoR2.CharacterModel.orig_UpdateOverlays orig, CharacterModel self) {
            orig(self);
            foreach (Overlay overlay in overlays) {
                if (overlay.action(self)) {
                    self.currentOverlays[self.activeOverlayCount++] = overlay.material;
                }
            }
        }
    }
}