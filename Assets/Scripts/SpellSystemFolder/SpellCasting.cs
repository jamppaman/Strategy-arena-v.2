using CharacterSystem;
using GameFieldSystem;
using ManagementSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellSystem
{
    public class SpellCasting : MonoBehaviour
    {
        public PlayerBehaviour playerBehaviour;
        public GridController gridController;
        public SpellButtons spellButtons;
        public SpellHandler spellHandler;
        public MouseController mc;
        public SpellCalculators spellCalculators;
        public SpellFormulaes spellFormulaes;
        public SpellChecker spellChecker;

        public SpellFormulaes.ManipulationType manipulationType;

        void Start()
        {
            spellButtons = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<SpellButtons>();
            spellButtons = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<SpellButtons>();
            spellCalculators = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<SpellCalculators>();
            spellFormulaes = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<SpellFormulaes>();
            spellChecker = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<SpellChecker>();
            mc = GameObject.FindGameObjectWithTag("MouseManager").GetComponent<MouseController>();
        }

        // Casting the spell
        public void CastSpell(SpellCreator spell, CharacterValues caster, Tile currentMouseTile)
        {
            
            playerBehaviour.aControll.PlayAttack(caster);
            playerBehaviour.aControll.PlaySpell(spell);
            Tile casterTile = gridController.GetTile(caster.currentTile.x, caster.currentTile.z);
            Tile targetTile = currentMouseTile;
            //Tile temp1 = gridController.GetTile(caster.currentTile.x,caster.currentTile.z);
            //Tile temp2 = gridController.GetTile(target.currentTile.x,target.currentTile.z);

            // V V V gets a list of tiles in AOE
            List<Tile> targetsList = spellHandler.AreaType(spell.areaType);

            // V V V goes through every attribute in the spells attribute list
            foreach (var temp in spell.spellArvot)
            {

                // V V V i need to make stuff here for attributes that dont need target and are single use
                if (temp.isSingleUse == true)
                {
                    TargetChecker(spell, temp, caster, currentMouseTile, currentMouseTile);
                }
                else
                {
                    foreach (var item in targetsList)
                    {
                        TargetChecker(spell, temp, caster, currentMouseTile, item);
                    }
                }
            }
            // V V V update UI and cooldowns
            spellCalculators.HandleCoolDownIncrease(spell);
            spellButtons.UpdateHpApMp();
        }

        public void TargetChecker(SpellCreator spell, SpellCreator.SpellAttribute spellAttribute, CharacterValues caster, Tile currentMouseTile, Tile item)
        {
            bool iNeedThis = spellChecker.CastNoTargetNeeded(spell, spellAttribute);
                // V V V checks if tile has a player
                PlayerInfo checker = item.CharCurrentlyOnTile;
                CharacterValues target = null;
                if (checker)
                    target = checker.thisCharacter;

                // V V V cast the attribute if player is on tile
                if (target || iNeedThis == true)
                {
                    CastAttribute(spellAttribute, caster, target, currentMouseTile);
                }
        }




        // Attribute casting
        public void CastAttribute(SpellCreator.SpellAttribute spellAttribute, CharacterValues caster, CharacterValues target, Tile currentMouseTile)
        {
            // V V V Gets targets and Casters 

            Tile casterTile = gridController.GetTile(caster.currentTile.x, caster.currentTile.z);
            Tile targetTile = null;
            if (target == null)
            {
                targetTile = currentMouseTile;
            }
            else
            {
                targetTile = gridController.GetTile(target.currentTile.x, target.currentTile.z);
            }

            // V V V Checks the type of attribute and acts accordinly
            switch (spellAttribute.attributeType)
            {
                // V V V Damage attribute
                case SpellCreator.SpellAttribute.AttributeType.damage:
                    int damageStuff = 0;
                    if (spellAttribute.hurtsAlly == true || spellAttribute.hurtsAlly == false && target.team != caster.team)
                    {
                        damageStuff = spellCalculators.TrueDamageCalculator(spellAttribute.spellDamageMax, spellAttribute.spellDamageMin, caster.damageChange, target.armorChange, caster.damagePlus, target.armorPlus);
                        spellCalculators.GetHit(target, damageStuff);
                    }
                    break;
                // V V V Heal attribute
                case SpellCreator.SpellAttribute.AttributeType.heal:
                    int healingIsFun = 0;
                    if (target.team == caster.team || spellAttribute.healsAll == true)
                    {
                        if (spellCalculators == null)
                            Debug.Log("calulator is shit");
                        healingIsFun = spellCalculators.TrueHealCalculator(spellAttribute.spellHealMax, spellAttribute.spellHealMin, target.healsReceived);
                        spellCalculators.GetHealed(target, healingIsFun);
                    }
                    break;
                // V V V Steal attribute
                case SpellCreator.SpellAttribute.AttributeType.steal:
                    int steal = 0;
                    if (spellAttribute.stealHurtsAlly == true || spellAttribute.stealHurtsAlly == false && target.team != caster.team)
                    {
                        steal = spellCalculators.TrueDamageCalculator(spellAttribute.stealDamageMax, spellAttribute.stealDamageMin, caster.damageChange, target.armorChange, caster.damagePlus, target.armorPlus);
                        spellCalculators.GetHit(target, steal);
                        spellCalculators.GetHealed(caster, Mathf.RoundToInt(steal / 2));
                    }
                    break;

                // V V V Effect attribute
                case SpellCreator.SpellAttribute.AttributeType.effect:
                    if (spellAttribute.effectOnTarget == true && target != caster)
                    {
                        ////////sEffects.ApplyEffect(caster, effect, target);
                    }
                    if (spellAttribute.effectOnCaster == true && target == caster)
                    {
                        //////////sEffects.ApplyEffect(caster, effect, target);
                    }
                    break;

                // V V V  Pull or Push attribute
                case SpellCreator.SpellAttribute.AttributeType.pullpush:
                    spellFormulaes.SpellPullPush(spellAttribute, casterTile, targetTile, currentMouseTile);
                    break;

                // V V V Other Manipulation attribute
                case SpellCreator.SpellAttribute.AttributeType.walk:
                    spellFormulaes.SpellWalkToAway( currentMouseTile, casterTile, spellAttribute);
                    break;

                case SpellCreator.SpellAttribute.AttributeType.teleport:
                    if (spellAttribute.switchWithTarget == true)
                    {
                        Debug.Log("fuck the what");
                        spellFormulaes.TeleportSwitch(casterTile, targetTile);
                    }
                    if (spellAttribute.switchWithTarget == false)
                    {
                        Debug.Log("what the fuck");
                        spellFormulaes.CasterTeleport(casterTile, currentMouseTile);
                    }

                    break;
            }
        }
        

        // V V V Resetting after the spell use
        public void Aftermath()
        {
            // V V V reduses AP
            playerBehaviour.currentCharacter.currentAp -= spellButtons.currentSpell.spellApCost;

            // V V V resetting lists
            if (mc.rangeTiles != null)
            {
                mc.ResetTileMaterials(mc.rangeTiles);
                mc.rangeTiles = null;
            }
            if (mc.nullTiles != null)
            {
                mc.ResetTileMaterials(mc.nullTiles);
                mc.nullTiles = null;
            }
            if (mc.targetedTiles != null)
            {
                mc.ResetTileMaterials(mc.targetedTiles);
                mc.targetedTiles = null;
            }
            spellButtons.currentSpell = null;
            spellButtons.spellOpen = false;
            spellButtons.UpdateHpApMp();
            playerBehaviour.UpdateTabs();
        }
    }
}
