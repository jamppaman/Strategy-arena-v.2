using CharacterSystem;
using GameFieldSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellSystem
{
    [CreateAssetMenu(fileName = "Spell", menuName = "new spell")]
    public class SpellCreator : ScriptableObject
    {  // base attributes
        public string spellName;
        public int rangeMin;
        public int rangeMax;
        public int areaOfEffect;
        public int spellApCost;
        public SpellHandler.SpellAreaType areaType;
        public SpellHandler.SpellRangeType rangeType;
        public bool needLineOfSight = true;
        public bool needTarget = false;
        public bool needFreeSquare = false;

        // cooldown stuff
        public int spellInitialCooldown = 0;
        public int spellCooldown = 0;
        public int spellCastPerturn = 0;
        public int castPerTarget = 0;

        // counters
        public int spellInitialCooldowncounter = 0;
        public int spellCastPerturncounter = 0;
        public int castPerTargetcounter = 0;


        // dont touch
        public int spellCooldownLeft = 0;
        public int trueDamage;
        public List<SpellAttribute> spellArvot = new List<SpellAttribute>(1);

        // base fluff
        public Sprite spellIcon;
        public AudioClip spellSound;
                [TextArea]
        public string description;


        // references
        GridController gridController;
        CharacterValues cv;
        PlayerBehaviour playerBehaviour;
        //////////////SpellCastChecks castChecks;


        [System.Serializable]
        public class SpellAttribute
        {
            // References
            ////////////////public StatusEffects sEffects;
            public GridController gridController;
            public SpellCalculators spellCalculators;
            public SpellFormulaes spellFormulaes;

            public bool isSingleUse = false;
            public enum AttributeType { damage, steal, heal, effect, pullpush, walk, teleport, silence};
            public AttributeType attributeType;

            // damage stuff
            public int spellDamageMin;
            public int spellDamageMax;
            public bool hurtsAlly = false;

            // heal stuff
            public int spellHealMin;
            public int spellHealMax;
            public bool healsAll = false;
            // steal stuff
            public int stealDamageMin;
            public int stealDamageMax;
            public bool stealHurtsAlly = false;

            // effect stuff
            public EffectCreator effect;
            public bool effectOnCaster = false;
            public bool effectOnTarget = false;

            // spell push
            public int spellPullPush;
            public SpellFormulaes.SpellPullPushType mySpellPullPushType;
            // V V V when true spell pulls
            public bool isItPull = true;
            public int wallSplatDmg;

            // teleport
            public bool switchWithTarget = false;

            // walking
            public int movemenPoints;
            public bool moveTowards;

            // immunity
            public int silenceAmount;

            // Add new variables here
            //
        }

        // Don't touch ---------------------------------
        void AddNew()
        {
            spellArvot.Add(new SpellAttribute());
        }

        void Remove(int index)
        {
            spellArvot.RemoveAt(index);
        }
        //----------------------------------------------

    }
}