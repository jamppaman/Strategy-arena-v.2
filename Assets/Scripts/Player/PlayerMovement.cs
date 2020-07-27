using GameFieldSystem;
using ManagementSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles the movement executing and validity checking. Movement happens only in cardinal directions.
/// </summary>

namespace CharacterSystem
{
    public class PlayerMovement : MonoBehaviour
    {

        public enum MovementMethod { NotSpecified, Teleport, Walk, Push };

        PlayerBehaviour playerController;
        public PlayerInfo playerInfo;
        GridController gridController;
        MouseController mouseController;
        AudioController aController;
        Tile _tile;
        public List<PathTile> pathTiles;
        public float movementSpeed;
        public bool unlimitedMovementPoints;

        /// <summary>
        /// Setting the CurrentTile property automatically subscribes the set tile to AnnounceTileChange event, and updates the playerInfo field in the said tile.
        /// No further additional setting in the tile part necessary, as this property handles the cross referencing by itself.
        /// </summary>

        public Tile CurrentTile
        {
            get
            {
                if (_tile == null)
                {
                    //Debug.Log("Haettiin tile jännästi, tee paremmin");
                    return gridController.GetTile((int)transform.localPosition.x, (int)transform.localPosition.z);
                }
                else
                {
                    return _tile;
                }
            }
            set
            {
                _tile = value;
                if (value != null)
                {
                    playerInfo.thisCharacter.currentTile = new PositionContainer(value.locX, value.locZ);
                    value.CharCurrentlyOnTile = playerInfo;
                }
                AnnounceTileChange(value);
            }
        }

        public UnityEvent exampleEvents;
        public delegate void TileEvent(Tile tile);
        public event TileEvent ChangeTile;

        /// <summary>
        /// Used in a doubly linked list, to keep track of the tiles in the path and for calculating the optimal route.
        /// </summary>

        public class PathTile
        {
            public Tile _tile;
            public Tile _destination;
            public List<Tile> _neighbours;
            public int? _distanceToTarget;
            public int _movementPointsLeft;
            public PathTile _previousTile;
            public PathTile _nextTile;

            public PathTile(Tile currentTile, Tile destination, int movementPointsLeft, PathTile previousTile)
            {
                _tile = currentTile;
                _destination = destination;
                _neighbours = GetWalkableTiles(_tile.GetTNeighbouringTiles());
                if (destination == null)
                    _distanceToTarget = null;
                else
                    _distanceToTarget = currentTile.GetCardinalDistance(destination);
                _movementPointsLeft = movementPointsLeft;
                _previousTile = previousTile;
                _nextTile = null;
            }

        }

        private void Start()
        {
            aController = GameObject.FindGameObjectWithTag("AudioController").GetComponent<AudioController>();
            gridController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridController>();
            if (!gridController)
                Debug.LogWarning("Gridcontroller is null!");
            mouseController = GameObject.FindGameObjectWithTag("MouseManager").GetComponent<MouseController>();
            if (!mouseController)
                Debug.LogWarning("Mousecontroller is null!");
            mouseController.currentMovement = this;
            if (!playerController)
                playerController = gameObject.GetComponentInParent<PlayerBehaviour>();
            if (!playerController)
                Debug.Log("Could not find playerbehaviour component in parents!");
            if (!playerInfo)
                playerInfo = gameObject.GetComponent<PlayerInfo>();
            if (!playerInfo)
                Debug.Log("Could not find playerinfo component");
            pathTiles = new List<PathTile>();
            Invoke("SetStartingTile", 1f);
            if (movementSpeed == 0)
            {
                movementSpeed = 3;      //Default arvon SAA MUUTTAA
            }
        }

        public List<Tile> TilesInRange()
        {

            return TilesInRange(CurrentTile, playerInfo.thisCharacter.currentMp, MovementMethod.Walk);
        }

        /// <summary>
        /// Returns all tiles that are in movement range, taking into account blockyblocks etc.
        /// </summary>
        //KESKEN
        public List<Tile> TilesInRange(Tile startTile, int movementPoints, MovementMethod method)
        {
            List<Tile> returnables = new List<Tile>();
            List<Tile> truereturnables = new List<Tile>();
            int movementLeft = movementPoints;

            switch (method)
            {
                case MovementMethod.Teleport:
                    List<Tile> lastIteration = new List<Tile>();
                    lastIteration.Add(startTile);
                    while (movementLeft > 0)
                    {
                        List<Tile> tempList = new List<Tile>();
                        foreach (var tile1 in lastIteration)
                        {
                            if (tile1 != null)
                            {
                                tempList = tempList.Union(tile1.GetTNeighbouringTiles()).ToList();
                                //Debug.Log("Returnables count: " + returnables.Count());
                            }
                        }
                        returnables = returnables.Union(tempList).ToList();
                        lastIteration = tempList;
                        movementLeft--;
                    }
                    break;

                case MovementMethod.Walk:
                    pathTiles = WithinWalkingDistance(startTile, movementPoints);
                    if (pathTiles != null)
                    {
                        foreach (var pathTile in pathTiles)
                        {
                            truereturnables.Add(pathTile._tile);
                        }
                    }
                    else
                    {
                        //Debug.Log("Path was null!");
                    }
                    break;

                default:
                    break;

            }
            foreach (var tile in returnables)
            {
                if (tile != null)
                {
                    if (tile.myType == Tile.BlockType.BaseBlock)
                        truereturnables.Add(tile);
                }
            }
            return truereturnables;
        }

