using System;
using System.Collections.Generic;
using RoR2;
using UnityEngine;
using UnityEngine.SceneManagement;
using YAU.Attributes;
using System.Linq;

namespace YAU.Helpers {
    public class DeployableManager {

        private static Dictionary<DeployableSlot, Func<CharacterMaster, int>> deployables = new();

        ///<summary>Handles limiting a deployable</summary>
        ///<param name="slot">the DeployableSlot</param>
        ///<param name="limit">the callback used to decide the limit</param>
        public static void AddDeployable(DeployableSlot slot, Func<CharacterMaster, int> limit) {
            deployables.Add(slot, limit);
        }

        [AutoRun]
        internal static void Initialize() {
            On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += HandleLimits;
        }

        private static int HandleLimits(On.RoR2.CharacterMaster.orig_GetDeployableSameSlotLimit orig, CharacterMaster self, DeployableSlot slot) {
            if (deployables.TryGetValue(slot, out Func<CharacterMaster, int> limit)) {
                return limit(self);
            }

            return orig(self, slot);
        }
    }
}