using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFieldSystem
{
    public class PositionContainer
    {

        public int x;
        public int z;

        /// <summary>
        /// Contains x and z coordinates for grid
        /// </summary>
        /// <param name="vec"></param>

        public PositionContainer(Vector3 vec)
        {
            this.x = (int)vec.x;
            this.z = (int)vec.z;
        }
        public PositionContainer(int x, int z)
        {
            this.x = x;
            this.z = z;
        }
    }

}