        /// <summary>
        /// Returns the route by calculating back from destination. Starting tile should have null in the variable _previousTile
        /// </summary>

        public static List<Tile> CalculateRouteBack(PathTile destinationTile)
        {
            List<PathTile> orderedRoute = LinkListAndOrder(destinationTile);
            List<Tile> wishIHadMeatballs = new List<Tile>();
            foreach (var pathTile in orderedRoute)
            {
                wishIHadMeatballs.Add(pathTile._tile);
            }
            return wishIHadMeatballs;
        }

        /// <summary>
        /// When first creating the doubly linked list, only the previous tile is recorded. This function links the nextTile as well as reorders the list to start from the path's first tile.
        /// </summary>
        /// <param name="destinationTile"></param>
        /// <returns></returns>

        static List<PathTile> LinkListAndOrder(PathTile destinationTile)
        {
            List<PathTile> route = new List<PathTile>();
            route.Add(destinationTile);
            PathTile tempTile = destinationTile;
            PathTile nextTile = null;
            while (tempTile != null)
            {
                route.Add(tempTile);
                tempTile._nextTile = nextTile;
                nextTile = tempTile;
                tempTile = tempTile._previousTile;
            }
            List<PathTile> orderedRoute = route.OrderByDescending(x => x._movementPointsLeft).ToList();
            return orderedRoute;
        }

        /// <summary>
        /// The main method for moving! Walking will not work if the default value (null) for destinationPathTile is used, as the path is supplied with the said variable.
        /// </summary>
        /// <param name="destinationTile"></param>
        /// <param name="method"></param>
        /// <param name="destinationPathTile"></param>
        /// <returns></returns>

        public bool MoveToTile(Tile destinationTile, MovementMethod method, PathTile destinationPathTile = null)
        {

            switch (method)
            {
                case MovementMethod.NotSpecified:
                    Debug.Log("Movement method not selected!");
                    break;

                case MovementMethod.Teleport:
                    Teleport(destinationTile);
                    break;

                case MovementMethod.Walk:
                    if (destinationTile.myType == Tile.BlockType.BlockyBlock || destinationTile.CharCurrentlyOnTile != null)
                    {
                        return false;
                    }
                    if (destinationPathTile != null && destinationPathTile._tile == destinationTile)
                    {
                        List<PathTile> route = LinkListAndOrder(destinationPathTile);
                        StartCoroutine("Walk", route);
                    }

                    break;

                default:
                    Debug.Log("Error with movement method selection!");  //Not yet implemented?
                    break;

            }
            CurrentTile = destinationTile;
            return true;
        }

        /// <summary>
        /// First method created for movement. For a more flashy teleporting method, add particle effects!
        /// </summary>
        /// <param name="destinationTile"></param>

        void Teleport(Tile destinationTile)
        {
            transform.localPosition = destinationTile.transform.localPosition;
            CurrentTile = destinationTile;
        }

        /// <summary>
        /// Iterates through the given list of PathTiles and lerps character's position between them. Slows down on the last trip.
        /// Adjust the variable movementSpeed in the Start method (default value for all characters) or change it in unity editor for faster or slower characters.
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>

        IEnumerator Walk(List<PathTile> route)
        {
            aController.PlayMovementStartLoop(playerInfo.thisCharacter);
            foreach (var tile in route)
            {
                mouseController.stillMoving = true;
                //Are we there yet?
                if (tile._nextTile != null)
                {

                    Transform end = tile._nextTile._tile.transform;
                    float startTime = Time.time;

                    //Second to last tile = last trip -> slow down!
                    if (tile._nextTile._nextTile == null)
                    {
                        aController.SlowMovementLoop();
                        while (Vector3.Distance(transform.position, end.position) > 0.05f)
                        {
                            transform.position = Vector3.Lerp(transform.position, end.position, movementSpeed * Time.deltaTime);    //Slowing down

                            yield return null;
                        }
                    }
                    else
                    {
                        Transform start = transform;
                        float journeyLength = Vector3.Distance(start.position, end.position);
                        while (Vector3.Distance(transform.position, end.position) > 0.05f)
                        {
                            float distCovered = (Time.time - startTime) * movementSpeed;
                            float fracJourney = distCovered / journeyLength;
                            transform.position = Vector3.Lerp(start.position, end.position, fracJourney);
                            yield return null;
                        }

                    }
                }
            }
            mouseController.stillMoving = false;
            aController.PlayMovementStopLoop();
        }


        public void MyTurn()
        {
            mouseController.currentMovement = this;
        }

        /// <summary>
        /// Creates a list of reachable tiles using the starting point and current movement points. Each PathTile in the list is the final tile in a path.
        /// Only the previous tile is recorded.
        /// </summary>
        /// <param name="startTile"></param>
        /// <param name="movementPoints"></param>
        /// <returns></returns>

