using CharacterSystem;
using GameFieldSystem;
using ManagementSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellSystem
{

    public class SpellHandler : MonoBehaviour
    {
        public enum SpellAreaType { Cross, Line, Normal, Square, Cone, Diagonal }; // Different types of AoE
        public enum SpellRangeType { Linear, Diagonal, Normal, LinDiag }; // How Player Targets the spell

        //referenses
        MouseController mouseController;
        PlayerBehaviour playerBehaviour;
        SpellButtons spellButtons;
        GridController gridController;
        LineOfSight lOS;


        void Start()
        {
            // Applying references references
            mouseController = GameObject.FindGameObjectWithTag("MouseManager").GetComponent<MouseController>();
            spellButtons = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<SpellButtons>();
            gridController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridController>();
            lOS = GetComponent<LineOfSight>();
            playerBehaviour = gameObject.GetComponentInParent<PlayerBehaviour>();

        }

        // V V V Makes a list of tiles of AOE
        public List<Tile> AreaType(SpellAreaType mySpellAreaType)
        {
            int[][] dirList = new int[4][] { new int[] { 0, 1 }, new int[] { 0, -1 }, new int[] { 1, 0 }, new int[] { -1, 0 } };
            List<Tile> targetTiles = new List<Tile>();
            if (mouseController.selected == null)
            {
                return targetTiles;
            }
            switch (mySpellAreaType)
            {
                //type linessa spellrange min ei saa olla 0
                case SpellAreaType.Line:
                    if (playerBehaviour.currentCharacter.currentTile.z < mouseController.selected.locZ)
                        LineTargets(0, dirList, out targetTiles);
                    else if (playerBehaviour.currentCharacter.currentTile.z > mouseController.selected.locZ)
                        LineTargets(1, dirList, out targetTiles);
                    else if (playerBehaviour.currentCharacter.currentTile.x > mouseController.selected.locX)
                        LineTargets(3, dirList, out targetTiles);
                    else
                        LineTargets(2, dirList, out targetTiles);
                    break;
                case SpellAreaType.Cross:

                    targetTiles.Add(mouseController.selected);
                    for (int i = 1; i <= spellButtons.currentSpell.areaOfEffect; i++)
                    {
                        foreach (var c in dirList)
                        {
                            targetTiles.Add(gridController.GetTile(mouseController.selected.locX + c[0] * i, mouseController.selected.locZ + c[1] * i));
                        }
                    }
                    break;
                case SpellAreaType.Diagonal:
                    targetTiles.Add(mouseController.selected);
                    for (int i = 1; i <= spellButtons.currentSpell.areaOfEffect; i++)
                    {
                        targetTiles.Add(gridController.GetTile(mouseController.selected.locX + i, mouseController.selected.locZ + i));
                        targetTiles.Add(gridController.GetTile(mouseController.selected.locX + i, mouseController.selected.locZ - i));
                        targetTiles.Add(gridController.GetTile(mouseController.selected.locX - i, mouseController.selected.locZ + i));
                        targetTiles.Add(gridController.GetTile(mouseController.selected.locX - i, mouseController.selected.locZ - i));
                    }
                    break;
                case SpellAreaType.Normal:
                    for (int i = 0 - spellButtons.currentSpell.areaOfEffect; i <= spellButtons.currentSpell.areaOfEffect; i++)
                    {
                        for (int j = 0 - spellButtons.currentSpell.areaOfEffect; j <= spellButtons.currentSpell.areaOfEffect; j++)
                        {
                            if (Mathf.Abs(i) + Mathf.Abs(j) <= spellButtons.currentSpell.areaOfEffect)
                            {
                                targetTiles.Add(gridController.GetTile(mouseController.selected.locX + j, mouseController.selected.locZ + i));
                            }
                        }
                    }
                    break;
                // spellrange ei saa olla 0
                case SpellAreaType.Cone:
                    if (playerBehaviour.currentCharacter.currentTile.z < mouseController.selected.locZ)
                    {
                        ConeTargets(0, dirList, out targetTiles);
                    }
                    else if (playerBehaviour.currentCharacter.currentTile.z > mouseController.selected.locZ)
                    {
                        ConeTargets(1, dirList, out targetTiles);
                    }
                    else if (playerBehaviour.currentCharacter.currentTile.x > mouseController.selected.locX)
                    {
                        ConeTargets(3, dirList, out targetTiles);
                    }
                    else
                    {
                        ConeTargets(2, dirList, out targetTiles);
                    }
                    break;
                case SpellAreaType.Square:
                    for (int i = 0 - spellButtons.currentSpell.areaOfEffect; i <= spellButtons.currentSpell.areaOfEffect; i++)
                    {
                        for (int j = 0 - spellButtons.currentSpell.areaOfEffect; j <= spellButtons.currentSpell.areaOfEffect; j++)
                        {
                            targetTiles.Add(gridController.GetTile(mouseController.selected.locX + j, mouseController.selected.locZ + i));
                        }
                    }
                    break;
            }
            List<Tile> returnables = new List<Tile>();
            foreach (var tile in targetTiles)
            {
                if (tile != null)
                {
                    if (tile.myType == Tile.BlockType.BaseBlock)
                    {
                        returnables.Add(tile);
                    }
                }
            }
            return returnables;
        }

        private void LineTargets(int directionIndex, int[][] dirList, out List<Tile> targetTiles)
        {
            targetTiles = new List<Tile>();
            for (int i = 0; i <= spellButtons.currentSpell.areaOfEffect; i++)
            {
                targetTiles.Add(gridController.GetTile(mouseController.selected.locX + dirList[directionIndex][0] * i, mouseController.selected.locZ + dirList[directionIndex][1] * i));
            }
        }
        private void ConeTargets(int directionIndex, int[][] dirList, out List<Tile> targetTiles)
        {
            targetTiles = new List<Tile>();
            for (int i = 0; i <= spellButtons.currentSpell.areaOfEffect; i++)
            {
                for (int j = 0 - i; j <= i; j++)
                {
                    int x2 = dirList[directionIndex][0];
                    int z2 = dirList[directionIndex][1];
                    if (x2 == 0)
                    {
                        x2 = j;
                        z2 *= i;
                    }
                    if (z2 == 0)
                    {
                        z2 = j;
                        x2 *= i;
                    }
                    targetTiles.Add(gridController.GetTile(mouseController.selected.locX + x2, mouseController.selected.locZ + z2));
                }
            }
        }

        // Makes a list of Tiles on Spells range
        public List<Tile> RangeType(SpellRangeType mySpellRangeType, bool kumpitile)
        {
            List<Tile> rangetiles = new List<Tile>();
            List<Tile> returnables = new List<Tile>();
            int i = spellButtons.currentSpell.rangeMin;
            int x = spellButtons.currentSpell.rangeMin;
            if (i == 0 && spellButtons.currentSpell.rangeType != SpellRangeType.Normal)
            {

                rangetiles.Add(gridController.GetTile(playerBehaviour.currentCharacter.currentTile));
                i++;
                x++;
            }
            switch (mySpellRangeType)
            {
                case SpellRangeType.Diagonal:
                    for (i = x; i <= spellButtons.currentSpell.rangeMax; i++)
                    {
                        rangetiles.Add(gridController.GetTile(playerBehaviour.currentCharacter.currentTile.x + i, playerBehaviour.currentCharacter.currentTile.z + i));
                        rangetiles.Add(gridController.GetTile(playerBehaviour.currentCharacter.currentTile.x + i, playerBehaviour.currentCharacter.currentTile.z - i));
                        rangetiles.Add(gridController.GetTile(playerBehaviour.currentCharacter.currentTile.x - i, playerBehaviour.currentCharacter.currentTile.z + i));
                        rangetiles.Add(gridController.GetTile(playerBehaviour.currentCharacter.currentTile.x - i, playerBehaviour.currentCharacter.currentTile.z - i));
                    }
                    break;
                case SpellRangeType.Linear:
                    for (i = x; i <= spellButtons.currentSpell.rangeMax; i++)
                    {
                        rangetiles.Add(gridController.GetTile(playerBehaviour.currentCharacter.currentTile.x, playerBehaviour.currentCharacter.currentTile.z + i));
                        rangetiles.Add(gridController.GetTile(playerBehaviour.currentCharacter.currentTile.x + i, playerBehaviour.currentCharacter.currentTile.z));
                        rangetiles.Add(gridController.GetTile(playerBehaviour.currentCharacter.currentTile.x, playerBehaviour.currentCharacter.currentTile.z - i));
                        rangetiles.Add(gridController.GetTile(playerBehaviour.currentCharacter.currentTile.x - i, playerBehaviour.currentCharacter.currentTile.z));
                    }
                    break;
                case SpellRangeType.Normal:
                    List<Tile> badTiles = new List<Tile>();
                    if (spellButtons.currentSpell.rangeMin != 0)
                    {
                        List<Tile> tempo = new List<Tile>();
                        for (int z = 0 - spellButtons.currentSpell.rangeMin; z < spellButtons.currentSpell.rangeMin; z++)
                        {
                            for (int j = 0 - spellButtons.currentSpell.rangeMin; j <= spellButtons.currentSpell.rangeMin; j++)
                            {
                                if (Mathf.Abs(z) + Mathf.Abs(j) < spellButtons.currentSpell.rangeMin)
                                {
                                    tempo.Add(gridController.GetTile(playerBehaviour.currentCharacter.currentTile.x + j, playerBehaviour.currentCharacter.currentTile.z + z));
                                }
                            }
                        }
                        foreach (var tile in tempo)
                        {
                            if (tile != null)
                            {
                                badTiles.Add(tile);
                            }
                        }
                    }
                    for (i = 0 - spellButtons.currentSpell.rangeMax; i <= spellButtons.currentSpell.rangeMax; i++)
                    {
                        for (int j = 0 - spellButtons.currentSpell.rangeMax; j <= spellButtons.currentSpell.rangeMax; j++)
                        {
                            if (Mathf.Abs(i) + Mathf.Abs(j) <= spellButtons.currentSpell.rangeMax)
                            {
                                rangetiles.Add(gridController.GetTile(playerBehaviour.currentCharacter.currentTile.x + j, playerBehaviour.currentCharacter.currentTile.z + i));
                            }
                        }
                    }
                    foreach (var tile in rangetiles)
                    {
                        if (tile != null)
                        {
                            if (spellButtons.currentSpell.needLineOfSight == true)
                            {
                                if (kumpitile == false)
                                {
                                    if (lOS.LoSCheck(gridController.GetTile(playerBehaviour.currentCharacter.currentTile), tile) == true)
                                    {
                                        if (badTiles != null)
                                        {
                                            bool isOk = true;
                                            foreach (var item in badTiles)
                                            {
                                                if (gridController.GetTile(item.locX, item.locZ) == gridController.GetTile(tile.locX, tile.locZ))
                                                {
                                                    isOk = false;
                                                }
                                            }
                                            if (tile.myType == Tile.BlockType.BaseBlock && isOk == true)
                                            {
                                                returnables.Add(tile);
                                            }
                                        }
                                        else
                                        {
                                            if (tile.myType == Tile.BlockType.BaseBlock)
                                            {
                                                returnables.Add(tile);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (lOS.LoSCheck(gridController.GetTile(playerBehaviour.currentCharacter.currentTile), tile) == false)
                                    {
                                        if (badTiles != null)
                                        {
                                            bool isOk = true;
                                            foreach (var item in badTiles)
                                            {
                                                if (gridController.GetTile(item.locX, item.locZ) == gridController.GetTile(tile.locX, tile.locZ))
                                                {
                                                    isOk = false;
                                                }
                                            }
                                            if (tile.myType == Tile.BlockType.BaseBlock && isOk == true)
                                            {
                                                returnables.Add(tile);
                                            }
                                        }
                                        else
                                        {
                                            if (tile.myType == Tile.BlockType.BaseBlock)
                                            {
                                                returnables.Add(tile);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (badTiles != null)
                                {
                                    bool isOk = true;
                                    foreach (var item in badTiles)
                                    {
                                        if (gridController.GetTile(item.locX, item.locZ) == gridController.GetTile(tile.locX, tile.locZ))
                                        {
                                            isOk = false;
                                        }
                                    }
                                    if (tile.myType == Tile.BlockType.BaseBlock && isOk == true)
                                    {
                                        returnables.Add(tile);
                                    }
                                }
                                else
                                {
                                    if (tile.myType == Tile.BlockType.BaseBlock)
                                    {
                                        returnables.Add(tile);
                                    }
                                }
                            }
                        }
                    }
                    break;
                case SpellRangeType.LinDiag:
                    for (i = x; i <= spellButtons.currentSpell.rangeMax; i++)
                    {
                        rangetiles.Add(gridController.GetTile(playerBehaviour.currentCharacter.currentTile.x, playerBehaviour.currentCharacter.currentTile.z + i));
                        rangetiles.Add(gridController.GetTile(playerBehaviour.currentCharacter.currentTile.x + i, playerBehaviour.currentCharacter.currentTile.z));
                        rangetiles.Add(gridController.GetTile(playerBehaviour.currentCharacter.currentTile.x, playerBehaviour.currentCharacter.currentTile.z - i));
                        rangetiles.Add(gridController.GetTile(playerBehaviour.currentCharacter.currentTile.x - i, playerBehaviour.currentCharacter.currentTile.z));
                    }
                    for (i = x; i <= spellButtons.currentSpell.rangeMax; i++)
                    {
                        rangetiles.Add(gridController.GetTile(playerBehaviour.currentCharacter.currentTile.x + i, playerBehaviour.currentCharacter.currentTile.z + i));
                        rangetiles.Add(gridController.GetTile(playerBehaviour.currentCharacter.currentTile.x + i, playerBehaviour.currentCharacter.currentTile.z - i));
                        rangetiles.Add(gridController.GetTile(playerBehaviour.currentCharacter.currentTile.x - i, playerBehaviour.currentCharacter.currentTile.z + i));
                        rangetiles.Add(gridController.GetTile(playerBehaviour.currentCharacter.currentTile.x - i, playerBehaviour.currentCharacter.currentTile.z - i));
                    }
                    break;
                default:
                    break;
            }

            if (spellButtons.currentSpell.rangeType != SpellRangeType.Normal)
            {
                foreach (var tile in rangetiles)
                {
                    if (tile != null)
                    {
                        if (spellButtons.currentSpell.needLineOfSight == true)
                        {
                            if (kumpitile == false)
                            {
                                if (lOS.LoSCheck(gridController.GetTile(playerBehaviour.currentCharacter.currentTile), tile) == true)
                                {
                                    if (tile.myType == Tile.BlockType.BaseBlock)
                                        returnables.Add(tile);
                                }
                            }
                            else
                            {
                                if (lOS.LoSCheck(gridController.GetTile(playerBehaviour.currentCharacter.currentTile), tile) == false)
                                {
                                    if (tile.myType == Tile.BlockType.BaseBlock)
                                        returnables.Add(tile);
                                }
                            }
                        }
                        else
                           if (tile.myType == Tile.BlockType.BaseBlock)
                            returnables.Add(tile);
                    }
                }
            }
            return returnables;
        }

    }

}