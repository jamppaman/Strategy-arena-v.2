using CharacterSystem;
using GameFieldSystem;
using SpellSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace ManagementSystem
{
    public class MouseController : MonoBehaviour
    {

        public Tile selected; // <- This is hover tile
        PlayerMovement.PathTile huijaus;
        List<Tile> tileList;
        public GameObject hitObject;


        Tile tile;
        GridController gridController;
        TurnManager turnManager;
        PlayerBehaviour playerBehaviour;
        SpellButtons spellButtons;
        SpellHandler spellHandler;
        SpellCasting spellCasting;
        SpellChecker spellChecker;

        public Material hovermaterial;
        public Material targetMaterial;
        public Material rangeMaterial;
        public Material rangeNullMaterial;
        public Material movementMaterial;
        public Material pathMaterial;
        public Material pathSelectedMaterial;
        private Tile previousTile;
        public List<Tile> targetedTiles = null;
        public List<Tile> rangeTiles = null;
        public List<Tile> nullTiles;
        List<Tile> movementRangeTiles;
        List<Tile> tilesToBeReset;
        List<Tile> tilesInPath;


        public PlayerMovement currentMovement;
        private bool rangeTilesPainted;
        private bool movementEnabled;
        public bool stillMoving;

        void Start()
        {
            GameObject tempGO1 = GameObject.FindGameObjectWithTag("GameController");
            if (tempGO1)
            {
                gridController = tempGO1.GetComponent<GridController>();
                turnManager = tempGO1.GetComponent<TurnManager>();
            }
            GameObject tempGO2 = GameObject.FindGameObjectWithTag("PlayerController");
            if (tempGO2)
            {
                playerBehaviour = tempGO2.GetComponent<PlayerBehaviour>();
                spellButtons = tempGO2.GetComponent<SpellButtons>();
                spellHandler = tempGO2.GetComponent<SpellHandler>();
                spellCasting = tempGO2.GetComponent<SpellCasting>();
                spellChecker = tempGO2.GetComponent<SpellChecker>();

            }
            tilesToBeReset = new List<Tile>();
            movementRangeTiles = new List<Tile>();
            SubscribtionOn();
            movementEnabled = true;
            stillMoving = false;
            rangeTiles = null;
        }

        private void OnDestroy()
        {
            SubscribtionOff();
        }

        void SubscribtionOn()
        {
            turnManager.TurnChange += HandlePlayerChange;
        }

        void SubscribtionOff()
        {
            turnManager.TurnChange -= HandlePlayerChange;
        }


        void Update()
        {

            //Mouse/ray setup

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;


            if (Physics.Raycast(ray, out hitInfo))
            {
                hitObject = hitInfo.transform.gameObject;

                if (hitObject.CompareTag("Tile"))
                {
                    //if (selected)
                    //    selected.GetComponent<Renderer>().material = selected.BaseMaterial;
                    selected = hitObject.gameObject.GetComponent<Tile>();

                    if (selected.myType == Tile.BlockType.BaseBlock && selected != previousTile)
                    {
                        // Debug.Log("Tämä on baseblock");

                        if (previousTile)
                        {
                            Renderer pr = previousTile.GetComponent<Renderer>();
                            pr.material = previousTile.GetComponent<Tile>().BaseMaterial;
                            previousTile = null;
                            if (targetedTiles != null)
                            {
                                ResetTileMaterials(targetedTiles);
                                targetedTiles = null;
                                ChangeTileMaterials(rangeTiles, rangeMaterial);
                                ChangeTileMaterials(nullTiles, rangeNullMaterial);
                            }
                        }
                        previousTile = selected;
                        Renderer sr = selected.GetComponent<Renderer>();
                        sr.material = hovermaterial;
                    }

                }
                //Jos hovertile on jotain muuta, resettaa hovertilet
                if (!hitObject.CompareTag("Tile") || (hitObject.CompareTag("Tile") && selected.myType != Tile.BlockType.BaseBlock))
                {
                    if (previousTile)
                    {
                        Renderer pr = previousTile.GetComponent<Renderer>();
                        pr.material = previousTile.GetComponent<Tile>().BaseMaterial;
                        previousTile = null;

                        if (targetedTiles != null)
                        {
                            ResetTileMaterials(targetedTiles);
                            targetedTiles = null;
                            ChangeTileMaterials(rangeTiles, rangeMaterial);
                            ChangeTileMaterials(nullTiles, rangeNullMaterial);
                        }
                    }
                }
            }

            if (hitInfo.transform == null)
            {
                selected = null;
            }

            // kun spell nappulaa on painettu
            if (spellButtons.spellOpen == true)
            {
                if (currentMovement)
                {
                    ResetTileMaterials(tilesToBeReset);
                    tilesToBeReset.Clear();
                    rangeTilesPainted = false;
                }


                //pitäisi maalata target range
                if (rangeTiles == null)
                {
                    if (spellButtons.currentSpell.needLineOfSight == true)
                    {
                        nullTiles = spellHandler.RangeType(spellButtons.currentSpell.rangeType, true);
                        foreach (var tile in nullTiles)
                        {
                            Renderer aR = tile.GetComponent<Renderer>();
                            aR.material = rangeNullMaterial;
                        }

                        rangeTiles = spellHandler.RangeType(spellButtons.currentSpell.rangeType, false);
                        foreach (var tile in rangeTiles)
                        {
                            Renderer aR = tile.GetComponent<Renderer>();
                            aR.material = rangeMaterial;
                        }
                    }
                    else
                    {
                        rangeTiles = spellHandler.RangeType(spellButtons.currentSpell.rangeType, false);
                        foreach (var tile in rangeTiles)
                        {
                            Renderer aR = tile.GetComponent<Renderer>();
                            aR.material = rangeMaterial;
                        }
                    }

                }

                //pitäisi maalata AOE tilet
                targetedTiles = spellHandler.AreaType(spellButtons.currentSpell.areaType);
                foreach (var tile in rangeTiles)
                {
                    if (tile == selected)
                    {
                        ChangeTileMaterials(targetedTiles, targetMaterial);
                    }
                }

                //kun spell castataan
                if (Input.GetMouseButtonDown(0) && !stillMoving && spellButtons.currentSpell != null)
                {

                    if (spellChecker.CheckCastability(spellButtons.currentSpell, selected) == true)
                    {
                        if (targetedTiles != null)
                        {
                        spellCasting.CastSpell(spellButtons.currentSpell, playerBehaviour.currentCharacter, selected);
                        foreach (var tile in rangeTiles)
                            {
                                Renderer tr = tile.GetComponent<Renderer>();
                                tr.material = tile.GetComponent<Tile>().BaseMaterial;
                            }
                            rangeTiles = null;
                            foreach (var target in targetedTiles)
                            {
                                Renderer ar = target.GetComponent<Renderer>();
                                ar.material = target.GetComponent<Tile>().BaseMaterial;
                            }
                            targetedTiles = null;
                            spellCasting.Aftermath();
                        }
                        else
                        {
                            spellButtons.SpellCancel();
                        }
                    }
                    else
                    {
                        Debug.Log("cannot be cast");
                        spellButtons.SpellCancel();
                    }
                }


                    // spell cansellataan
                    if (Input.GetMouseButtonDown(1))
                {
                    if (rangeTiles != null)
                    {
                        ResetTileMaterials(rangeTiles);
                        rangeTiles = null;
                    }
                    if (targetedTiles != null)
                    {
                        ResetTileMaterials(targetedTiles);
                        targetedTiles = null;
                    }
                    spellButtons.SpellCancel();
                }
                movementEnabled = false;
            }
            else
            {
                movementEnabled = true;
            }

            if (movementEnabled)
            {
                if (!currentMovement)
                {
                    Debug.Log("Movement still not set!");
                }

                if (!rangeTilesPainted || selected == null)
                {
                    ResetTileMaterials(tilesToBeReset);
                    tilesToBeReset.Clear();
                    movementRangeTiles = currentMovement.TilesInRange();
                    ChangeTileMaterials(movementRangeTiles, movementMaterial);
                    tilesToBeReset.AddRange(movementRangeTiles);
                    rangeTilesPainted = true;
                }

                ChangeTileMaterials(movementRangeTiles, movementMaterial);  //otherwise hovering over resets the tiles
                if (movementRangeTiles.Contains(selected))
                {
                    //Hae reitti ja maalaa se
                    PlayerMovement.PathTile tempTest = currentMovement.pathTiles.Where(x => x._tile == selected).FirstOrDefault();
                    if (tempTest != null)
                    {
                        tilesInPath = PlayerMovement.CalculateRouteBack(tempTest);
                        ChangeTileMaterials(tilesInPath, pathMaterial);
                        Renderer sr = selected.GetComponent<Renderer>();
                        sr.material = pathSelectedMaterial;
                        huijaus = tempTest;
                    }
                    else
                    {
                        huijaus = null;
                    }
                }

                if (Input.GetMouseButtonDown(0)
                        && !stillMoving
                        && selected != currentMovement.CurrentTile
                        && movementRangeTiles.Contains(selected))
                {
                    if (currentMovement.MoveToTile(selected, PlayerMovement.MovementMethod.Walk, huijaus))
                    {
                        if (!currentMovement.unlimitedMovementPoints)
                        {
                            currentMovement.playerInfo.thisCharacter.currentMp -= (tilesInPath.Count() - 2);
                        }
                    }

                    ResetTileMaterials(tilesToBeReset);
                    tilesToBeReset.Clear();
                    rangeTilesPainted = false;
                    spellButtons.UpdateHpApMp();
                }
            }
        }

        public void ResetTileMaterials(List<Tile> tileList)
        {
            foreach (var tile in tileList)
            {
                Renderer ar = tile.GetComponent<Renderer>();
                ar.material = tile.GetComponent<Tile>().BaseMaterial;
            }
        }

        void ChangeTileMaterials(List<Tile> tileList, Material material)
        {
            if (tileList != null && tileList.Count > 0)
            {
                foreach (var tile in tileList)
                {
                    if (Tile.BlockType.BaseBlock == tile.myType)
                    {
                        Renderer pr = tile.GetComponent<Renderer>();
                        pr.material = material;
                    }
                }
            }
        }

        void HandlePlayerChange(PlayerInfo player)
        {
            currentMovement = player.gameObject.GetComponent<PlayerMovement>();
            movementEnabled = true;
            stillMoving = false;
            rangeTilesPainted = false;
        }

    }
}