using System;

namespace YAU.Interfaces {
    public interface IOnBuffInflictedReceiver {
        void OnBuffInflicted(BuffDef buff);
    }

    internal class OnBuffInflictedHandler {
        internal static void Initialize() {
            On.RoR2.CharacterBody.AddBuff_BuffIndex += HandleUntimed;
        }

        internal static void HandleUntimed(On.RoR2.CharacterBody.orig_AddBuff_BuffIndex orig, CharacterBody self, BuffIndex index) {
            orig(self, index);
            IOnBuffInflictedReceiver[] bodyReceivers = self.GetComponents<IOnBuffInflictedReceiver>();
            foreach (IOnBuffInflictedReceiver receiver in bodyReceivers) {
                receiver.OnBuffInflicted(BuffCatalog.GetBuffDef(index));
            }

            if (self.master) {
                IOnBuffInflictedReceiver[] masterReceivers = self.master.GetComponents<IOnBuffInflictedReceiver>();
                foreach (IOnBuffInflictedReceiver receiver in masterReceivers) {
                    receiver.OnBuffInflicted(BuffCatalog.GetBuffDef(index));
                }
            }
        }
    }
}