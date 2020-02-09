using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIsystem
{
    public class BlockInfo : MonoBehaviour
    {

        public CharacterSystem.CharacterValues character;
        public GameObject arrow;

        void Start()
        {
            GetComponent<Tooltip>().character = character;
        }
    }

}