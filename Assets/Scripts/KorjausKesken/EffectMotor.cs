using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterSystem;
using ManagementSystem;

namespace SpellSystem
{
    public class EffectMotor : MonoBehaviour
    {
        EffectCreator effectCreator;
        CharacterValues characterValues;
        TeamManager tManager;
        PlayerBehaviour pBehaviour;

        public List<EffectCreator> effectList = new List<EffectCreator>();

        void Start()
        {
            if (!tManager)
                tManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<TeamManager>();
            if (!pBehaviour)
                pBehaviour = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerBehaviour>();
            effectList.Clear();
            //////////ClearEffectValues();
        }

        public void CreateEffect(CharacterValues caster, EffectCreator effect, CharacterValues target)
        {
            if (effect.targeting == EffectCreator.Targeting.caster || effect.targeting == EffectCreator.Targeting.both)
            {
                EffectApply(effect, caster);
            }
            if (effect.targeting == EffectCreator.Targeting.target || effect.targeting == EffectCreator.Targeting.both)
            {
                EffectApply(effect, target);
            }

        }

        public void EffectApply(EffectCreator effect, CharacterValues target)
        {
            bool inUse = false;
            EffectCreator clone = Object.Instantiate(effect) as EffectCreator;
            clone.owner = target;
            clone.isCastThisTurn = true;
            clone.remainingTurns = clone.effectDuration;
            foreach (EffectCreator eff in effectList)
            {
                if (eff.owner == clone.owner)
                {
                    if (eff.effectName == clone.effectName)
                    {
                        inUse = true;
                        if (clone.stacks == false)
                        {
                            eff.isCastThisTurn = true;
                            eff.remainingTurns = clone.effectDuration;      //New spells are always better!
                        }
                        else
                        {
                            effectList.Add(clone);
                            pBehaviour.AddTabEffect(clone, target);
                        }
                    }
                    else
                    {
                        effectList.Add(clone);
                        pBehaviour.AddTabEffect(clone, target);
                    }
                }
            }
            if (!inUse)
            {
                effectList.Add(clone);
                EffectAdd(clone, target);
                pBehaviour.AddTabEffect(clone, target);
            }



        }

        public void UpdateEffect(CharacterValues current, bool inTurnStart)
        {
            foreach (EffectCreator eff in effectList)
            {
                if (eff.owner == current && eff.isAtTurnStart == inTurnStart)
                {
                    eff.remainingTurns--;
                    if (eff.isCastThisTurn != true)
                    {
                        CheckEffects(eff, current);
                    }


                }
            }

        }

        public void CheckEffects(EffectCreator effect, CharacterValues owner)
        {
            if (effect.remainingTurns >= 0)
            {
                EffectReset(effect, owner);
            }
        }

        public void CheckExceptions(EffectCreator effect, CharacterValues owner)
        {
            foreach (var huugo in effect.effectArvot)
            {
                switch (huugo.effect)
                {
                    case EffectCreator.EffectType.Effect.armorBuffPercent:
                        break;
                    case EffectCreator.EffectType.Effect.damageBuffPercent:
                        break;
                    case EffectCreator.EffectType.Effect.damageBuffPlus:
                        break;
                    case EffectCreator.EffectType.Effect.healModifier:
                        break;
                    case EffectCreator.EffectType.Effect.damageOverTime:
                        //
                        break;
                    case EffectCreator.EffectType.Effect.healOverTime:
                        //
                        break;
                    case EffectCreator.EffectType.Effect.mpModify:
                        //
                        break;
                    case EffectCreator.EffectType.Effect.apModify:
                        //
                        break;
                    case EffectCreator.EffectType.Effect.heavy:
                        owner.heavy = true;
                        break;
                    case EffectCreator.EffectType.Effect.immunity:

                        break;
                    default:

                        break;
                }
            }
        }

        public void EffectAdd(EffectCreator effect, CharacterValues target)
        {
            foreach (var item in effect.effectArvot)
            {
                switch (item.effect)
                {
                    case EffectCreator.EffectType.Effect.armorBuffPercent:
                        break;
                    case EffectCreator.EffectType.Effect.armorBuffPlus:
                        break;
                    case EffectCreator.EffectType.Effect.damageBuffPercent:
                        break;
                    case EffectCreator.EffectType.Effect.damageBuffPlus:
                        break;
                    case EffectCreator.EffectType.Effect.healModifier:
                        break;
                    case EffectCreator.EffectType.Effect.damageOverTime:
                        //
                        break;
                    case EffectCreator.EffectType.Effect.healOverTime:
                        //
                        break;
                    case EffectCreator.EffectType.Effect.mpModify:
                        //
                        break;
                    case EffectCreator.EffectType.Effect.apModify:
                        //
                        break;
                    case EffectCreator.EffectType.Effect.heavy:
                        target.heavy = true;
                        break;
                    case EffectCreator.EffectType.Effect.immunity:

                        break;
                    default:

                        break;
                }
            }
        }

        public void EffectReset(EffectCreator effect, CharacterValues target)
        {
            foreach (var item in effect.effectArvot)
            {
                switch (item.effect)
                {
                    case EffectCreator.EffectType.Effect.armorBuffPercent:
                        target.armorChange -= item.armorModifyPercent;
                        break;
                    case EffectCreator.EffectType.Effect.armorBuffPlus:
                        target.armorPlus -= item.armorModifyPlus;
                        if (target.armorPlus < 0)
                        {
                            target.armorPlus = 0;
                        }
                        break;
                    case EffectCreator.EffectType.Effect.damageBuffPercent:
                        target.damageChange -= item.damageModifyPercent;
                        break;
                    case EffectCreator.EffectType.Effect.damageBuffPlus:
                        target.damagePlus -= item.damageModifyPlus;
                        break;
                    case EffectCreator.EffectType.Effect.healModifier:
                        target.healsReceived -= item.healModify;
                        break;
                    case EffectCreator.EffectType.Effect.damageOverTime:
                        //
                        break;
                    case EffectCreator.EffectType.Effect.healOverTime:
                        //
                        break;
                    case EffectCreator.EffectType.Effect.mpModify:
                        //
                        break;
                    case EffectCreator.EffectType.Effect.apModify:
                        //
                        break;
                    case EffectCreator.EffectType.Effect.heavy:
                        target.heavy = false;
                        break;
                    case EffectCreator.EffectType.Effect.immunity:

                        break;
                    default:

                        break;
                }
            }


        }

        // cleans character effect variables
        public void EffectHardReset( CharacterValues target)
        {

        }


    }
}

