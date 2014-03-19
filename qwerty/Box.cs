using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qwerty
{
    class Box
    {
        public SpaceShit spaceObject = null;
        public int id;
        public int x;
        public int y;
        public int xcenter;
        public int ycenter;

        public int xpoint1 = 15;
        public int xpoint2 = 20;
        public int xpoint3 = 30;
        public int xpoint4 = 35;
        public int xpoint5 = 30;
        public int xpoint6 = 20;

        public int ypoint1 = 25;
        public int ypoint2 = 15;
        public int ypoint3 = 15;
        public int ypoint4 = 25;
        public int ypoint5 = 35;
        public int ypoint6 = 35;

        public Box(int scale)
        {
            xpoint1 = 15 * scale;
            xpoint2 = xpoint6 = xpoint1 + 5 * scale;
            xpoint3 = xpoint5 = xpoint1 + 15 * scale;
            xpoint4 = xpoint1 + 20 * scale;

            ypoint1 = 10 + (15 * scale);
            ypoint2 = ypoint3 = ypoint1 - 10 * scale;
            ypoint5 = ypoint6 = ypoint1 + 10 * scale;
            ypoint4 = ypoint1;
        }
        public void xmove(int dx)
        {
            xpoint1 += dx;
            xpoint2 += dx;
            xpoint3 += dx;
            xpoint4 += dx;
            xpoint5 += dx;
            xpoint6 += dx;
        }

        public void ymove(int dx)
        {
            ypoint1 += dx;
            ypoint2 += dx;
            ypoint3 += dx;
            ypoint4 += dx;
            ypoint5 += dx;
            ypoint6 += dx;
        }
        public void centerDetermine()
        {
            xcenter = (xpoint2 + xpoint3) / 2;
            ycenter = (ypoint2 + ypoint6) / 2;
        }

        
    }
}
