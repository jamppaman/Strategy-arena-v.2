using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManagementSystem
{
    public class GameStart : MonoBehaviour
    {

        public enum Map { baseMap, one, two };
        public Map currentMap;

        void Start()
        {
            switch (currentMap)
            {
                case Map.baseMap:
                    break;
                case Map.one:
                    break;
                case Map.two:
                    break;
                default:
                    break;
            }





        }

        void Update()
        {

        }
    }

}