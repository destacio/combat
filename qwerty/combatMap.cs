using System;
using System.Collections.Generic;
using System.Drawing;
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

            deltax = 20 * scale;
            deltay = 20 * scale;

            InitializeMap();
        }

        public Box getBoxByCoords(int x, int y)
        {
            Box targetBox = null;
            for (int i = 0; i < boxes.Count; i++ )
            {
                if (boxes[i].x == x && boxes[i].y == y)
                {
                    targetBox = boxes[i];
                    break;
                }
            }
            return targetBox;
        }
       
        public void InitializeMap()
        {
            int xcoord = 0;
            int ycoord = 0;
            int count = 0;

            scale = 40;

            for(int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if(i % 2 == 1)
                    {
                        // нечетная
                        if (j % height == 0) ycoord = 1;
                        Box box = new Box(scale, i, j, new Size(scale + 10,(int)(Math.Sin(Math.PI/3) * scale + 10)));
                        
                        box.x = xcoord;
                        box.y = ycoord;
                        box.id = count++;
                        box.centerDetermine();

                        boxes.Add(box);

                    }
                    else
                    {
                        // четная
                        if (j % height == 0) ycoord = 0;
                        Box box = new Box(scale, i, j, new Size(scale + 10, (int)(Math.Sin(Math.PI / 3) * scale + 10)));

                        box.x = xcoord;
                        box.y = ycoord;
                        box.id = count++;
                        box.centerDetermine();

                        boxes.Add(box);
                    }
                    ycoord += 2;
                }
                xcoord++;
            }
        }
    }

 
}
