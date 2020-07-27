using ManagementSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIsystem
{
    public class TurnTimelineController : MonoBehaviour
    {

        public List<GameObject> blocks;
        private TurnManager tManager;
       // private int currentArrow = 0;

        void Start()
        {
            tManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<TurnManager>();
            tManager.TurnChange += HandleTurnChange;
            //Get sprites
            foreach (GameObject block in blocks)
            {
                block.GetComponent<Image>().sprite = block.GetComponent<BlockInfo>().character.portrait;
                //Disable arrows
                DisableArrows();
            }
        }
        private void OnDestroy()
        {
            tManager.TurnChange -= HandleTurnChange;
        }

        private void HandleTurnChange(PlayerInfo player)
        {
            MoveArrow(player.thisCharacter);
        }

        public void MoveArrow(CharacterSystem.CharacterValues character)
        {
            DisableArrows();

            foreach (GameObject block in blocks)
            {
                if (block.GetComponent<BlockInfo>().character)
                {
                    if (block.GetComponent<BlockInfo>().character == character)
                    {
                        block.GetComponent<BlockInfo>().arrow.gameObject.SetActive(true);
                    }
                }
            }
        }

        public void DisableArrows()
        {
            foreach (GameObject block in blocks)
            {
                block.GetComponent<BlockInfo>().arrow.gameObject.SetActive(false);
            }
        }

        public void RemoveBlock(CharacterSystem.CharacterValues character)
        {
            foreach (GameObject block in blocks)
            {
                if (block.GetComponent<BlockInfo>().character)
                {
                    if (block.GetComponent<BlockInfo>().character == character)
                    {
                        block.SetActive(false);
                    }
                }
            }
        }


    }

}