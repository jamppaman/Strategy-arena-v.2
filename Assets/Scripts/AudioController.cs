using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManagementSystem
{
    public class AudioController : MonoBehaviour
    {

        public AudioSource mainMusicControl;
        public AudioSource spellAudio;
        public AudioSource moveAudio;
        public AudioClip mainMusic;

        void Start()
        {

            if (mainMusicControl && mainMusic)
            {
                mainMusicControl.loop = true;
                mainMusicControl.clip = mainMusic;
                mainMusicControl.Play();
            }
        }

        public void PlaySpell(SpellSystem.SpellCreator spell)
        {

            spellAudio.loop = false;

            spellAudio.clip = spell.spellSound;
            spellAudio.Play();
        }

        public void PlayMovementStartLoop(CharacterSystem.CharacterValues character)
        {
            moveAudio.loop = true;

            moveAudio.clip = character.walkSoundLoop;
            moveAudio.Play();
        }
        public void SlowMovementLoop()
        {
            moveAudio.pitch -= 0.15f;
            moveAudio.volume -= 0.5f;
        }
        public void PlayMovementStopLoop()
        {
            moveAudio.pitch = 1f;
            moveAudio.volume = 1f;
            moveAudio.Stop();
        }

        public void PlayAttack(CharacterSystem.CharacterValues character)
        {
            moveAudio.loop = false;
            moveAudio.clip = character.attackSound;
            moveAudio.Play();
        }

        public void PlayDamageTaken(CharacterSystem.CharacterValues character)
        {
            spellAudio.loop = false;

            spellAudio.clip = character.takeDamageSound;
            spellAudio.Play();
        }


    }

}