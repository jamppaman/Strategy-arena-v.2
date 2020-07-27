using CharacterSystem;
using ManagementSystem;
using System.Collections;
using System.Collections.Generic;
using UIsystem;
using UnityEngine;
using UnityEngine.UI;

namespace SpellSystem
{
    public class SpellButtons : MonoBehaviour
    {
        //references
        public MouseController mc;
        public CharacterValues cv;
        TurnManager turnManager;
        public SpellChecker spellChecker;
        public PlayerBehaviour playerBehaviour;

        //variables
        public SpellCreator currentSpell;
        public bool spellOpen = false;
        public Button spellButton1, spellButton2, spellButton3, spellButton4, spellButton5, spellButton6;
        public Text hpText, apText, mpText;

        void Start()
        {

            spellButton1 = spellButton1.GetComponent<Button>();
            spellButton1.onClick.AddListener(Spell1Cast);
            spellButton2 = spellButton2.GetComponent<Button>();
            spellButton2.onClick.AddListener(Spell2Cast);
            spellButton3 = spellButton3.GetComponent<Button>();
            spellButton3.onClick.AddListener(Spell3Cast);
            spellButton4 = spellButton4.GetComponent<Button>();
            spellButton4.onClick.AddListener(Spell4Cast);
            spellButton5 = spellButton5.GetComponent<Button>();
            spellButton5.onClick.AddListener(Spell5Cast);
            spellButton6 = spellButton6.GetComponent<Button>();
            spellButton6.onClick.AddListener(Spell6Cast);

            turnManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<TurnManager>();
            spellChecker = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<SpellChecker>();
            playerBehaviour = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerBehaviour>();
            turnManager.TurnChange += HandleTurnChange;
            turnManager.TurnEnd += HandleTurnEnd;
        }

        void Update()
        {   
            // starting spells if 1-6 are pressed
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Spell1Cast();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Spell2Cast();
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Spell3Cast();
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                Spell4Cast();
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                Spell5Cast();
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                Spell6Cast();
            }
        }

        //prepping casting of spells 1-6
        public void Spell1Cast()
        {
            SpellCancel();
            currentSpell = cv.spell_1;
            spellOpen = true;
        }
        public void Spell2Cast()
        {
            SpellCancel();
            currentSpell = cv.spell_2;
            spellOpen = true;
        }
        public void Spell3Cast()
        {
            SpellCancel();
            currentSpell = cv.spell_3;
            spellOpen = true;
        }
        public void Spell4Cast()
        {
            SpellCancel();
            currentSpell = cv.spell_4;
            spellOpen = true;
        }
        public void Spell5Cast()
        {
            SpellCancel();
            currentSpell = cv.spell_5;
            spellOpen = true;
        }
        public void Spell6Cast()
        {
            SpellCancel();
            currentSpell = cv.spell_6;
            spellOpen = true;
        }

        // canceling spells
        public void SpellCancel()
        {
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
            currentSpell = null;
            spellOpen = false;
        }

        private void OnDestroy()
        {
            turnManager.TurnChange -= HandleTurnChange;
            turnManager.TurnEnd -= HandleTurnEnd;
        }

        #region EventHandlers

        private void HandleTurnChange(PlayerInfo player)
        {
            cv = player.thisCharacter;
            //Debug.Log("Handling event");
            spellButton1.GetComponent<Image>().sprite = cv.spell_1.spellIcon;
            spellButton2.GetComponent<Image>().sprite = cv.spell_2.spellIcon;
            spellButton3.GetComponent<Image>().sprite = cv.spell_3.spellIcon;
            spellButton4.GetComponent<Image>().sprite = cv.spell_4.spellIcon;
            spellButton5.GetComponent<Image>().sprite = cv.spell_5.spellIcon;
            spellButton6.GetComponent<Image>().sprite = cv.spell_6.spellIcon;

            spellButton1.GetComponent<Tooltip>().spell = cv.spell_1;
            spellButton2.GetComponent<Tooltip>().spell = cv.spell_2;
            spellButton3.GetComponent<Tooltip>().spell = cv.spell_3;
            spellButton4.GetComponent<Tooltip>().spell = cv.spell_4;
            spellButton5.GetComponent<Tooltip>().spell = cv.spell_5;
            spellButton6.GetComponent<Tooltip>().spell = cv.spell_6;

            //////////HandleCooldownDecrease(cv.spell_1);
            //////////HandleCooldownDecrease(cv.spell_2);
            //////////HandleCooldownDecrease(cv.spell_3);
            //////////HandleCooldownDecrease(cv.spell_4);
            //////////HandleCooldownDecrease(cv.spell_5);
            //////////HandleCooldownDecrease(cv.spell_6);

            playerBehaviour.UpdateTabs();
            UpdateHpApMp();
        }

