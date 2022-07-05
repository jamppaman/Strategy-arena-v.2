using CharacterSystem;
using ManagementSystem;
using System.Collections;
using System.Collections.Generic;
using UIsystem;
using UnityEngine;


namespace SpellSystem
{
    public class SpellCalculators : MonoBehaviour
    {
        // references
        public PlayerBehaviour playerBehaviour;
        public TurnManager turnManager;
        public HitText hitText;
        TeamManager teamManager;


        void Start()
        {
            teamManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<TeamManager>();
            hitText = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<HitText>();
            turnManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<TurnManager>();
        }

        // V V V damaging health values
        public void GetHit(CharacterValues target, int damage)
        {
            if (target.immunity == true)
            {
                damage = 0;
            }
            //Damage dealt
            target.currentHP -= damage;
            //Popup
            if (damage != 0)
            {
                hitText.DamageText(target, damage * (-1));
            }
            //Max health reduction
            target.currentMaxHP -= Mathf.RoundToInt(damage * turnManager.maxHealthReduction);
            if (target.currentHP <= 0)
            {
                teamManager.GetDead(target);
            }
            playerBehaviour.UpdateTabs();
        }
        // V V V healing health values
        public void GetHealed(CharacterValues target, int heal)
        {
            target.currentHP += heal;
            //Popup
            if (target.currentHP != target.currentMaxHP && heal != 0)
            {
                hitText.DamageText(target, heal);
            }
            //Max hp check
            if (target.currentHP > target.currentMaxHP)
            {
                target.currentHP = target.currentMaxHP;
            }
        }


        // mininum damage calculator
        public int MinDamCacl(int damMin, float damChange, float armorChange, int damPlus, int armorPlus)
        {
            int tempdamage = Mathf.RoundToInt(damMin * (1 + (damChange - armorChange)) + (damPlus - armorPlus));

            return tempdamage;
        }
        
        // maxinum damage calculator
        public int MaxDamCacl(int damMax, float damChange, float armorChange, int damPlus, int armorPlus)
        {
            int tempdamage = Mathf.RoundToInt(damMax * (1 + (damChange - armorChange)) + (damPlus - armorPlus));

            return tempdamage;
        }

        // randomizes damage between min and max
        public int TrueDamageCalculator(int damMax, int damMin, float damChange, float armorChange, int damPlus, int armorPlus)
        {
            int tempdamageMin = MinDamCacl(damMin, damChange, armorChange, damPlus, armorPlus);
            int tempdamageMax = MaxDamCacl(damMax, damChange, armorChange, damPlus, armorPlus);
            int trueDamage = UnityEngine.Random.Range(tempdamageMin, tempdamageMax);

            return trueDamage;
        }

        // mininum heal calculator
        public int MinHealCacl(int damMin, float heals)
        {
            int tempHeal = Mathf.RoundToInt(damMin * (1 + heals));

            return tempHeal;
        }

        // maxinum heal calculator
        public int MaxHealCacl(int damMax, float heals)
        {
            int tempHeal = Mathf.RoundToInt(damMax * (1 + heals));

            return tempHeal;
        }

        // randomizes healing between min and max
        public int TrueHealCalculator(int healMax, int healMin, float heals)
        {
            int tempHealMin = MinHealCacl(healMin, heals);
            int tempHealMax = MaxHealCacl(healMax, heals);
            int trueHeal = UnityEngine.Random.Range(tempHealMin, tempHealMax);

            return trueHeal;
        }

        // resets cooldown calculators to default values
        public void HandleCooldownReset(SpellCreator spell)
        {
            spell.spellInitialCooldowncounter = spell.spellInitialCooldown;
            spell.spellCooldownLeft = 0;
            spell.spellCastPerturncounter = 0;
        }

        // handles cooldown decrease <-- oh REALLY. i would have never guessed
        public void HandleCooldownDecrease(SpellCreator spell)
        {

            if (spell.spellInitialCooldowncounter > 0)
                spell.spellInitialCooldowncounter--;

            if (spell.spellCooldownLeft > 0)
                spell.spellCooldownLeft--;


            spell.spellCastPerturncounter = 0;
        }

        // handles cooldown increase <-- are you even trying
        public void HandleCoolDownIncrease(SpellCreator spell)
        {
            if (spell.spellCastPerturn != 0)
            {
                spell.spellCastPerturncounter++;
            }
            if ((spell.spellCastPerturncounter >= spell.spellCastPerturn && spell.spellCastPerturn != 0) || spell.spellCooldown > 0)
            {
                spell.spellCooldownLeft = spell.spellCooldown;
            }
        }


    }
}