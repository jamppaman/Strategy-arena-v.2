using SpellSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManagementSystem
{
    public class TeamManager : MonoBehaviour
    {

        public List<PlayerInfo> teamA;
        public List<PlayerInfo> teamB;
        public bool firstTeamIsActive;
        public int playerPositionInTeam;
        [SerializeField]
        private int charactersPerTeam = 5;
        public PlayerInfo activePlayer; //Changed playerbehaviour to PlayerInfo
        public StatusEffects sEffects;
        public UIsystem.TurnTimelineController timeline;
        public SpellCast spellCast;
        public TurnManager tManager;


        private void Awake()
        {
            if (spellCast == null)
            {
                spellCast = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<SpellCast>();
            }
            SetupTeams();
        }

        // Use this for initialization
        void Start()
        {
            if (spellCast == null)
            {
                spellCast = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<SpellCast>();
            }
            firstTeamIsActive = true;
            playerPositionInTeam = 0;
            activePlayer = teamA[0];

            if (!sEffects)
                sEffects = gameObject.GetComponent<StatusEffects>();
            if (!tManager)
                tManager = gameObject.GetComponent<TurnManager>();
            if (teamA == null || teamB == null)
            {
                Debug.LogWarning("One of the teams is null!");
            }
            else if (charactersPerTeam != teamA.Count || charactersPerTeam != teamB.Count)
            {
                Debug.LogWarning("Check team sizes!");
            }

        }


        /// <summary>
        /// Sets current points to max values before match starts. Should be replaced by a method in character selection screen!
        /// </summary>

        private void SetupTeams()
        {
            foreach (var player in teamA)
            {
                SetupPlayer(player);
            }
            foreach (var player in teamB)
            {
                SetupPlayer(player);
            }
            spellCast.ActivateBodies();
        }

        private void SetupPlayer(PlayerInfo player)
        {
            player.thisCharacter.damagePlus = 0;
            player.thisCharacter.armorPlus = 0;
            player.thisCharacter.moving = false;
            player.thisCharacter.dead = false;
            player.thisCharacter.currentMaxHP = player.thisCharacter.maxHP;
            player.thisCharacter.currentHP = player.thisCharacter.maxHP;
            player.thisCharacter.currentAp = player.thisCharacter.maxAp;
            player.thisCharacter.currentMp = player.thisCharacter.maxMp;
            player.thisCharacter.damageChange = 0;
            player.thisCharacter.armorChange = 0;
            player.thisCharacter.healsReceived = 0;
            player.thisCharacter.heavy = false;

            spellCast.HandleCooldownReset(player.thisCharacter.spell_1);
            spellCast.HandleCooldownReset(player.thisCharacter.spell_2);
            spellCast.HandleCooldownReset(player.thisCharacter.spell_3);
            spellCast.HandleCooldownReset(player.thisCharacter.spell_4);
            spellCast.HandleCooldownReset(player.thisCharacter.spell_5);
            spellCast.HandleCooldownReset(player.thisCharacter.spell_6);
        }

        void ChangeTurn()
        {
            if (firstTeamIsActive)
            {
                firstTeamIsActive = false;
                activePlayer = teamB[playerPositionInTeam];
            }
            else
            {
                firstTeamIsActive = true;
                playerPositionInTeam++;
                if (playerPositionInTeam > charactersPerTeam - 1)
                {
                    playerPositionInTeam = 0;
                    tManager.turnNumber += 1;
                    tManager.UpdateTurnNumber();
                }
                activePlayer = teamA[playerPositionInTeam];
            }
            sEffects.UpdateEffects();
            sEffects.CalculateEffects(activePlayer.thisCharacter);
        }

        public PlayerInfo ChangeTurnUntilValidPlayer()
        {
            return ChangeTurnUntilValidPlayer(activePlayer);
        }

        private PlayerInfo ChangeTurnUntilValidPlayer(PlayerInfo playerEndingTurn)
        {
            playerEndingTurn.RefreshPoints();
            if (!IsOneTeamDead())
            {
                activePlayer = playerEndingTurn;
                ChangeTurn();
                while (activePlayer.thisCharacter.currentHP <= 0)
                {
                    ChangeTurn();
                    if (activePlayer == playerEndingTurn)  //Looped, we need a check for eg. 3 vs 0 situation, when someone is killed! -> Done -> CheckTeamHealths (not tested yet) -> Renamed to IsOneTeamDead
                    {
                        //AnnounceEndOfGame();
                        break;
                    }
                }
            }
            return activePlayer;
        }

        public bool IsOneTeamDead()
        {
            bool teamADead = true;
            bool teamBDead = true;
            foreach (var pb in teamA)
            {
                if (pb.thisCharacter.currentHP > 0)
                {
                    teamADead = false;
                    break;
                }
            }
            foreach (var pb in teamB)
            {
                if (pb.thisCharacter.currentHP > 0)
                {
                    teamBDead = false;
                    break;
                }
            }
            if (teamADead || teamBDead)
            {
                AnnounceEndOfGame();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AnnounceEndOfGame()
        {
            Debug.Log("End of game!");
        }

    }//Made by Asser

}