        private void HandleTurnEnd(PlayerInfo player)
        {
            //////////DisableButtonsIfNotAp();
        }

        #endregion

        // makes buttons noninterractable if the spell it is tied to is too expensive to use
        public void DisableButtonsIfNotAp()
        {
            if (cv.spell_1.spellApCost > cv.currentAp || spellChecker.SpellCooldownCheck(cv.spell_1) == false)
            {
                spellButton1.interactable = false;
            }
            else
            {
                spellButton1.interactable = true;
            }
            if (cv.spell_2.spellApCost > cv.currentAp || spellChecker.SpellCooldownCheck(cv.spell_2) == false)
            {
                spellButton2.interactable = false;

            }
            else
            {
                spellButton2.interactable = true;

            }
            if (cv.spell_3.spellApCost > cv.currentAp || spellChecker.SpellCooldownCheck(cv.spell_3) == false)
            {
                spellButton3.interactable = false;

            }
            else
            {
                spellButton3.interactable = true;

            }
            if (cv.spell_4.spellApCost > cv.currentAp || spellChecker.SpellCooldownCheck(cv.spell_4) == false)
            {
                spellButton4.interactable = false;

            }
            else
            {
                spellButton4.interactable = true;

            }
            if (cv.spell_5.spellApCost > cv.currentAp || spellChecker.SpellCooldownCheck(cv.spell_5) == false)
            {
                spellButton5.interactable = false;

            }
            else
            {
                spellButton5.interactable = true;

            }
            if (cv.spell_6.spellApCost > cv.currentAp || spellChecker.SpellCooldownCheck(cv.spell_6) == false)
            {
                spellButton6.interactable = false;

            }
            else
            {
                spellButton6.interactable = true;

            }

            //Tabs
            if (playerBehaviour.GetTab(cv) != null && cv.spell_1.spellCooldownLeft > 1)
            {
                playerBehaviour.GetTab(cv).spell1.GetComponent<Button>().interactable = false;
            }
            else
            {
                if (playerBehaviour.GetTab(cv) != null)
                {
                    playerBehaviour.GetTab(cv).spell1.GetComponent<Button>().interactable = true;
                }
            }

            if (playerBehaviour.GetTab(cv) != null && cv.spell_2.spellCooldownLeft > 1)
            {
                playerBehaviour.GetTab(cv).spell2.GetComponent<Button>().interactable = false;
            }
            else
            {
                if (playerBehaviour.GetTab(cv) != null)
                {
                    playerBehaviour.GetTab(cv).spell2.GetComponent<Button>().interactable = true;
                }
            }

            if (playerBehaviour.GetTab(cv) != null && cv.spell_3.spellCooldownLeft > 1)
            {
                playerBehaviour.GetTab(cv).spell3.GetComponent<Button>().interactable = false;
            }
            else
            {
                if (playerBehaviour.GetTab(cv) != null)
                {
                    playerBehaviour.GetTab(cv).spell3.GetComponent<Button>().interactable = true;
                }
            }

            if (playerBehaviour.GetTab(cv) != null && cv.spell_4.spellCooldownLeft > 1)
            {
                playerBehaviour.GetTab(cv).spell4.GetComponent<Button>().interactable = false;
            }
            else
            {
                if (playerBehaviour.GetTab(cv) != null)
                {
                    playerBehaviour.GetTab(cv).spell4.GetComponent<Button>().interactable = true;
                }
            }

            if (playerBehaviour.GetTab(cv) != null && cv.spell_5.spellCooldownLeft > 1)
            {
                playerBehaviour.GetTab(cv).spell5.GetComponent<Button>().interactable = false;
            }
            else
            {
                if (playerBehaviour.GetTab(cv) != null)
                {
                    playerBehaviour.GetTab(cv).spell5.GetComponent<Button>().interactable = true;
                }
            }

            if (playerBehaviour.GetTab(cv) != null && cv.spell_6.spellCooldownLeft > 1)
            {
                playerBehaviour.GetTab(cv).spell6.GetComponent<Button>().interactable = false;
            }
            else
            {
                if (playerBehaviour.GetTab(cv) != null)
                {
                    playerBehaviour.GetTab(cv).spell6.GetComponent<Button>().interactable = true;
                }
            }
        }

        // updates hp ap and mp in UI
        public void UpdateHpApMp()
        {
            hpText.text = "HP: " + cv.currentHP + " / " + cv.currentMaxHP;
            apText.text = "AP: " + cv.currentAp;
            mpText.text = "MP: " + cv.currentMp;
            DisableButtonsIfNotAp();
        }
    }

}
