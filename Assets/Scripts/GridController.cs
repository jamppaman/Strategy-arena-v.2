using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace GameFieldSystem
{
    public class GridController : MonoBehaviour
    {

        public enum Directions { none, left, up, right, down };

        public GameObject tiles;
        private Tile[] tileList;
        private List<List<Tile>> tileGrid;

        void Start()
        {
            tileList = tiles.GetComponentsInChildren<Tile>();
            tileGrid = new List<List<Tile>>(); // Initialize overlist
            for (int j = 0; j < 24; j++)
            { // Initialize each list
                List<Tile> tempList = new List<Tile>();
                for (int z = 0; z < 24; z++)
                {
                    tempList.Add(null); // Initialize each list
                }
                tileGrid.Add(tempList);
            }
            StartCoroutine("InitializeGrid");
            //Tile test = GetTile(4, 6);
            //test.transform.position += new Vector3(2f, 2f, 2f);
        }

        /// <summary>
        /// Looping makes sure, that we do not go out of range in the lists. Maybe you fix it yes kind sir?
        /// </summary>
        /// <returns></returns>
        IEnumerator InitializeGrid()
        {
            for (int i = 0; i < tileList.Length; i++)
            {
                bool meatballs = true;
                while (meatballs)
                {
                    if (tileList[i].locX == 0 || tileList[i].locZ == 0)
                    {
                        yield return new WaitForSeconds(0.001f);
                    }
                    else
                    {
                        meatballs = false;  //The index will not be negative now! Hooray!
                    }
                }
                tileGrid[tileList[i].locX - 1][tileList[i].locZ - 1] = tileList[i];
            }
        }

        //USE THIS TO CALL GRID!
        public Tile GetTile(int xCord, int zCord)
        {
            if (xCord > 24 || zCord > 24 || zCord < 1 || xCord < 1)
                return null;
            else
                return tileGrid[xCord - 1][zCord - 1];
        }

        public Tile GetTile(PositionContainer container)
        {
            if (container != null)
                return GetTile(container.x, container.z);
            else
            {
                Debug.Log("container is null!");
                return null;
            }
        }

        public List<Tile> GetTilesNextTo(int xCord, int zCord)
        {
            List<Tile> palautus = new List<Tile>();
            Tile tempTile;
            tempTile = GetTile(xCord + 1, zCord);
            if (tempTile != null) palautus.Add(tempTile);
            tempTile = GetTile(xCord - 1, zCord);
            if (tempTile != null) palautus.Add(tempTile);
            tempTile = GetTile(xCord, zCord + 1);
            if (tempTile != null) palautus.Add(tempTile);
            tempTile = GetTile(xCord, zCord - 1);
            if (tempTile != null) palautus.Add(tempTile);
            return palautus;
        }

        public List<Tile> GetTilesInLinearDirection(Tile startTile, int range, Directions direction)
        {
            List<Tile> palautus = new List<Tile>();
            Tile tempTile;
            int rangeLeft = range;
            bool acceptedTile = true;
            int counter = 0;
            while (rangeLeft > 0 && acceptedTile)
            {
                counter++;
                tempTile = GetTileInDirection(startTile, counter, direction);
                if (tempTile != null)
                {
                    palautus.Add(tempTile);
                }
                else
                {
                    acceptedTile = false;
                }
            }

            return palautus;

        }

        /// <summary>
        /// Distance 1 means the tile next to the starting tile.
        /// </summary>
        /// <param name="startTile"></param>
        /// <param name="direction"></param>
        /// <param name="distance"></param>
        /// <returns></returns>

        public Tile GetTileInDirection(Tile startTile, int distance, Directions direction)
        {
            Tile tempTile = null;
            switch (direction)
            {
                case Directions.left:
                    tempTile = GetTile(startTile.locX - distance, startTile.locZ);
                    break;
                case Directions.right:
                    tempTile = GetTile(startTile.locX + distance, startTile.locZ);
                    break;
                case Directions.up:
                    tempTile = GetTile(startTile.locZ + distance, startTile.locX);
                    break;
                case Directions.down:
                    tempTile = GetTile(startTile.locZ - distance, startTile.locX);
                    break;

                default:
                    break;

            }

            return tempTile;
        }

        void Update()
        {

        }
    }

}