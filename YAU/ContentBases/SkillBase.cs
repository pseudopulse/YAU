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
using RoR2.Skills;
using EntityStates;
using YAU.Extensions.ROR;

namespace YAU.ContentBases {
    public abstract class SkillBase<T> : SkillBase where T : SkillBase<T>{
        public static T Instance { get; private set; }

        public SkillBase() {
            if (Instance == null) {
                Instance = this as T;
            }
            else {
                YAU.ModLogger.LogError("Attempted to instantiate class " + typeof(T).Name + " when an instance already exists.");
            }
        }
    }
    public abstract class SkillBase {
        public SkillDef skillDef;
        public virtual Type SkillType { get; } = typeof(SkillDef);
        public abstract string Name { get; }
        public abstract string Description { get; }
        public virtual string LangToken => Name.ToUpper().RemoveUnsafeCharacters();
        public abstract float Cooldown { get; }
        public virtual int MaxStock { get; } = 1;
        public virtual int RechargeStock { get; } = 1;
        public virtual int StockToConsume { get; } = 1;
        public virtual string[] Keywords { get; } = null;
        public abstract Sprite Icon { get; }
        public abstract Type ActivationState { get; }
        public abstract string MachineName { get; }
        public virtual bool CanceledFromSprinting { get; } = true;
        public virtual bool CancelSprinting { get; } = true;
        public virtual bool Agile { get; } = false;
        public virtual bool Combat { get; } = true;
        public virtual bool ForceSprint { get; } = false;
        public virtual bool DelayCooldown { get; } = false;
        public virtual UnlockableDef Unlock { get; } = null;
        private SerializableEntityStateType serializableActivationType;

        public void Initialize(YAUContentPack pack, string identifier) {

            serializableActivationType = new(ActivationState);
            pack.RegisterEntityState(ActivationState);

            skillDef = (SkillDef)ScriptableObject.CreateInstance(SkillType);
            skillDef.skillName = Name;
            skillDef.skillNameToken = $"{identifier}_SKILL_{LangToken}_NAME";
            skillDef.skillDescriptionToken = $"{identifier}_SKILL_{LangToken}_DESC";
            LanguageManager.RegisterLanguageToken(skillDef.skillNameToken, Name);
            LanguageManager.RegisterLanguageToken(skillDef.skillDescriptionToken, Description);
            skillDef.keywordTokens = Keywords;
            skillDef.activationState = serializableActivationType;
            skillDef.activationStateMachineName = MachineName;
            skillDef.icon = Icon;
            skillDef.isCombatSkill = Combat;
            skillDef.baseMaxStock = MaxStock;
            skillDef.rechargeStock = RechargeStock;
            skillDef.baseRechargeInterval = Cooldown;
            skillDef.canceledFromSprinting = Agile ? false : CanceledFromSprinting;
            skillDef.cancelSprintingOnActivation = Agile ? false : CancelSprinting;
            skillDef.forceSprintDuringState = ForceSprint;
            skillDef.resetCooldownTimerOnUse = true;
            skillDef.beginSkillCooldownOnSkillEnd = DelayCooldown;

            Setup();

            pack.RegisterScriptableObject(skillDef);
        }

        public virtual void Setup() {

        }

        public void AddToBody(GameObject prefab, SkillSlot slot) {
            SkillLocator locator = prefab.GetComponent<SkillLocator>();
            if (locator) {
                GenericSkill skill = slot switch {
                    SkillSlot.Primary => locator.primary,
                    SkillSlot.Secondary => locator.secondary,
                    SkillSlot.Utility => locator.utility,
                    SkillSlot.Special => locator.special,
                    _ => null
                };

                if (skill && skill.skillFamily) {
                    SkillFamily.Variant variant = new() {
                        skillDef = skillDef,
                        unlockableDef = Unlock,
                        viewableNode = new(skillDef.skillNameToken, false, null)
                    };

                    HG.ArrayUtils.ArrayAppend(ref skill._skillFamily.variants, in variant);
                }
            }
        }

        public bool IsBodyUsing(CharacterBody body) {
            return body.HasSkillEquipped(skillDef);
        }
    }
}