using CharacterSystem;
using GameFieldSystem;
using ManagementSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellSystem
{
    public class SpellFormulaes : MonoBehaviour
    {
        // Variables for Pull/push Use
        public enum SpellPullPushType { LineTowardsPlayer, DiagonalTowardsPlayer, BothTowardsPlayer, LineTowardsMouse, DiagonalTowardsMouse, BothTowardsMouse, };
        public enum Side { up, down, left, right, upRight, upLeft, downRight, downLeft, sideError };

        public enum ManipulationType { casterTeleport , teleportSwich, walkTowards, walkAway};

        //references
        GridController gridController;
        MouseController mouseController;

        void Start()
        {
            mouseController = GameObject.FindGameObjectWithTag("MouseManager").GetComponent<MouseController>();
            gridController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridController>();

            if (!mouseController)
                Debug.LogWarning("Mousecontroller is null!");
        }

        // V V V switches places with target
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

        // V V V Teleports caster to target
        public void CasterTeleport(Tile caster, Tile target)
        {
            if (caster != null)
                Debug.Log("casteri löydetty");
            if(target != null)
                Debug.Log("casteri löydetty");
            PlayerMovement playerTeleport = null;
            playerTeleport = caster.CharCurrentlyOnTile.gameObject.GetComponent<PlayerMovement>();
            if (playerTeleport != null)
                Debug.Log("movement löydetty");
            if (playerTeleport && target.CharCurrentlyOnTile == false)
            {
                    Debug.Log("käydään tele");
                playerTeleport.MoveToTile(target, PlayerMovement.MovementMethod.Teleport);
            }
        }



        // V V V Handles Pull/ Push iniatilation
        public void SpellPullPush(SpellCreator.SpellAttribute spell, Tile caster, Tile target, Tile mouseTile)
        {
            //rajaus rajaa pushin ja pullin liike rataa
            int rajaus = 0;
            //side määrittää veto/työntö suunnan
            Side side;

            // Chooses what tiles are used in Pull/Push Actions
            switch (spell.mySpellPullPushType)
            {
                case SpellPullPushType.BothTowardsMouse:
                    side = WhichSide(mouseTile, target, spell.isItPull);
                    PullPushMotor(side, spell, target, rajaus);
                    break;
                case SpellPullPushType.BothTowardsPlayer:
                    side = WhichSide(caster, target, spell.isItPull);
                    PullPushMotor(side, spell, target, rajaus);
                    break;
                case SpellPullPushType.DiagonalTowardsMouse:
                    rajaus = 1;
                    side = WhichSide(mouseTile, target, spell.isItPull);
                    PullPushMotor(side, spell, target, rajaus);
                    break;
                case SpellPullPushType.DiagonalTowardsPlayer:
                    rajaus = 1;
                    side = WhichSide(caster, target, spell.isItPull);
                    PullPushMotor(side, spell, target, rajaus);
                    break;
                case SpellPullPushType.LineTowardsMouse:
                    rajaus = 2;
                    side = WhichSide(mouseTile, target, spell.isItPull);
                    PullPushMotor(side, spell, target, rajaus);
                    break;
                case SpellPullPushType.LineTowardsPlayer:
                    rajaus = 2;
                    side = WhichSide(caster, target, spell.isItPull);
                    PullPushMotor(side, spell, target, rajaus);
                    break;
            }

        }

        // V V V Pull/Push methods motor 
        public void PullPushMotor(Side tempSide, SpellCreator.SpellAttribute attribute, Tile start, int rajaaja)
        {
            // korjaus on temp tile
            Tile korjaus = start;
            //korjaus.locX = start.locX;
            //korjaus.locZ = start.locZ;

            // tempdir ja helpdir ovat tilapäisiä suuntia joilla ohjataan tarkistuksia
            GridController.Directions tempDir = GridController.Directions.none;
            GridController.Directions helpDir = GridController.Directions.none;

            // inline määrittää onko veto lineaarinen [+] vai diagonaalinen [X]
            bool inline = true;
            switch (tempSide)
            {
                case Side.up:
                    tempDir = GridController.Directions.down;
                    break;
                case Side.down:
                    tempDir = GridController.Directions.up;
                    break;
                case Side.left:
                    tempDir = GridController.Directions.right;
                    break;
                case Side.right:
                    tempDir = GridController.Directions.left;
                    break;
                case Side.upLeft:
                    tempDir = GridController.Directions.down;
                    helpDir = GridController.Directions.right;
                    inline = false;
                    break;
                case Side.upRight:
                    tempDir = GridController.Directions.down;
                    helpDir = GridController.Directions.left;
                    inline = false;
                    break;
                case Side.downLeft:
                    tempDir = GridController.Directions.up;
                    helpDir = GridController.Directions.right;
                    inline = false;
                    break;
                case Side.downRight:
                    tempDir = GridController.Directions.up;
                    helpDir = GridController.Directions.left;
                    inline = false;
                    break;
            }
            // kun veto on lineaarinen
            if (inline == true)
            {
                if (rajaaja == 0 || rajaaja == 2)
                {
                    // for luuppi käydään läpi vetojen verran
                    for (int i = 0; i < attribute.spellPullPush; i++)
                    {
                        Tile temp = gridController.GetTileInDirection(gridController.GetTile(korjaus.locX, korjaus.locZ), 1, tempDir);
                        if (temp.myType == Tile.BlockType.BaseBlock && temp.CharCurrentlyOnTile == false)
                        {
                            PullPushMove(korjaus, temp, PlayerMovement.MovementMethod.Teleport);
                            korjaus = temp;
                        }
                        else
                        {
                            Debug.Log("wall hit");
                            break;
                        }
                    }
                }

            }
            else
            {
                if (rajaaja == 0 || rajaaja == 1)
                {
                    for (int i = 0; i < attribute.spellPullPush; i++)
                    {
                        Tile temp1 = gridController.GetTileInDirection(gridController.GetTile(korjaus.locX, korjaus.locZ), 1, tempDir);
                        Tile temp2 = gridController.GetTileInDirection(gridController.GetTile(korjaus.locX, korjaus.locZ), 1, helpDir);
                        Tile temp3 = gridController.GetTileInDirection(gridController.GetTile(temp1.locX, temp1.locZ), 1, helpDir);
                        if (temp1.myType == Tile.BlockType.BaseBlock && temp1.CharCurrentlyOnTile == false && temp2.myType == Tile.BlockType.BaseBlock && temp2.CharCurrentlyOnTile == false && temp3.myType == Tile.BlockType.BaseBlock && temp3.CharCurrentlyOnTile == false)
                        {
                            PullPushMove(korjaus, temp3, PlayerMovement.MovementMethod.Teleport);
                            korjaus = temp3;
                            //item.locX = temp3.locX;
                            //item.locZ = temp3.locZ;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

            }
            inline = true;
        }

        // V V V Figures the side relation between two tiles
        public Side WhichSide(Tile start, Tile end, bool isItPull)
        {
            int x = start.locX - end.locX;
            int z = start.locZ - end.locZ;
            if (isItPull == false)
            {
                x *= -1;
                z *= -1;
            }


            if (x > 0)
            {
                if (z == 0)
                {
                    return Side.left;
                }
                if (z < 0)
                {
                    return Side.upLeft;
                }
                if (z > 0)
                {
                    return Side.downLeft;
                }
            }
            if (x < 0)
            {
                if (z == 0)
                {
                    return Side.right;
                }
                if (z < 0)
                {
                    return Side.upRight;
                }
                if (z > 0)
                {
                    return Side.downRight;
                }
            }
            if (x == 0)
            {
                if (z < 0)
                {
                    return Side.up;
                }
                if (z > 0)
                {
                    return Side.down;
                }
            }
            Debug.Log("shit, side error");
            return Side.sideError;
        }

        // V V V Gives orders to move
        public void PullPushMove(Tile start, Tile end, PlayerMovement.MovementMethod method)
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
                playerMovement.MoveToTile(end, method);  //Väliaikainen liikkuminen
            }
        }

        public void SpellWalkToAway(Tile target, Tile caster, SpellCreator.SpellAttribute attribute)
        {
            Side side;
            side = WhichSide(caster, target, attribute.moveTowards);

            GridController.Directions tempDir = GridController.Directions.none;

            switch (side)
            {
                case Side.up:
                    tempDir = GridController.Directions.up;
                    break;
                case Side.down:
                    tempDir = GridController.Directions.down;
                    break;
                case Side.left:
                    tempDir = GridController.Directions.left;
                    break;
                case Side.right:
                    tempDir = GridController.Directions.right;
                    break;
            }
            Tile korjaus = caster;

            // for luuppi käydään läpi vetojen verran
            for (int i = 0; i < attribute.movemenPoints; i++)
            {

                Tile temp = gridController.GetTileInDirection(gridController.GetTile(korjaus.locX, korjaus.locZ), 1, tempDir);
                if (temp.myType == Tile.BlockType.BaseBlock && temp.CharCurrentlyOnTile == false)
                {
                    PullPushMove(korjaus, temp, PlayerMovement.MovementMethod.Teleport);
                    korjaus = temp;
                }
                else
                {
                    Debug.Log("wall hit");
                    break;
                }
            }

        }
    }
}