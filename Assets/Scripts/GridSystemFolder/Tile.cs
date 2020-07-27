using CharacterSystem;
using ManagementSystem;
using SpellSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFieldSystem
{

    public class Tile : MonoBehaviour
    {
        public GameObject targetyTile;
        public int locX;
        public int locZ;
        public PositionContainer positionContainer;

        public Material ShootThroughBlockMaterial;
        public Material BlockyBlockMaterial;
        private Material thisMaterial;
        public Material GridHoverMaterial;
        public Material BaseMaterial;
        public Material TargetMaterial;
        public Material RangeMaterial;
        public Material MovementMaterial;
        GridController gridController;
        TeamManager teamManager;
        List<Tile> tileList;
        //PlayerBehaviour playerBehaviour;

        PlayerInfo _charCurrentlyOnTile;
        public PlayerInfo CharCurrentlyOnTile
        {
            get
            {
                return _charCurrentlyOnTile;
            }
            set
            {
                if (_charCurrentlyOnTile != null)
                {
                    var playerMovement = _charCurrentlyOnTile.GetComponent<PlayerMovement>();
                    playerMovement.ChangeTile -= HandleChangeTile;
                }
                _charCurrentlyOnTile = value;
                if (value != null)
                {
                    var playerMovement = _charCurrentlyOnTile.GetComponent<PlayerMovement>();
                    playerMovement.ChangeTile += HandleChangeTile;
                }
            }
        }

        public bool isFree;
        public bool ShootThrough;
        public bool Targetable;
        public bool WalkThrough;
        public enum BlockType { BaseBlock, ShootThroughBlock, BlockyBlock, StartA, StartB };
        public BlockType myType;

        // Use this for initialization
        void Start()
        {
            gridController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridController>();
            teamManager = gridController.gameObject.GetComponent<TeamManager>();
            locX = (int)transform.localPosition.x;
            locZ = (int)transform.localPosition.z;
            thisMaterial = GetComponent<Renderer>().material;
            BaseMaterial = GetComponent<Renderer>().material;
            isFree = true;
            switch (myType)
            {
                case BlockType.BaseBlock:
                    ShootThrough = true;
                    Targetable = true;
                    WalkThrough = true;
                    break;

                case BlockType.ShootThroughBlock:
                    ShootThrough = true;
                    Targetable = false;
                    WalkThrough = false;
                    thisMaterial.color = ShootThroughBlockMaterial.color;
                    //GetComponent<Renderer>().gameObject.SetActive(false);
                    break;

                case BlockType.BlockyBlock:
                    ShootThrough = false;
                    Targetable = false;
                    WalkThrough = false;
                    thisMaterial.color = BlockyBlockMaterial.color;
                    transform.localScale += new Vector3(0.1f, 0.55f, 0.1f);
                    transform.position += new Vector3(0, 0.3f, 0);
                    break;
            }
        }

        private void HandleChangeTile(Tile tile)
        {
            if (tile != this)
            {
                CharCurrentlyOnTile = null;
            }
        }

        /// <summary>
        /// No diagonal movement, so cardinal distance is more useful gamewise
        /// </summary>

        public int GetCardinalDistance(Tile other)
        {
            int differencex = Mathf.Abs(other.locX - locX);
            int diffenercez = Mathf.Abs(other.locZ - locZ);
            return differencex + diffenercez;
        }

        /// <summary>
        /// Used for astar
        /// </summary>

        private float GetRealDistance(Tile other)
        {
            int differencex = Mathf.Abs(other.locX - locX);
            int diffenercez = Mathf.Abs(other.locZ - locZ);
            return Mathf.Sqrt(differencex ^ 2 + diffenercez ^ 2);
        }

        //public List<Tile> GetRoute(Tile other)
        //{
        //    int maxDistance = GetCardinalDistance(other);
        //    List<Tile> route = new List<Tile>();
        //    return route;
        //}

        public Tile GetOneCloserBruteForce(Tile other, bool checkIfAvailable)
        {
            Tile bestTile = null;
            float shortestDistance = 999;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    Tile testTile = gridController.GetTile(locX + i, locZ + j);
                    if (testTile == null)
                        continue;  //skips iteration, as such tile is out of bounds
                    if (testTile.CheckAvailability())

                        if (bestTile != null)
                        {
                            float temp = testTile.GetRealDistance(other);
                            if (temp < shortestDistance)
                            {
                                shortestDistance = temp;
                                bestTile = testTile;
                            }
                        }
                        else
                        {
                            bestTile = testTile;
                            shortestDistance = testTile.GetRealDistance(other);
                        }
                }
            }
            return bestTile;
        }

        public bool CheckAvailability()
        {
            if (WalkThrough && isFree)
                return true;
            return false;
        }

        public List<Tile> GetTNeighbouringTiles()
        {
            return (gridController.GetTilesNextTo(locX, locZ));
        }

    }

}