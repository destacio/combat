using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qwerty
{
    class combatMap
    {
        public int width;
        public int height;
        public int scale = 3;
        public List<Box> boxes = new List<Box>();
        public int deltax; 
        public int deltay; 
        public combatMap(int w, int h) 
        {
            width = w;
            height = h;

            deltax = 15 * scale;
            deltay = 20 * scale;

            iniBasicPoints();
        }
        public void iniBasicPoints()
        {
            for(int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if(i % 2 == 1)
                    {
                        // нечетная
                        Box box = new Box(scale);
                        box.xmove(deltax * i - 10 * scale);
                        box.ymove(deltay * j + 10 * scale);

                        boxes.Add(box);

                    }
                    else
                    {
                        // четная
                        Box box = new Box(scale);
                        box.xmove(deltax * i - 10 * scale);
                        box.ymove(deltay * j + 0);

                        boxes.Add(box);
                    }
                }
            }
        }
    }

 
}
