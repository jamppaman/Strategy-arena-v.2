using CharacterSystem;
using SpellSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIsystem
{
    public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        PlayerBehaviour pb;

        public SpellCreator spell;
        public CharacterValues character;
        public EffectValues effect;
        UImanager ui;
        Text _text;

        void Start()
        {
            if (!ui)
            {
                ui = GameObject.FindGameObjectWithTag("UIcanvas").GetComponent<UImanager>();
            }
            if (!pb)
            {
                pb = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerBehaviour>();
            }
        }
        void Update()
        {
            if (effect)
            {
                if (effect.remainingTurns <= 0)
                {
                    gameObject.SetActive(false);
                }
            }
        }

        public void UpdateInfo(SpellCreator x)
        {
            spell = x;
        }
        public void UpdateInfo(CharacterValues x)
        {
            character = x;
        }
        public void UpdateInfo(EffectValues x)
        {
            effect = x;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //asettaa tooltipin hiiren eri puolille v
            if (Input.mousePosition.y >= 350f)
            {
                if (Input.mousePosition.x >= 800f)
                {
                    ui.tooltip.GetComponent<RectTransform>().anchoredPosition = new Vector2(1, 1);
                    ui.tooltip.GetComponent<RectTransform>().pivot = new Vector2(1, 1);
                    ui.tooltip.GetComponent<RectTransform>().position = new Vector2(1, 1);
                    ui.tooltip.transform.position = Input.mousePosition + new Vector3(-10f, 0f, 10f);
                }
                else
                {
                    ui.tooltip.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 1);
                    ui.tooltip.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
                    ui.tooltip.GetComponent<RectTransform>().position = new Vector2(0, 1);
                    ui.tooltip.transform.position = Input.mousePosition + new Vector3(10f, 0f, 10f);
                }
            }
            else
            {
                if (Input.mousePosition.x >= 800f)
                {
                    ui.tooltip.GetComponent<RectTransform>().anchoredPosition = new Vector2(1, 0);
                    ui.tooltip.GetComponent<RectTransform>().pivot = new Vector2(1, 0);
                    ui.tooltip.GetComponent<RectTransform>().position = new Vector2(1, 0);
                    ui.tooltip.transform.position = Input.mousePosition + new Vector3(-10f, 0f, 10f);
                }
                else
                {
                    ui.tooltip.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                    ui.tooltip.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
                    ui.tooltip.GetComponent<RectTransform>().position = new Vector2(0, 0);
                    ui.tooltip.transform.position = Input.mousePosition + new Vector3(10f, 0f, 10f);
                }
            }
            //^
            //SPELL
            if (spell && !character && !effect)
            {
                ui._text = "";
                ui._text += spell.spellName + "\n";
                ui._text += "\nAP: " + spell.spellApCost;
                if (spell.rangeType != Abilities.SpellRangeType.LinDiag)
                {
                    ui._text += "\nRange: " + spell.rangeType + " : " + spell.rangeMin;
                }
                if (spell.rangeType == Abilities.SpellRangeType.LinDiag)
                {
                    ui._text += "\nRange: " + "Linear & Diagonal" + " : " + spell.rangeMin;
                }
                if (spell.rangeMin != spell.rangeMax)
                {
                    ui._text += " - " + spell.rangeMax;
                }
                if (spell.areaOfEffect != 0)
                {
                    ui._text += "\nArea of effect: " + spell.areaType + " : " + spell.areaOfEffect;
                    ui._text += "\n";
                }
                if (spell.areaOfEffect == 0)
                {
                    ui._text += "\nSingle target";
                    ui._text += "\n";
                }
                //if (spell.spellDamageMin != 0)
                //{
                //    if (spell.damageStealsHp == false)
                //    {
                //        ui._text += "\n" + "Damage: " + spell.spellDamageMin + " - " + spell.spellDamageMax;
                //    }
                //    else
                //    {
                //        ui._text += "\n" + "Damage (Steal): " + spell.spellDamageMin + " - " + spell.spellDamageMax;
                //    }
                //}
                //if (spell.spellHealMin != 0)
                //{
                //    ui._text += "\n" + "Heal: " + spell.spellHealMin + " - " + spell.spellHealMax;
                //}

                if (spell.needLineOfSight == false)
                {
                    ui._text += "\nNeeds line of sight: ";
                    ui._text += "No";
                }

                if (spell.needTarget == true)
                {
                    ui._text += "\nNeeds a target: ";
                    ui._text += "Yes";
                }
                if (spell.needFreeSquare == true)
                {
                    ui._text += "\nNeeds a free square: ";
                    ui._text += "Yes";
                }


                //SPELLS EFFECT
                //if (spell.effect != null)
                //{
                //    //Effect info
                //    if (spell.effect.immune == true)
                //    {
                //        ui._text += "\nImmune to damage:" + " (" + spell.effect.effectDuration + " turns)";
                //    }
                //    if (spell.effect.heavyState == true)
                //    {
                //        ui._text += "\nHeavy state:" + " (" + spell.effect.effectDuration + " turns)";
                //    }
                //    if (spell.effect.damageModifyPlus != 0)
                //    {
                //        if (spell.effect.damageModifyPlus > 0)
                //        {
                //            ui._text += "\nDamage: +" + spell.effect.damageModifyPlus + " (" + spell.effect.effectDuration + " turns)";
                //        }
                //        else
                //        {
                //            ui._text += "\nDamage: " + spell.effect.damageModifyPlus + " (" + spell.effect.effectDuration + " turns)";
                //        }
                //    }
                //    if (spell.effect.damageModifyPercent != 0)
                //    {
                //        if (spell.effect.damageModifyPercent > 0)
                //        {
                //            ui._text += "\nDamage: +" + spell.effect.damageModifyPercent * 100 + "%" + " (" + spell.effect.effectDuration + " turns)";
                //        }
                //        else
                //        {
                //            ui._text += "\nDamage: " + spell.effect.damageModifyPercent * 100 + "%" + " (" + spell.effect.effectDuration + " turns)";
                //        }
                //    }
                //    if (spell.effect.armorModifyPlus != 0)
                //    {
                //        if (spell.effect.armorModifyPlus > 0)
                //        {
                //            ui._text += "\nArmor: +" + spell.effect.armorModifyPlus + " (" + spell.effect.effectDuration + " turns)";
                //        }
                //        else
                //        {
                //            ui._text += "\nArmor: " + spell.effect.armorModifyPlus + " (" + spell.effect.effectDuration + " turns)";
                //        }
                //    }
                //    if (spell.effect.armorModifyPercent != 0)
                //    {
                //        if (spell.effect.armorModifyPercent > 0)
                //        {
                //            ui._text += "\nArmor: +" + spell.effect.armorModifyPercent * 100 + "%" + " (" + spell.effect.effectDuration + " turns)";
                //        }
                //        else
                //        {
                //            ui._text += "\nArmor: " + spell.effect.armorModifyPercent * 100 + "%" + " (" + spell.effect.effectDuration + " turns)";
                //        }
                //    }
                //    if (spell.effect.healModify != 0)
                //    {
                //        if (spell.effect.healModify > 0)
                //        {
                //            ui._text += "\nHeal modifier: +" + spell.effect.healModify * 100 + "%" + " (" + spell.effect.effectDuration + " turns)";
                //        }
                //        else
                //        {
                //            ui._text += "\nHeal modifier: " + spell.effect.healModify * 100 + "%" + " (" + spell.effect.effectDuration + " turns)";
                //        }
                //    }
                //    if (spell.effect.apModify != 0)
                //    {
                //        if (spell.effect.apModify > 0)
                //        {
                //            ui._text += "\nAP: +" + spell.effect.apModify + " (" + spell.effect.effectDuration + " turns)";
                //        }
                //        else
                //        {
                //            ui._text += "\nAP: " + spell.effect.apModify + " (" + spell.effect.effectDuration + " turns)";
                //        }
                //    }
                //    if (spell.effect.mpModify != 0)
                //    {
                //        if (spell.effect.mpModify > 0)
                //        {
                //            ui._text += "\nMP: +" + spell.effect.mpModify + " (" + spell.effect.effectDuration + " turns)";
                //        }
                //        else
                //        {
                //            ui._text += "\nMP: " + spell.effect.mpModify + " (" + spell.effect.effectDuration + " turns)";
                //        }
                //    }

                //}



                //if (spell.spellPushback != 0)
                //{
                //    ui._text += "\nPushback: " + spell.spellPushback;
                //}
                //if (spell.spellPull != 0)
                //{
                //    ui._text += "\nPull: " + spell.spellPull;
                //}
                //if (spell.moveCloserToTarget != 0)
                //{
                //    ui._text += "\nMove closer to target: " + spell.moveCloserToTarget;
                //}
                //if (spell.moveAwayFromTarget != 0)
                //{
                //    ui._text += "\nMove away from target: " + spell.moveAwayFromTarget;
                //}
                if (spell.spellInitialCooldown != 0 || spell.spellCooldown != 0 || spell.spellCastPerturn != 0 || spell.castPerTarget != 0)
                {
                    ui._text += "\n";
                }
                if (spell.spellInitialCooldown != 0)
                {
                    ui._text += "\nInitial cooldown: " + spell.spellInitialCooldown;
                }
                if (spell.spellCooldown != 0)
                {
                    ui._text += "\nCooldown: " + spell.spellCooldown;
                }
                if (spell.spellCastPerturn != 0)
                {
                    ui._text += "\nCast per turn: " + spell.spellCastPerturn;
                }
                if (spell.castPerTarget != 0)
                {
                    ui._text += "\nCast per target: " + spell.castPerTarget;
                }
                ui._text += "\n";
                ui._text += "\n" + spell.description;
                if (spell.spellCooldownLeft != 0)
                {
                    ui._text += "\n";
                    ui._text += "\nCOOLDOWN LEFT: " + spell.spellCooldownLeft + " TURNS!";
                }

                ui.ShowTooltip();
            }
            //CHARACTER
            else if (!spell && character && !effect)
            {
                ui._text = "";
                ui._text += character.characterName;
                ui._text += "\n";
                ui._text += "\nHP: " + character.currentHP + " / " + character.currentMaxHP;
                ui._text += "\nAP: " + character.currentAp;
                ui._text += "\nMP: " + character.currentMp;
                ui.ShowTooltip();
            }
            //EFFECT
            else if (!spell && !character && effect)
            {
                ui._text = "";
                ui._text += effect.effectName;
                ui._text += "\n";
                ui._text += "\nTurns remaining: " + effect.remainingTurns;
                ui._text += "\n";
                if (effect.immune == true)
                {
                    ui._text += "\nImmune to damage";
                }
                if (effect.heavyState == true)
                {
                    ui._text += "\nHeavy state";
                }
                if (effect.damageModifyPlus != 0)
                {
                    if (effect.damageModifyPlus > 0)
                    {
                        ui._text += "\nDamage: +" + effect.damageModifyPlus;
                    }
                    else
                    {
                        ui._text += "\nDamage: " + effect.damageModifyPlus;
                    }
                }
                if (effect.damageModifyPercent != 0)
                {
                    if (effect.damageModifyPercent > 0)
                    {
                        ui._text += "\nDamage: +" + effect.damageModifyPercent * 100 + "%";
                    }
                    else
                    {
                        ui._text += "\nDamage: " + effect.damageModifyPercent * 100 + "%";
                    }
                }
                if (effect.armorModifyPlus != 0)
                {
                    if (effect.armorModifyPlus > 0)
                    {
                        ui._text += "\nArmor: +" + effect.armorModifyPlus;
                    }
                    else
                    {
                        ui._text += "\nArmor: " + effect.armorModifyPlus;
                    }
                }
                if (effect.armorModifyPercent != 0)
                {
                    if (effect.armorModifyPercent > 0)
                    {
                        ui._text += "\nArmor: +" + effect.armorModifyPercent * 100 + "%";
                    }
                    else
                    {
                        ui._text += "\nArmor: " + effect.armorModifyPercent * 100 + "%";
                    }
                }
                if (effect.healModify != 0)
                {
                    if (effect.healModify > 0)
                    {
                        ui._text += "\nHeal modifier: +" + effect.healModify * 100 + "%";
                    }
                    else
                    {
                        ui._text += "\nHeal modifier: " + effect.healModify * 100 + "%";
                    }
                }
                if (effect.apModify != 0)
                {
                    if (effect.apModify > 0)
                    {
                        ui._text += "\nAP: +" + effect.apModify;
                    }
                    else
                    {
                        ui._text += "\nAP: " + effect.apModify;
                    }
                }
                if (effect.mpModify != 0)
                {
                    if (effect.mpModify > 0)
                    {
                        ui._text += "\nMP: +" + effect.mpModify;
                    }
                    else
                    {
                        ui._text += "\nMP: " + effect.mpModify;
                    }
                }



                ui.ShowTooltip();
            }
            else
            {
                Debug.Log("error");
            }
            ui.ShowTooltip();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ui.HideTooltip();
        }
    }

}