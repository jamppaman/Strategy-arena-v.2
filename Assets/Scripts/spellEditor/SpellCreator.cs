using CharacterSystem;
using GameFieldSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using PlayerSystem;

namespace SpellSystem
{
    [CreateAssetMenu(fileName = "SpellCreator", menuName = "new spell creator")]
    public class SpellCreator : ScriptableObject
    {  // base attributes
        public string spellName;
        public int rangeMin;
        public int rangeMax;
        public int areaOfEffect;
        public int spellApCost;
        public Abilities.SpellAreaType areaType;
        public Abilities.SpellRangeType rangeType;
        public bool needLineOfSight = true;
        public bool needTarget = false;
        public bool needFreeSquare = false;

        // cooldown stuff
        public int spellInitialCooldown;
        public int spellCooldown;
        public int spellCastPerturn;
        public int castPerTarget;

        // counters
        public int spellInitialCooldowncounter;
        public int spellCastPerturncounter;
        public int castPerTargetcounter;


        // dont touch
        public int spellCooldownLeft = 0;
        public int trueDamage;

        // base fluff
        public Sprite spellIcon;
        public AudioClip spellSound;
                [TextArea]
        public string description;


        // references
        public Abilities abilities;
        public GridController gridController;
        public SpellCast spellCast;
        public CharacterValues cv;
        public PlayerBehaviour playerBehaviour;

        [System.Serializable]
        public class SpellAttribute
        {
            public Abilities abilities;
            public StatusEffects sEffects;
            public GridController gridController;
            public SpellCast spellCast;
            public enum AttributeType { damage, steal, heal, effect, push, pull, manipulation };
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
            public EffectValues effect;
            public bool effectOnCaster = false;
            public bool effectOnTarget = true;

            // spell push
            public int spellPushback;
            public Abilities.SpellPushType mySpellPushType;

            // spell push
            public int spellPull;
            public Abilities.SpellPullType mySpellPullType;

            // moving stuff
            public bool teleportToTarget = false;
            public bool chagePlaceWithTarget = false;
            public int moveCloserToTarget;
            public int moveAwayFromTarget;

            // conditions apply later

            // casting
            public void CastAttribute(CharacterValues caster, CharacterValues target, Tile currentMouseTile)
            {
                switch (attributeType)
                {
                    case AttributeType.damage:
                        int damageStuff = 0;
                        if (hurtsAlly == true || hurtsAlly == false && target.team != caster.team)
                        {
                            damageStuff = spellCast.TrueDamageCalculator(spellDamageMax, spellDamageMin, caster.damageChange, target.armorChange, caster.damagePlus, target.armorPlus);
                            spellCast.GetHit(target, damageStuff);
                        }
                        break;
                    case AttributeType.heal:
                        int healingIsFun = 0;
                        if (target.team == caster.team || healsAll == true)
                        {
                            healingIsFun = spellCast.TrueHealCalculator(spellHealMax, spellHealMin, target.healsReceived);
                            spellCast.GetHealed(target, healingIsFun);
                        }

                        break;
                    case AttributeType.steal:
                        int steal = 0;
                        if (stealHurtsAlly == true || stealHurtsAlly == false && target.team != caster.team)
                        {
                            steal = spellCast.TrueDamageCalculator(stealDamageMax, stealDamageMin, caster.damageChange, target.armorChange, caster.damagePlus, target.armorPlus);
                            spellCast.GetHit(target, steal);
                            spellCast.GetHealed(caster, Mathf.RoundToInt(steal / 2));
                        }

                        break;
                    case AttributeType.effect:
                        if (effectOnTarget == true && target != caster)
                        {
                            sEffects.ApplyEffect(caster, effect, target);
                        }
                        if (effectOnCaster == true && target == caster)
                        {
                            sEffects.ApplyEffect(caster, effect, target);
                        }
                        break;
                    case AttributeType.pull:
                        //abilities.SpellPull(mySpellPullType);
                        break;
                    case AttributeType.push:
                        //abilities.SpellPush(mySpellPushType);
                        break;
                    case AttributeType.manipulation:
                        Tile casterTile = gridController.GetTile(caster.currentTile.x, caster.currentTile.z);
                        Tile targetTile = currentMouseTile;
                        if (moveCloserToTarget != 0)
                        {
                           // abilities.WalkTowardsTarget();
                        }
                        if (moveAwayFromTarget != 0)
                        {
                           // abilities.MoveAwayFromTarget();
                        }

                        if (teleportToTarget == true)
                        {
                            abilities.CasterTeleport(casterTile);
                        }
                        if (chagePlaceWithTarget == true)
                        {
                            abilities.TeleportSwitch(casterTile, targetTile);
                        }
                        break;
                }
            }
        }
        public List<SpellAttribute> spellArvot = new List<SpellAttribute>(1);

        public void CastSpell(CharacterValues caster, Tile mousetile)
        {
            //playerBehaviour.aControll.PlayAttack(caster);
            //playerBehaviour.aControll.PlaySpell(spellSound);
            List<CharacterValues> maalit = spellCast.TargetChecker();

            foreach (CharacterValues target in maalit)
            {
                foreach (SpellAttribute item in spellArvot)
                {
                    item.CastAttribute(caster, target, mousetile);
                }
            }
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