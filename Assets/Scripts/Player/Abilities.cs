using CharacterSystem;
using GameFieldSystem;
using ManagementSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpellSystem
{
    //Muokka summarya vapaasti 
    /// <summary>
    /// Lists abilities for the attached player. Generates the list using a class enum.
    /// Stores the patterns used in different abilites.
    /// </summary>

    public class Abilities : MonoBehaviour
    {
        public enum SpellAreaType { Cross, Line, Normal, Square, Cone, Diagonal }; // Different types of AoE
        public enum SpellRangeType { Linear, Diagonal, Normal, LinDiag }; // How Player Targets the spell
        public enum SpellPushType { LineFromPlayer, DiagonalFromPlayer, BothFromPlayer, LineFromMouse, DiagonalFromMouse, BothFromMouse };
        public enum SpellPullType { LineTowardsPlayer, DiagonalTowardsPlayer, BothTowardsPlayer, LineTowardsMouse, DiagonalTowardsMouse, BothTowardsMouse, };

        GridController gridController;
        PlayerBehaviour playerBehaviour;
        MouseController mouseController;
        SpellCast spellCast;
        LineOfSight lOS;

        void Start()
        {
            mouseController = GameObject.FindGameObjectWithTag("MouseManager").GetComponent<MouseController>();
            gridController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridController>();
            spellCast = GetComponent<SpellCast>();
            playerBehaviour = GetComponent<PlayerBehaviour>();
            lOS = GetComponent<LineOfSight>();
            if (!gridController)
                Debug.LogWarning("Gridcontroller is null!");
            if (!mouseController)
                Debug.LogWarning("Mousecontroller is null!");
        }

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

                //spellrange min ei saa olla 0
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
                    for (int i = 1; i <= spellCast.currentSpell.areaOfEffect; i++)
                    {
                        foreach (var c in dirList)
                        {
                            targetTiles.Add(gridController.GetTile(mouseController.selected.locX + c[0] * i, mouseController.selected.locZ + c[1] * i));
                        }
                    }
                    break;
                case SpellAreaType.Diagonal:
                    targetTiles.Add(mouseController.selected);
                    for (int i = 1; i <= spellCast.currentSpell.areaOfEffect; i++)
                    {
                        targetTiles.Add(gridController.GetTile(mouseController.selected.locX + i, mouseController.selected.locZ + i));
                        targetTiles.Add(gridController.GetTile(mouseController.selected.locX + i, mouseController.selected.locZ - i));
                        targetTiles.Add(gridController.GetTile(mouseController.selected.locX - i, mouseController.selected.locZ + i));
                        targetTiles.Add(gridController.GetTile(mouseController.selected.locX - i, mouseController.selected.locZ - i));
                    }
                    break;
                case SpellAreaType.Normal:
                    for (int i = 0 - spellCast.currentSpell.areaOfEffect; i <= spellCast.currentSpell.areaOfEffect; i++)
                    {
                        for (int j = 0 - spellCast.currentSpell.areaOfEffect; j <= spellCast.currentSpell.areaOfEffect; j++)
                        {
                            if (Mathf.Abs(i) + Mathf.Abs(j) <= spellCast.currentSpell.areaOfEffect)
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
                    for (int i = 0 - spellCast.currentSpell.areaOfEffect; i <= spellCast.currentSpell.areaOfEffect; i++)
                    {
                        for (int j = 0 - spellCast.currentSpell.areaOfEffect; j <= spellCast.currentSpell.areaOfEffect; j++)
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
            for (int i = 0; i <= spellCast.currentSpell.areaOfEffect; i++)
            {
                targetTiles.Add(gridController.GetTile(mouseController.selected.locX + dirList[directionIndex][0] * i, mouseController.selected.locZ + dirList[directionIndex][1] * i));
            }
        }
        private void ConeTargets(int directionIndex, int[][] dirList, out List<Tile> targetTiles)
        {
            targetTiles = new List<Tile>();
            for (int i = 0; i <= spellCast.currentSpell.areaOfEffect; i++)
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

        public List<Tile> RangeType(SpellRangeType mySpellRangeType, bool kumpitile)
        {
            List<Tile> rangetiles = new List<Tile>();
            List<Tile> returnables = new List<Tile>();
            int i = spellCast.currentSpell.rangeMin;
            int x = spellCast.currentSpell.rangeMin;
            if (i == 0 && spellCast.currentSpell.rangeType != SpellRangeType.Normal)
            {
                rangetiles.Add(gridController.GetTile(playerBehaviour.currentCharacter.currentTile));
                i++;
                x++;
            }
            switch (mySpellRangeType)
            {
                case SpellRangeType.Diagonal:
                    for (i = x; i <= spellCast.currentSpell.rangeMax; i++)
                    {
                        rangetiles.Add(gridController.GetTile(playerBehaviour.currentCharacter.currentTile.x + i, playerBehaviour.currentCharacter.currentTile.z + i));
                        rangetiles.Add(gridController.GetTile(playerBehaviour.currentCharacter.currentTile.x + i, playerBehaviour.currentCharacter.currentTile.z - i));
                        rangetiles.Add(gridController.GetTile(playerBehaviour.currentCharacter.currentTile.x - i, playerBehaviour.currentCharacter.currentTile.z + i));
                        rangetiles.Add(gridController.GetTile(playerBehaviour.currentCharacter.currentTile.x - i, playerBehaviour.currentCharacter.currentTile.z - i));
                    }
                    break;
                case SpellRangeType.Linear:
                    for (i = x; i <= spellCast.currentSpell.rangeMax; i++)
                    {
                        rangetiles.Add(gridController.GetTile(playerBehaviour.currentCharacter.currentTile.x, playerBehaviour.currentCharacter.currentTile.z + i));
                        rangetiles.Add(gridController.GetTile(playerBehaviour.currentCharacter.currentTile.x + i, playerBehaviour.currentCharacter.currentTile.z));
                        rangetiles.Add(gridController.GetTile(playerBehaviour.currentCharacter.currentTile.x, playerBehaviour.currentCharacter.currentTile.z - i));
                        rangetiles.Add(gridController.GetTile(playerBehaviour.currentCharacter.currentTile.x - i, playerBehaviour.currentCharacter.currentTile.z));
                    }
                    break;
                case SpellRangeType.Normal:
                    List<Tile> badTiles = new List<Tile>();
                    if (spellCast.currentSpell.rangeMin != 0)
                    {
                        List<Tile> tempo = new List<Tile>();
                        for (int z = 0 - spellCast.currentSpell.rangeMin; z < spellCast.currentSpell.rangeMin; z++)
                        {
                            for (int j = 0 - spellCast.currentSpell.rangeMin; j <= spellCast.currentSpell.rangeMin; j++)
                            {
                                if (Mathf.Abs(z) + Mathf.Abs(j) < spellCast.currentSpell.rangeMin)
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
                    for (i = 0 - spellCast.currentSpell.rangeMax; i <= spellCast.currentSpell.rangeMax; i++)
                    {
                        for (int j = 0 - spellCast.currentSpell.rangeMax; j <= spellCast.currentSpell.rangeMax; j++)
                        {
                            if (Mathf.Abs(i) + Mathf.Abs(j) <= spellCast.currentSpell.rangeMax)
                            {
                                rangetiles.Add(gridController.GetTile(playerBehaviour.currentCharacter.currentTile.x + j, playerBehaviour.currentCharacter.currentTile.z + i));
                            }
                        }
                    }
                    foreach (var tile in rangetiles)
                    {
                        if (tile != null)
                        {
                            if (spellCast.currentSpell.needLineOfSight == true)
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
                    for (i = x; i <= spellCast.currentSpell.rangeMax; i++)
                    {
                        rangetiles.Add(gridController.GetTile(playerBehaviour.currentCharacter.currentTile.x, playerBehaviour.currentCharacter.currentTile.z + i));
                        rangetiles.Add(gridController.GetTile(playerBehaviour.currentCharacter.currentTile.x + i, playerBehaviour.currentCharacter.currentTile.z));
                        rangetiles.Add(gridController.GetTile(playerBehaviour.currentCharacter.currentTile.x, playerBehaviour.currentCharacter.currentTile.z - i));
                        rangetiles.Add(gridController.GetTile(playerBehaviour.currentCharacter.currentTile.x - i, playerBehaviour.currentCharacter.currentTile.z));
                    }
                    for (i = x; i <= spellCast.currentSpell.rangeMax; i++)
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

            if (spellCast.currentSpell.rangeType != SpellRangeType.Normal)
            {
                foreach (var tile in rangetiles)
                {
                    if (tile != null)
                    {
                        if (spellCast.currentSpell.needLineOfSight == true)
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



        //public void SpellPull(SpellPullType myPullRangeType)
        //{
        //    List<Tile> AoeList = AreaType(spellCast.currentSpell.areaType);
        //    List<Tile> targetList = new List<Tile>();
        //    Tile anchor = mouseController.selected;
        //    Tile caster = gridController.GetTile(playerBehaviour.currentCharacter.currentTile);
        //    foreach (var tile in AoeList)
        //    {
        //        if (tile.CharCurrentlyOnTile && tile != caster)
        //        {
        //            Debug.Log("founf char");
        //            targetList.Add(tile);
        //        }
        //    }
        //    foreach (var item in targetList)
        //    {
        //        Tile carpet = item;
        //        Debug.Log("starting pull on target");
        //        GridController.Directions mydirection = new GridController.Directions();
        //        GridController.Directions helpDirection = new GridController.Directions();
        //        bool inLine = true;
        //        switch (myPullRangeType) {
        //            case SpellPullType.BothTowardsMouse:
        //                if (item.locX == anchor.locX && item.locZ < anchor.locZ)
        //                {
        //                    Debug.Log("pull up");
        //                    mydirection = GridController.Directions.up;
        //                    inLine = true;
        //                }
        //                else if (item.locX == anchor.locX && item.locZ > anchor.locZ)
        //                {
        //                    Debug.Log("pull down");
        //                    mydirection = GridController.Directions.down;
        //                    inLine = true;
        //                }
        //                else if (item.locZ == anchor.locZ && item.locX < anchor.locX)
        //                {
        //                    Debug.Log("pull right");
        //                    mydirection = GridController.Directions.right;
        //                    inLine = true;
        //                }
        //                else if (item.locZ == anchor.locZ && item.locX > anchor.locX)
        //                {
        //                    Debug.Log("pull left");
        //                    mydirection = GridController.Directions.left;
        //                    inLine = true;
        //                }
        //                else if (item.locX < anchor.locX && item.locZ < anchor.locZ)
        //                {
        //                    Debug.Log("pull up right");
        //                    mydirection = GridController.Directions.up;
        //                    helpDirection = GridController.Directions.right;
        //                    inLine = false;
        //                }
        //                else if (item.locX < anchor.locX && item.locZ > anchor.locZ)
        //                {
        //                    Debug.Log("pull down right");
        //                    mydirection = GridController.Directions.down;
        //                    helpDirection = GridController.Directions.right;
        //                    inLine = false;
        //                }
        //                else if (item.locX > anchor.locX && item.locZ < anchor.locZ)
        //                {
        //                    Debug.Log("pull up left");
        //                    mydirection = GridController.Directions.up;
        //                    helpDirection = GridController.Directions.left;
        //                    inLine = false;
        //                }
        //                else if (item.locX > anchor.locX && item.locZ > anchor.locZ)
        //                {
        //                    Debug.Log("pull down left");
        //                    mydirection = GridController.Directions.down;
        //                    helpDirection = GridController.Directions.left;
        //                    inLine = false;
        //                }
        //                break;
        //            case SpellPullType.BothTowardsPlayer:
        //                if (anchor.locX == caster.locX && anchor.locZ < caster.locZ)
        //                {
        //                    mydirection = GridController.Directions.up;
        //                    inLine = true;
        //                }
        //                else if (anchor.locX == caster.locX && anchor.locZ > caster.locZ)
        //                {
        //                    mydirection = GridController.Directions.down;
        //                    inLine = true;
        //                }
        //                else if (anchor.locZ == caster.locZ && anchor.locX < caster.locX)
        //                {
        //                    mydirection = GridController.Directions.right;
        //                    inLine = true;
        //                }
        //                else if (anchor.locZ == caster.locZ && anchor.locX > caster.locX)
        //                {
        //                    mydirection = GridController.Directions.left;
        //                    inLine = true;
        //                }
        //                else if (anchor.locX < caster.locX && anchor.locZ < caster.locZ)
        //                {
        //                    mydirection = GridController.Directions.up;
        //                    helpDirection = GridController.Directions.right;
        //                    inLine = false;
        //                }
        //                else if (anchor.locX < caster.locX && anchor.locZ > caster.locZ)
        //                {
        //                    mydirection = GridController.Directions.down;
        //                    helpDirection = GridController.Directions.right;
        //                    inLine = false;
        //                }
        //                else if (anchor.locX > caster.locX && anchor.locZ < caster.locZ)
        //                {
        //                    mydirection = GridController.Directions.up;
        //                    helpDirection = GridController.Directions.left;
        //                    inLine = false;
        //                }
        //                else if (anchor.locX > caster.locX && anchor.locZ > caster.locZ)
        //                {
        //                    mydirection = GridController.Directions.down;
        //                    helpDirection = GridController.Directions.left;
        //                    inLine = false;
        //                }
        //                break;
        //            case SpellPullType.DiagonalTowardsMouse:
        //                if (item.locX < anchor.locX && item.locZ < anchor.locZ)
        //                {
        //                    mydirection = GridController.Directions.up;
        //                    helpDirection = GridController.Directions.right;
        //                    inLine = false;
        //                }
        //                else if (item.locX < anchor.locX && item.locZ > anchor.locZ)
        //                {
        //                    mydirection = GridController.Directions.down;
        //                    helpDirection = GridController.Directions.right;
        //                    inLine = false;
        //                }
        //                else if (item.locX > anchor.locX && item.locZ < anchor.locZ)
        //                {
        //                    mydirection = GridController.Directions.up;
        //                    helpDirection = GridController.Directions.left;
        //                    inLine = false;
        //                }
        //                else if (item.locX > anchor.locX && item.locZ > anchor.locZ)
        //                {
        //                    mydirection = GridController.Directions.down;
        //                    helpDirection = GridController.Directions.left;
        //                    inLine = false;
        //                }
        //                break;
        //            case SpellPullType.DiagonalTowardsPlayer:
        //                if (anchor.locX < caster.locX && anchor.locZ < caster.locZ)
        //                {
        //                    mydirection = GridController.Directions.up;
        //                    helpDirection = GridController.Directions.right;
        //                    inLine = false;
        //                }
        //                else if (anchor.locX < caster.locX && anchor.locZ > caster.locZ)
        //                {
        //                    mydirection = GridController.Directions.down;
        //                    helpDirection = GridController.Directions.right;
        //                    inLine = false;
        //                }
        //                else if (anchor.locX > caster.locX && anchor.locZ < caster.locZ)
        //                {
        //                    mydirection = GridController.Directions.up;
        //                    helpDirection = GridController.Directions.left;
        //                    inLine = false;
        //                }
        //                else if (anchor.locX > caster.locX && anchor.locZ > caster.locZ)
        //                {
        //                    mydirection = GridController.Directions.down;
        //                    helpDirection = GridController.Directions.left;
        //                    inLine = false;
        //                }
        //                break;
        //            case SpellPullType.LineTowardsMouse:
        //                if (item.locX == anchor.locX && item.locZ < anchor.locZ)
        //                {
        //                    mydirection = GridController.Directions.up;
        //                    inLine = true;
        //                }
        //                else if (item.locX == anchor.locX && item.locZ > anchor.locZ)
        //                {
        //                    mydirection = GridController.Directions.down;
        //                    inLine = true;
        //                }
        //                else if (item.locZ == anchor.locZ && item.locX < anchor.locX)
        //                {
        //                    mydirection = GridController.Directions.right;
        //                    inLine = true;
        //                }
        //                else if (item.locZ == anchor.locZ && item.locX > anchor.locX)
        //                {
        //                    mydirection = GridController.Directions.left;
        //                    inLine = true;
        //                }
        //                break;
        //            case SpellPullType.LineTowardsPlayer:
        //                if (anchor.locX == caster.locX && anchor.locZ < caster.locZ)
        //                {
        //                    mydirection = GridController.Directions.up;
        //                    inLine = true;
        //                }
        //                else if (anchor.locX == caster.locX && anchor.locZ > caster.locZ)
        //                {
        //                    mydirection = GridController.Directions.down;
        //                    inLine = true;

        //                }
        //                else if (anchor.locZ == caster.locZ && anchor.locX < caster.locX)
        //                {
        //                    mydirection = GridController.Directions.right;
        //                    inLine = true;
        //                }
        //                else if (anchor.locZ == caster.locZ && anchor.locX > caster.locX)
        //                {
        //                    mydirection = GridController.Directions.left;
        //                    inLine = true;
        //                }
        //                break;
        //            default:
        //                break;
        //        }
        //        if (inLine == true)
        //        {
        //            for (int i = 0; i < spellCast.currentSpell.spellPull; i++)
        //            {
        //                Debug.Log("causing pull action");
        //                Tile temp = gridController.GetTileInDirection(gridController.GetTile(carpet.locX, carpet.locZ), 1, mydirection);
        //                if (temp.myType == Tile.BlockType.BaseBlock)
        //                {
        //                    Debug.Log("Making the pull");
        //                    PullPushAct(carpet, temp);
        //                    carpet = temp;
        //                }
        //                else
        //                {
        //                    Debug.Log("wall hit");
        //                    break;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            for (int i = 0; i < spellCast.currentSpell.spellPull; i++)
        //            {
        //                Tile temp1 = gridController.GetTileInDirection(gridController.GetTile(carpet.locX, carpet.locZ), 1, mydirection);
        //                Tile temp2 = gridController.GetTileInDirection(gridController.GetTile(carpet.locX, carpet.locZ), 1, helpDirection);
        //                Tile temp3 = gridController.GetTileInDirection(gridController.GetTile(temp1.locX, temp1.locZ), 1, helpDirection);
        //                if (temp1.myType == Tile.BlockType.BaseBlock && temp2.myType == Tile.BlockType.BaseBlock && temp3.myType == Tile.BlockType.BaseBlock)
        //                {
        //                    PullPushAct(carpet, temp3);
        //                    carpet = temp3;
        //                    //item.locX = temp3.locX;
        //                    //item.locZ = temp3.locZ;
        //                }
        //                else
        //                {
        //                    break;
        //                }
        //            }
        //        }
        //        inLine = true;
        //    }
        //}

        //public void SpellPush(SpellPushType myPushRangeType)
        //{
        //    List<Tile> AoeList = AreaType(spellCast.currentSpell.mySpellAreaType);
        //    List<Tile> targetList = new List<Tile>();
        //    Tile anchor = mouseController.selected;
        //    Tile caster = gridController.GetTile(playerBehaviour.currentCharacter.currentTile);
        //    // etsii siirrettävät pelaajat aoe:sta
        //    foreach (var tile in AoeList)
        //    {
        //        if (tile.CharCurrentlyOnTile && tile != caster)
        //        {
        //            targetList.Add(tile);
        //        }
        //    }
        //    foreach (var item in targetList)
        //    {
        //        Tile carpet = item;
        //        GridController.Directions mydirection = new GridController.Directions();
        //        GridController.Directions helpDirection = new GridController.Directions();
        //        bool inLine = true;
        //        switch (myPushRangeType)
        //        {
        //            case SpellPushType.BothFromMouse:
        //                if (item.locX == anchor.locX && item.locZ < anchor.locZ)
        //                {
        //                    mydirection = GridController.Directions.down;
        //                    inLine = true;
        //                }
        //                else if (item.locX == anchor.locX && item.locZ > anchor.locZ)
        //                {
        //                    mydirection = GridController.Directions.up;
        //                    inLine = true;
        //                }
        //                else if (item.locZ == anchor.locZ && item.locX < anchor.locX)
        //                {
        //                    mydirection = GridController.Directions.left;
        //                    inLine = true;
        //                }
        //                else if (item.locZ == anchor.locZ && item.locX > anchor.locX)
        //                {
        //                    mydirection = GridController.Directions.right;
        //                    inLine = true;
        //                }
        //                else if (item.locX < anchor.locX && item.locZ < anchor.locZ)
        //                {
        //                    mydirection = GridController.Directions.down;
        //                    helpDirection = GridController.Directions.left;
        //                    inLine = false;
        //                }
        //                else if (item.locX < anchor.locX && item.locZ > anchor.locZ)
        //                {
        //                    mydirection = GridController.Directions.up;
        //                    helpDirection = GridController.Directions.left;
        //                    inLine = false;
        //                }
        //                else if (item.locX > anchor.locX && item.locZ < anchor.locZ)
        //                {
        //                    mydirection = GridController.Directions.down;
        //                    helpDirection = GridController.Directions.right;
        //                    inLine = false;
        //                }
        //                else if (item.locX > anchor.locX && item.locZ > anchor.locZ)
        //                {
        //                    mydirection = GridController.Directions.up;
        //                    helpDirection = GridController.Directions.right;
        //                    inLine = false;
        //                }
        //                break;
        //            case SpellPushType.BothFromPlayer:
        //                if (anchor.locX == caster.locX && anchor.locZ < caster.locZ)
        //                {
        //                    mydirection = GridController.Directions.down;
        //                    inLine = true;

        //                }
        //                else if (anchor.locX == caster.locX && anchor.locZ > caster.locZ)
        //                {
        //                    mydirection = GridController.Directions.up;
        //                    inLine = true;
        //                }
        //                else if (anchor.locZ == caster.locZ && anchor.locX < caster.locX)
        //                {
        //                    mydirection = GridController.Directions.left;
        //                    inLine = true;
        //                }
        //                else if (anchor.locZ == caster.locZ && anchor.locX > caster.locX)
        //                {
        //                    mydirection = GridController.Directions.right;
        //                    inLine = true;
        //                }
        //                else if (anchor.locX < caster.locX && anchor.locZ < caster.locZ)
        //                {
        //                    mydirection = GridController.Directions.down;
        //                    helpDirection = GridController.Directions.left;
        //                    inLine = false;
        //                }
        //                else if (anchor.locX < caster.locX && anchor.locZ > caster.locZ)
        //                {
        //                    mydirection = GridController.Directions.up;
        //                    helpDirection = GridController.Directions.left;
        //                    inLine = false;
        //                }
        //                else if (anchor.locX > caster.locX && anchor.locZ < caster.locZ)
        //                {
        //                    mydirection = GridController.Directions.down;
        //                    helpDirection = GridController.Directions.right;
        //                    inLine = false;
        //                }
        //                else if (anchor.locX > caster.locX && anchor.locZ > caster.locZ)
        //                {
        //                    mydirection = GridController.Directions.up;
        //                    helpDirection = GridController.Directions.right;
        //                    inLine = false;
        //                }
        //                break;
        //            case SpellPushType.DiagonalFromMouse:
        //                if (item.locX < anchor.locX && item.locZ < anchor.locZ)
        //                {
        //                    mydirection = GridController.Directions.down;
        //                    helpDirection = GridController.Directions.left;
        //                    inLine = false;
        //                }
        //                else if (item.locX < anchor.locX && item.locZ > anchor.locZ)
        //                {
        //                    mydirection = GridController.Directions.up;
        //                    helpDirection = GridController.Directions.left;
        //                    inLine = false;
        //                }
        //                else if (item.locX > anchor.locX && item.locZ < anchor.locZ)
        //                {
        //                    mydirection = GridController.Directions.down;
        //                    helpDirection = GridController.Directions.right;
        //                    inLine = false;
        //                }
        //                else if (item.locX > anchor.locX && item.locZ > anchor.locZ)
        //                {
        //                    mydirection = GridController.Directions.up;
        //                    helpDirection = GridController.Directions.right;
        //                    inLine = false;
        //                }
        //                break;
        //            case SpellPushType.DiagonalFromPlayer:
        //                if (anchor.locX < caster.locX && anchor.locZ < caster.locZ)
        //                {
        //                    mydirection = GridController.Directions.down;
        //                    helpDirection = GridController.Directions.left;
        //                    inLine = false;
        //                }
        //                else if (anchor.locX < caster.locX && anchor.locZ > caster.locZ)
        //                {
        //                    mydirection = GridController.Directions.up;
        //                    helpDirection = GridController.Directions.left;
        //                    inLine = false;
        //                }
        //                else if (anchor.locX > caster.locX && anchor.locZ < caster.locZ)
        //                {
        //                    mydirection = GridController.Directions.down;
        //                    helpDirection = GridController.Directions.right;
        //                    inLine = false;
        //                }
        //                else if (anchor.locX > caster.locX && anchor.locZ > caster.locZ)
        //                {
        //                    mydirection = GridController.Directions.up;
        //                    helpDirection = GridController.Directions.right;
        //                    inLine = false;
        //                }
        //                break;
        //            case SpellPushType.LineFromMouse:
        //                if (item.locX == anchor.locX && item.locZ < anchor.locZ)
        //                {
        //                    mydirection = GridController.Directions.down;
        //                    inLine = true;
        //                }
        //                else if (item.locX == anchor.locX && item.locZ > anchor.locZ)
        //                {
        //                    mydirection = GridController.Directions.up;
        //                    inLine = true;
        //                }
        //                else if (item.locZ == anchor.locZ && item.locX < anchor.locX)
        //                {
        //                    mydirection = GridController.Directions.left;
        //                    inLine = true;
        //                }
        //                else if (item.locZ == anchor.locZ && item.locX > anchor.locX)
        //                {
        //                    mydirection = GridController.Directions.right;
        //                    inLine = true;
        //                }
        //                break;
        //            case SpellPushType.LineFromPlayer:
        //                if (anchor.locX == caster.locX && anchor.locZ < caster.locZ)
        //                {
        //                    mydirection = GridController.Directions.down;
        //                    inLine = true;
        //                }
        //                else if (anchor.locX == caster.locX && anchor.locZ > caster.locZ)
        //                {
        //                    mydirection = GridController.Directions.up;
        //                    inLine = true;
        //                }
        //                else if (anchor.locZ == caster.locZ && anchor.locX < caster.locX)
        //                {
        //                    mydirection = GridController.Directions.left;
        //                    inLine = true;
        //                }
        //                else if (anchor.locZ == caster.locZ && anchor.locX > caster.locX)
        //                {
        //                    mydirection = GridController.Directions.right;
        //                    inLine = true;
        //                }
        //                break;
        //            default:
        //                break;
        //        }
        //        if (inLine == true)
        //        {
        //            for (int i = 0; i < spellCast.currentSpell.spellPushback; i++)
        //            {
        //                Tile temp = gridController.GetTileInDirection(gridController.GetTile(carpet.locX, carpet.locZ), 1, mydirection);
        //                if (temp.myType == Tile.BlockType.BaseBlock)
        //                {
        //                    Debug.Log("Making the pull");
        //                    PullPushAct(carpet, temp);
        //                    carpet = temp;
        //                }
        //                else
        //                {
        //                    break;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            for (int i = 0; i < spellCast.currentSpell.spellPushback; i++)
        //            {
        //                Tile temp1 = gridController.GetTileInDirection(gridController.GetTile(carpet.locX, carpet.locZ), 1, mydirection);
        //                Tile temp2 = gridController.GetTileInDirection(gridController.GetTile(carpet.locX, carpet.locZ), 1, helpDirection);
        //                Tile temp3 = gridController.GetTileInDirection(gridController.GetTile(temp1.locX, temp1.locZ), 1, helpDirection);
        //                if (temp1.myType == Tile.BlockType.BaseBlock && temp2.myType == Tile.BlockType.BaseBlock && temp3.myType == Tile.BlockType.BaseBlock)
        //                {
        //                    PullPushAct(carpet, temp3);
        //                    carpet = temp3;
        //                    //item.locX = temp3.locX;
        //                    //item.locZ = temp3.locZ;
        //                }
        //                else
        //                {
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //}

        public void TeleportSwitch(Tile caster, Tile target)
        {
            PlayerMovement playerMovement = null;
            PlayerMovement targetMovement = null;
            if (caster.CharCurrentlyOnTile)
            {
                Debug.Log("casteri löydetty");
                playerMovement = caster.CharCurrentlyOnTile.gameObject.GetComponent<PlayerMovement>();
            }
            if (target.CharCurrentlyOnTile)
            {
                Debug.Log("maali löydetty");
                targetMovement = target.CharCurrentlyOnTile.gameObject.GetComponent<PlayerMovement>();
            }
            if (playerMovement && targetMovement)
            {
                Debug.Log("tehdään siirto");
                playerMovement.MoveToTile(target, PlayerMovement.MovementMethod.Teleport);
                targetMovement.MoveToTile(caster, PlayerMovement.MovementMethod.Teleport);
            }
        }
        public void CasterTeleport(Tile caster)
        {
            PlayerMovement playerTeleport = null;
            playerTeleport = caster.CharCurrentlyOnTile.gameObject.GetComponent<PlayerMovement>();
            if (playerTeleport && mouseController.selected.CharCurrentlyOnTile == false)
            {
                playerTeleport.MoveToTile(mouseController.selected, PlayerMovement.MovementMethod.Teleport);
            }
        }
        public void PullPushAct(Tile start, Tile end)
        {
            // move player on tile start onto tile end
            PlayerMovement playerMovement = null;
            if (start.CharCurrentlyOnTile)
            {
                playerMovement = start.CharCurrentlyOnTile.gameObject.GetComponent<PlayerMovement>();
            }
            if (playerMovement)
            {
                //playerMovement.MoveToTile(end, PlayerMovement.MovementMethod.push);    //Tämä tulee lopulliseen versioon, ei vielä implementoitu
                playerMovement.MoveToTile(end, PlayerMovement.MovementMethod.Teleport);  //Väliaikainen liikkuminen
            }
        }

        public bool CheckCastability(SpellSystem.SpellCreator spell, Tile target)
        {

            if (TargetInRangeCheck(spell, target) == false)
            {
                return false;
            }
            if (playerBehaviour.currentCharacter.currentAp < spellCast.currentSpell.spellApCost)
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
        public bool TargetInRangeCheck(SpellSystem.SpellCreator spell, Tile target)
        {
            //// ---------------------- nämä conflictas, muut oli ok -------------------------
            List<Tile> range = RangeType(spell.rangeType, false);
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

        //public void WalkTowardsTarget()
        //{
        //    Tile anchor = mouseController.selected;
        //    Tile caster = gridController.GetTile(playerBehaviour.currentCharacter.currentTile);
        //    GridController.Directions mydirection = new GridController.Directions();
        //    GridController.Directions helpDirection = new GridController.Directions();
        //    bool inLine = true;
        //    if (anchor.locX == caster.locX && anchor.locZ < caster.locZ)
        //    {
        //        mydirection = GridController.Directions.down;
        //        inLine = true;

        //    }
        //    else if (anchor.locX == caster.locX && anchor.locZ > caster.locZ)
        //    {
        //        mydirection = GridController.Directions.up;
        //        inLine = true;
        //    }
        //    else if (anchor.locZ == caster.locZ && anchor.locX < caster.locX)
        //    {
        //        mydirection = GridController.Directions.left;
        //        inLine = true;
        //    }
        //    else if (anchor.locZ == caster.locZ && anchor.locX > caster.locX)
        //    {
        //        mydirection = GridController.Directions.right;
        //        inLine = true;
        //    }
        //    else if (anchor.locX < caster.locX && anchor.locZ < caster.locZ)
        //    {
        //        mydirection = GridController.Directions.down;
        //        helpDirection = GridController.Directions.left;
        //        inLine = false;
        //    }
        //    else if (anchor.locX < caster.locX && anchor.locZ > caster.locZ)
        //    {
        //        mydirection = GridController.Directions.up;
        //        helpDirection = GridController.Directions.left;
        //        inLine = false;
        //    }
        //    else if (anchor.locX > caster.locX && anchor.locZ < caster.locZ)
        //    {
        //        mydirection = GridController.Directions.down;
        //        helpDirection = GridController.Directions.right;
        //        inLine = false;
        //    }
        //    else if (anchor.locX > caster.locX && anchor.locZ > caster.locZ)
        //    {
        //        mydirection = GridController.Directions.up;
        //        helpDirection = GridController.Directions.right;
        //        inLine = false;
        //    }
        //    if (inLine == true)
        //    {
        //        for (int i = 0; i < spellCast.currentSpell.moveCloserToTarget; i++)
        //        {
        //            Tile temp = gridController.GetTileInDirection(caster, 1, mydirection);
        //            if (temp.myType == Tile.BlockType.BaseBlock && temp != null)
        //            {
        //                PullPushAct(caster, temp);
        //                caster = gridController.GetTile(playerBehaviour.currentCharacter.currentTile);
        //            }
        //            else
        //            {
        //                break;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        for (int i = 0; i < spellCast.currentSpell.moveCloserToTarget; i++)
        //        {
        //            Tile temp1 = gridController.GetTileInDirection(caster, 1, mydirection);
        //            Tile temp2 = gridController.GetTileInDirection(caster, 1, helpDirection);
        //            Tile temp3 = gridController.GetTileInDirection(gridController.GetTile(temp1.locX, temp1.locZ), 1, helpDirection);
        //            if (temp1.myType == Tile.BlockType.BaseBlock && temp2.myType == Tile.BlockType.BaseBlock && temp3.myType == Tile.BlockType.BaseBlock && temp1 != null && temp2 != null && temp3 != null)
        //            {
        //                PullPushAct(caster, temp3);
        //                caster = gridController.GetTile(playerBehaviour.currentCharacter.currentTile);
        //            }
        //            else
        //            {
        //                break;
        //            }
        //        }
        //    }
        //}
        //public void MoveAwayFromTarget()
        //{
        //    Tile anchor = mouseController.selected;
        //    Tile caster = gridController.GetTile(playerBehaviour.currentCharacter.currentTile);
        //    GridController.Directions mydirection = new GridController.Directions();
        //    GridController.Directions helpDirection = new GridController.Directions();
        //    bool inLine = true;
        //    if (anchor.locX == caster.locX && anchor.locZ < caster.locZ)
        //    {
        //        mydirection = GridController.Directions.up;
        //        inLine = true;
        //    }
        //    else if (anchor.locX == caster.locX && anchor.locZ > caster.locZ)
        //    {
        //        mydirection = GridController.Directions.down;
        //        inLine = true;
        //    }
        //    else if (anchor.locZ == caster.locZ && anchor.locX < caster.locX)
        //    {
        //        mydirection = GridController.Directions.right;
        //        inLine = true;
        //    }
        //    else if (anchor.locZ == caster.locZ && anchor.locX > caster.locX)
        //    {
        //        mydirection = GridController.Directions.left;
        //        inLine = true;
        //    }
        //    else if (anchor.locX < caster.locX && anchor.locZ < caster.locZ)
        //    {
        //        mydirection = GridController.Directions.up;
        //        helpDirection = GridController.Directions.right;
        //        inLine = false;
        //    }
        //    else if (anchor.locX < caster.locX && anchor.locZ > caster.locZ)
        //    {
        //        mydirection = GridController.Directions.down;
        //        helpDirection = GridController.Directions.right;
        //        inLine = false;
        //    }
        //    else if (anchor.locX > caster.locX && anchor.locZ < caster.locZ)
        //    {
        //        mydirection = GridController.Directions.up;
        //        helpDirection = GridController.Directions.left;
        //        inLine = false;
        //    }
        //    else if (anchor.locX > caster.locX && anchor.locZ > caster.locZ)
        //    {
        //        mydirection = GridController.Directions.down;
        //        helpDirection = GridController.Directions.left;
        //        inLine = false;
        //    }
        //    if (inLine == true)
        //    {
        //        for (int i = 0; i < spellCast.currentSpell.moveAwayFromTarget; i++)
        //        {
        //            Tile temp = gridController.GetTileInDirection(caster, 1, mydirection);
        //            if (temp.myType == Tile.BlockType.BaseBlock && temp != null)
        //            {
        //                PullPushAct(caster, temp);
        //                caster = gridController.GetTile(playerBehaviour.currentCharacter.currentTile);
        //            }
        //            else
        //            {
        //                break;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        for (int i = 0; i < spellCast.currentSpell.moveAwayFromTarget; i++)
        //        {
        //            Tile temp1 = gridController.GetTileInDirection(caster, 1, mydirection);
        //            Tile temp2 = gridController.GetTileInDirection(caster, 1, helpDirection);
        //            Tile temp3 = gridController.GetTileInDirection(gridController.GetTile(temp1.locX, temp1.locZ), 1, helpDirection);
        //            if (temp1.myType == Tile.BlockType.BaseBlock && temp2.myType == Tile.BlockType.BaseBlock && temp3.myType == Tile.BlockType.BaseBlock && temp1 != null && temp2 != null && temp3 != null)
        //            {
        //                PullPushAct(caster, temp3);
        //                caster = gridController.GetTile(playerBehaviour.currentCharacter.currentTile);
        //            }
        //            else
        //            {
        //                break;
        //            }
        //        }
        //    }
        //}
        //public void Tester()
        //{
        //    var derp = RangeType();
        //    Color ihana = new Color(1, 0, 1);

        //    foreach (var herp in derp)
        //    {
        //        herp.BaseMaterial.SetColor("wut", ihana);
        //    }
        //}
    }

}