using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CharacterSystem;

namespace UIsystem
{
    public class CharacterTab : MonoBehaviour
    {

        public static int tabNumber;
        public Text tName;
        public Text tHealth;
        public Text tAp;
        public Text tMp;
        public GameObject spell1;
        public GameObject spell2;
        public GameObject spell3;
        public GameObject spell4;
        public GameObject spell5;
        public GameObject spell6;
        public GameObject effectBlock;
        public Image characterIcon;
        public Image highlight;
        public Image deathMask;
        PlayerBehaviour _player;
        bool isMyPlayerActive;
        public CharacterValues characterVal;
        public GameObject healthBar;
        public GameObject panel;

        //Muokataan kaikkia PlayerBehaviourissa

        void Start()
        {
            //Subscribe();

            if (characterVal)
            {
                if (characterVal.portrait)
                {
                    characterIcon.sprite = characterVal.portrait;
                }
                UpdateSpellIcons();
                UpdateInfo();
                spell1.GetComponent<Tooltip>().spell = characterVal.spell_1;
                spell2.GetComponent<Tooltip>().spell = characterVal.spell_2;
                spell3.GetComponent<Tooltip>().spell = characterVal.spell_3;
                spell4.GetComponent<Tooltip>().spell = characterVal.spell_4;
                spell5.GetComponent<Tooltip>().spell = characterVal.spell_5;
                spell6.GetComponent<Tooltip>().spell = characterVal.spell_6;
            }
        }

        public void UpdateInfo()
        {
            UpdateName(characterVal.characterName);
            UpdateHp(characterVal.currentHP, characterVal.currentMaxHP);
            UpdateAp(characterVal.currentAp);
            UpdateMp(characterVal.currentMp);
        }

        public void UpdateSpellIcons()
        {
            spell1.GetComponent<Image>().sprite = characterVal.spell_1.spellIcon;
            spell2.GetComponent<Image>().sprite = characterVal.spell_2.spellIcon;
            spell3.GetComponent<Image>().sprite = characterVal.spell_3.spellIcon;
            spell4.GetComponent<Image>().sprite = characterVal.spell_4.spellIcon;
            spell5.GetComponent<Image>().sprite = characterVal.spell_5.spellIcon;
            spell6.GetComponent<Image>().sprite = characterVal.spell_6.spellIcon;
        }

        public void AddEffectIcon(SpellSystem.EffectCreator effect)
        {
            GameObject GO = Instantiate(effectBlock.gameObject);
            GO.transform.SetParent(panel.transform);
            GO.GetComponentInChildren<Image>().sprite = effect.effectIcon;
            GO.GetComponent<Tooltip>().effect = effect;
        }

        //public void RemoveEffectIcon(EffectValues effect)
        //{
        //    foreach (Tooltip child in GetComponentsInChildren<Tooltip>())
        //    {
        //        if (child.effect == effect)
        //        {
        //            child.gameObject.SetActive(false);
        //        }
        //    }
        //}

        public void ToggleHighlight(bool lightswitch)
        {
            highlight.gameObject.SetActive(lightswitch);
        }

        private void OnDestroy()
        {
            //SubscribeOff();
        }

        //private void Subscribe()
        //{
        //    GameObject GC = GameObject.FindGameObjectWithTag("GameController");
        //    TurnManager TM = GC.GetComponent<TurnManager>();
        //    TM.TurnChange += handleTurnChange;
        //}

        ////Voi laittaa myös pelaajan kuoleman kohdalle
        //private void SubscribeOff()
        //{
        //    GameObject GC = GameObject.FindGameObjectWithTag("GameController");
        //    TurnManager TM = GC.GetComponent<TurnManager>();
        //    TM.TurnChange -= handleTurnChange;
        //}

        private void handleTurnChange(PlayerBehaviour player)
        {
            if (_player == player)
                isMyPlayerActive = true;
            else
                isMyPlayerActive = false;
        }

        public void UpdateHp(int minHP, int maxHP)
        {
            if (minHP <= 0)
            {
                characterVal.currentHP = 0;
                characterVal.currentAp = 0;
                characterVal.currentMp = 0;

                minHP = characterVal.currentHP;
                UpdateAp(characterVal.currentAp);
                UpdateMp(characterVal.currentMp);
                Die();
                //Remember to disable effects
            }
            tHealth.text = minHP.ToString() + "/" + maxHP.ToString();
            UpdateHpBar();
        }

        public void UpdateAp(int AP)
        {
            tAp.text = AP.ToString();
        }

        public void UpdateMp(int MP)
        {
            tMp.text = MP.ToString();
        }

        public void UpdateName(string name)
        {
            tName.text = name;
        }

        public void UpdateHpBar()
        {
            float minHPf = characterVal.currentHP;
            float maxHPf = characterVal.currentMaxHP;
            float hpBarAmount = minHPf / maxHPf;
            healthBar.GetComponent<Image>().fillAmount = hpBarAmount;
        }

        public void Die()
        {
            deathMask.gameObject.SetActive(true);
        }
    }

}