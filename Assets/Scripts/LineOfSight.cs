using GameFieldSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellSystem
{
    public class LineOfSight : MonoBehaviour
    {
        public GridController gridC;

        public bool LoSCheck(Tile startpos, Tile target)
        {


            int x0 = startpos.locX;
            int y0 = startpos.locZ;
            int x1 = target.locX;
            int y1 = target.locZ;

            var clear = true;
            var dx = Math.Abs(x1 - x0);
            var dy = Math.Abs(y1 - y0);
            var x = x0;
            var y = y0;
            var n = -1 + dx + dy;
            var x_inc = (x1 > x0 ? 1 : -1);
            var y_inc = (y1 > y0 ? 1 : -1);
            var error = dx - dy;
            dx *= 2;
            dy *= 2;

            for (var i = 0; i < 1; i++)
            {

                if (error > 0)
                {
                    x += x_inc;
                    error -= dy;
                }

                else if (error < 0)
                {
                    y += y_inc;
                    error += dx;
                }

                else
                {
                    x += x_inc;
                    error -= dy;
                    y += y_inc;
                    error += dx;
                    n--;
                }
            }

            while (n > 0 && clear)
            {

                if (gridC.GetTile(x, y).myType == Tile.BlockType.BlockyBlock || gridC.GetTile(x, y).CharCurrentlyOnTile == true)
                {
                    clear = false;
                }

                else
                {

                    if (error > 0)
                    {
                        x += x_inc;
                        error -= dy;
                    }

                    else if (error < 0)
                    {
                        y += y_inc;
                        error += dx;
                    }

                    else
                    {
                        x += x_inc;
                        error -= dy;
                        y += y_inc;
                        error += dx;
                        n--;
                    }

                    n--;
                }
            }
            return clear;
        }
    }

}