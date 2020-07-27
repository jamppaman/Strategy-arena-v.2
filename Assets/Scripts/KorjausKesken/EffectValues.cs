using CharacterSystem;
using UnityEngine;

namespace SpellSystem
{
    [CreateAssetMenu(fileName = "Effect", menuName = "New Effect")]
    public class EffectValues : ScriptableObject
    {
        [Header("Info")]
        [Space(6)]
        public string effectName;
        public Sprite effectIcon;
        [Space(6)]
        [Header("Buffs/Debuffs")]
        [Space(6)]
        public int effectDuration = 0;
        public bool stacks = true; //<
        public bool canBeSilenced = true; //<
        [Space(6)]
        public int silenceTurns = 0;    //<
        public bool immune = false;   //<
        public bool heavyState = false; //<
        [Space(6)]
        public int damageModifyPlus = 0;    //<
        public bool dmPlusAllies;
        public bool dmPlusEnemies;
        [Space(6)]
        [Range(-1f, 1f)]
        public float damageModifyPercent = 0;   //<
        public bool dmPercentAllies;
        public bool dmPercentEnemies;
        [Space(6)]
        public int armorModifyPlus = 0;   //<
        public bool amPlusAllies;
        public bool amPlusEnemies;
        [Space(6)]
        [Range(-1f, 1f)]
        public float armorModifyPercent = 0;   //<
        public bool amPerAllies;
        public bool amPerEnemies;
        [Space(6)]
        [Range(-1f, 1f)]
        public float healModify = 0;    //<
        public bool hmAllies;
        public bool hmEnemies;
        [Space(6)]
        public int apModify = 0;    //<
        public bool apAllies;
        public bool apEnemies;
        [Space(6)]
        public int mpModify = 0;    //<
        public bool mpAllies;
        public bool mpEnemies;
        [Space(6)]
        [Header("Do not touch!")]
        [Space(6)]

        public int remainingTurns = 0;
        public CharacterValues caster;
        public CharacterValues target;
    }

}