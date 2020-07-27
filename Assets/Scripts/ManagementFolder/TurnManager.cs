using SpellSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ManagementSystem
{
    public class TurnManager : MonoBehaviour
    {


        public delegate void PlayerEvent(PlayerInfo player);
        public event PlayerEvent TurnChange;
        public event PlayerEvent TurnEnd;
        public Button nextTurnButton;
        public TeamManager teamManager;
        public SpellButtons spellButtons;
        
        public Text TurnNumberText;
        public Text MaxHrText;
        public float maxHealthReduction;
        public int turnNumber;
        public int maxHealthReductionStartTurn = 1;
        public float maxHealthReductionAmount = 0.1f;


        public void AnnounceTurnChange(GameObject newPlayerGO)
        {
            if (newPlayerGO)
            {
                PlayerInfo temp = newPlayerGO.GetComponent<PlayerInfo>();
                AnnounceTurnChange(temp);
            }
        }

        public void AnnounceTurnChange(PlayerInfo newPlayer)
        {
            if (TurnChange != null && newPlayer != null)
            {
                TurnChange(newPlayer);
            }
        }

        public void AnnounceTurnEnd(PlayerInfo endingPlayer)
        {
            if (TurnEnd != null && endingPlayer != null)
            {
                TurnEnd(endingPlayer);
            }
        }

        void Start()
        {

            Button next = nextTurnButton.GetComponent<Button>();
            next.onClick.AddListener(NextTurn);
            if (!teamManager)
                teamManager = gameObject.GetComponent<TeamManager>();
            spellButtons = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<SpellButtons>();
            turnNumber = 1;
            maxHealthReduction = 0;

            Invoke("Purkka", 0.1f);
        }

        void Purkka()
        {
            AnnounceTurnChange(teamManager.teamA[0]);
            UpdateTurnNumber();
        }

        public void NextTurn()
        {
            if (teamManager)
            {
                AnnounceTurnEnd(teamManager.activePlayer);
                spellButtons.SpellCancel();
                PlayerInfo temp = teamManager.ChangeTurnUntilValidPlayer();
                AnnounceTurnChange(temp);
            }
            else
                Debug.Log("Could not find team manager script!");
        }

        public void UpdateTurnNumber()
        {
            TurnNumberText.text = "Turn: " + turnNumber;
            UpdateMaxHR();
            MaxHrText.text = "Max health reduction: " + maxHealthReduction * 100 + "%";
        }

        public void UpdateMaxHR()
        {
            if (turnNumber >= maxHealthReductionStartTurn && maxHealthReduction < 1)
            {
                maxHealthReduction = (turnNumber - (maxHealthReductionStartTurn - 1)) * maxHealthReductionAmount;
            }
        }
    }
}

//Made by Asser