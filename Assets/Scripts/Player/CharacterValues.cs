using GameFieldSystem;
using SpellSystem;
using UnityEngine;
namespace CharacterSystem
{

    [CreateAssetMenu(fileName = "Character", menuName = "New Character")]
    public class CharacterValues : ScriptableObject
    {
        [Header("Character info")]
        [Space(6)]
        public string characterName;
        public Sprite portrait;
        public int maxHP;
        public int maxAp;
        public int maxMp;
        public SpellCreator spell_1;
        public SpellCreator spell_2;
        public SpellCreator spell_3;
        public SpellCreator spell_4;
        public SpellCreator spell_5;
        public SpellCreator spell_6;
        public AudioClip walkSoundLoop; //<
        public AudioClip attackSound; //<
        public AudioClip takeDamageSound; //<
        [Space(30)]
        [Header("Ingame modifiables")]
        [Space(6)]
        public int damagePlus;
        public int armorPlus;
        public bool moving;
        public PositionContainer currentTile;
        public bool dead;
        public bool team; // true = 1 false = 2
        public int currentMaxHP;
        public int currentHP;
        public int currentAp;
        public int currentMp;
        public float damageChange;
        public float armorChange;
        public float healsReceived;
        public bool heavy = false;
    }

}