        private List<PathTile> WithinWalkingDistance(Tile startTile, int movementPoints)
        {
            if (startTile == null)
            {
                //Debug.Log("Starting tile for path is null!");
                return null;
            }
            PathTile startPathTile = new PathTile(startTile, null, movementPoints, null);
            List<PathTile> unprocessedTiles = null;
            List<PathTile> processedTiles = new List<PathTile>();
            unprocessedTiles = ProcessPathTile(startPathTile);
            while (unprocessedTiles != null && unprocessedTiles.Count > 0)
            {
                PathTile tempTile = unprocessedTiles[0];
                if (tempTile != null)
                {
                    List<PathTile> tempList = null;
                    if (Upsert(processedTiles, tempTile))
                    {
                        tempList = ProcessPathTile(tempTile);
                    }
                    unprocessedTiles.Remove(tempTile);
                    if (tempList != null)
                        unprocessedTiles.AddRange(tempList);
                }
            }
            return processedTiles;
        }

        /// <summary>
        /// Returns null if out of movement points. A* method not necessary for optimisation, as the map is small and no clear endpoint/target is presented.
        /// </summary>
        /// <param name="startTile"></param>
        /// <param name="dontUseAStar"></param>
        /// <returns></returns>

        private List<PathTile> ProcessPathTile(PathTile startTile, bool dontUseAStar = false)
        {
            List<PathTile> neighbours = null;
            if (startTile._movementPointsLeft > 0)
            {
                neighbours = CreatePathTileNeighbours(startTile);
                if (!dontUseAStar)
                {
                    //Do A* stuff
                }
            }
            return neighbours;
        }

        /// <summary>
        /// Used for route optimisation. Returns true if an existing tile was updated or a new one inserted instead of being rejected.
        /// Updates only if the new tile has a larger value in movementPointsLeft.
        /// </summary>
        /// <param name="targetList"></param>
        /// <param name="addedTile"></param>
        /// <returns></returns>

        private bool Upsert(List<PathTile> targetList, PathTile addedTile)
        {
            var sameTile = targetList.Where(x => x._tile == addedTile._tile).FirstOrDefault();
            if (sameTile != null)
            {
                if (addedTile._movementPointsLeft > sameTile._movementPointsLeft)
                {
                    sameTile._movementPointsLeft = addedTile._movementPointsLeft;
                    return true;    //Existing PathTile was updated
                }
                else
                {
                    return false;   //Returning false should be captured and tile's neighbours should not be reprocessed!
                }
            }
            targetList.Add(addedTile);
            return true;    //Added missing PathTile to list
        }

        /// <summary>
        /// Returns a list of neighbouring tiles as PathTiles, that has 1 movement subtracted when compared to the given currentTile parameter.
        /// </summary>
        /// <param name="currentTile"></param>
        /// <returns></returns>

        private List<PathTile> CreatePathTileNeighbours(PathTile currentTile)
        {
            List<PathTile> pathTiles = new List<PathTile>();
            foreach (var tile in currentTile._neighbours)
            {
                PathTile pathTile = new PathTile(tile, currentTile._destination, currentTile._movementPointsLeft - 1, currentTile);
                pathTiles.Add(pathTile);
            }
            return pathTiles;
        }

        /// <summary>
        /// An event that is called, when a new currentTile is set.
        /// </summary>
        /// <param name="tile"></param>

        void AnnounceTileChange(Tile tile)
        {
            if (ChangeTile != null /*&& tile != null*/)
            {
                ChangeTile(tile);
            }
        }

        /// <summary>
        /// The exampleEvents list set in unity editor is invoked when this method is called. Sometimes useful for debugging and testing.
        /// </summary>

        public void ExampleEventsForEditor()
        {
            exampleEvents.Invoke();
        }

        public static List<Tile> GetWalkableTiles(List<Tile> sourceTiles)
        {
            List<Tile> tiles = new List<Tile>();
            foreach (var tile in sourceTiles)
            {
                //if (tile.WalkThrough && tile.isFree)
                if (tile.myType == Tile.BlockType.BaseBlock && tile.CharCurrentlyOnTile == false)
                {
                    tiles.Add(tile);
                }
            }
            return tiles;
        }

        void SetStartingTile()
        {
            //CurrentTile = gridController.GetTile(playerInfo.thisCharacter.currentTile);       //Käytetään tätä sitten, kun charactervalueseilla on järkevät aloitustiedot
            CurrentTile = gridController.GetTile((int)transform.position.x, (int)transform.position.z);   //Väliaikainen homma
        }


#if UNITY_EDITOR
        //using UnityEditor;

        [UnityEditor.CustomEditor(typeof(PlayerMovement))]
        public class MovementButtonEditor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();

                PlayerMovement PlayerMovementScript = (PlayerMovement)target;
                if (GUILayout.Button("Invoke example events"))
                {
                    PlayerMovementScript.ExampleEventsForEditor();
                }
            }
        }
#endif

    }
}