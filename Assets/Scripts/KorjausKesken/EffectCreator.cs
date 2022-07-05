using CharacterSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellSystem
{
    [CreateAssetMenu(fileName = "Effect", menuName = "new effect")]
    public class EffectCreator : ScriptableObject
    { 
        // base attributes
        public string effectName;
        public int effectDuration = 0;
        public bool isAtTurnStart;

        //rules
        public bool canBeSilenced = true; //<
        public bool stacks = true;
        public enum Targeting {caster, target, both };
        public Targeting targeting;
        public enum FriendlyFire {onlyEnemies, onlyFriends, noLimiter};
        public FriendlyFire friendlyFire;

        public List<EffectType> effectArvot = new List<EffectType>(1);

        // base fluff
        public Sprite effectIcon;
        [TextArea]
        public string description;

        // Calculators
        public int remainingTurns = 0;
        public bool isCastThisTurn = true;
        public CharacterValues owner; 


[System.Serializable]
        public class EffectType
        {
            public enum Effect {damageOverTime, healOverTime, damageBuffPlus, damageBuffPercent, immunity, armorBuffPlus, armorBuffPercent, heavy, healModifier, apModify, mpModify};
            public Effect effect;

            // damage stuff
            public int damageOverTime;
            public int damageModifyPlus;
            public float damageModifyPercent = 0;

            // armor stuff
            public int armorModifyPlus;
            public float armorModifyPercent = 0;

            // heavy
            public bool heavyState = false;

            // immunity
            public bool immune = false;

            // Heal stuff
            public float healModify = 0;
            public int healOverTime;

            // AP/MP modify
            public int apModify;
            public int mpModify;
        }

        // Don't touch ---------------------------------
        void AddNew()
        {
            effectArvot.Add(new EffectType());
        }

        void Remove(int index)
        {
            effectArvot.RemoveAt(index);
        }
        //----------------------------------------------

    }
}
