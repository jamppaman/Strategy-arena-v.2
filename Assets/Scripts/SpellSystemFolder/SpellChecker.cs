using CharacterSystem;
using GameFieldSystem;
using ManagementSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellSystem
{
    public class SpellChecker : MonoBehaviour
    {
        PlayerBehaviour playerBehaviour;
        SpellHandler spellHandler;

        private void Start()
        {
            playerBehaviour = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerBehaviour>();
            spellHandler = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<SpellHandler>();
        }
        // Checks spell Cast rules and are they fullfilled
        public bool CheckCastability(SpellCreator spell, Tile target)
        {

            if (TargetInRangeCheck(spell, target) == false)
            {
                return false;
            }
            if (playerBehaviour.currentCharacter.currentAp < spell.spellApCost)
            {
                return false;
            }
            if (spell.needTarget == true && target.CharCurrentlyOnTile == false)
            {
                return false;
            }
            if (spell.needFreeSquare == true && target.CharCurrentlyOnTile == true)
            {
                return false;
            }
            if (SpellCooldownCheck(spell) == false)
            {
                return false;
            }
            return true;
        }

        public bool TargetInRangeCheck(SpellCreator spell, Tile target)
        {
            //// ---------------------- nämä conflictas, muut oli ok -------------------------
            List<Tile> range = spellHandler.RangeType(spell.rangeType, false);
            bool correct = false;
            foreach (var temp in range)
            {
                if (temp == target)
                {
                    correct = true;
                }
            }
            if (correct == false)
            {
                return false;
            }
            //// --------------------- Pasta carbonara on hyvää vai mitä ---------------------
            return true;
        }

        // V V V Checks if spell is in cooldown
        public bool SpellCooldownCheck(SpellSystem.SpellCreator spell)
        {
            if (spell.spellInitialCooldowncounter > 0)
            {
                return false;
            }
            if (spell.spellCastPerturncounter >= spell.spellCastPerturn && spell.spellCastPerturn != 0)
            {
                return false;
            }
            if (spell.spellCooldownLeft > 0)
            {
                return false;
            }

            return true;
        }

        public bool CastNoTargetNeeded(SpellCreator spell, SpellCreator.SpellAttribute attribute)
        {
            if (attribute.attributeType == SpellCreator.SpellAttribute.AttributeType.walk ||  attribute.attributeType == SpellCreator.SpellAttribute.AttributeType.teleport && attribute.switchWithTarget == false)
            {
                return true;
            }
            else
            {
                return false;
            }

        }


        public List<CharacterValues> TargetChecker(SpellCreator currentSpell)
        {
            List<Tile> targetTileList = spellHandler.AreaType(currentSpell.areaType);
            List<CharacterValues> targetList = new List<CharacterValues>();
            foreach (var item in targetTileList)
            {
                PlayerInfo checker = item.CharCurrentlyOnTile;
                CharacterValues target = null;
                if (checker)
                {
                    target = checker.thisCharacter;
                    targetList.Add(target);
                }
            }
            return targetList;
        }
    }
}
