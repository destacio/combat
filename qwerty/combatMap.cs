using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qwerty
{
    class CombatMap
    {
        public int width;
        public int height;
        public int scale = 3;
        public List<Cell> boxes = new List<Cell>(); 
        public CombatMap(int w, int h) 
        {
            width = w;
            height = h;

            InitializeMap();
        }

        public Cell getBoxByCoords(int x, int y)
        {
            Cell targetBox = null;
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
                        Cell box = new Cell(scale, i, j, new Size(scale + 10,(int)(Math.Sin(Math.PI/3) * scale + 10)));
                        
                        //box.x = xcoord;
                        //box.y = ycoord;
                        box.id = count++;

                        boxes.Add(box);

                    }
                    else
                    {
                        // четная
                        if (j % height == 0) ycoord = 0;
                        Cell box = new Cell(scale, i, j, new Size(scale + 10, (int)(Math.Sin(Math.PI / 3) * scale + 10)));

                        //box.x = xcoord;
                        //box.y = ycoord;
                        box.id = count++;

                        boxes.Add(box);
                    }
                    ycoord += 2;
                }
                xcoord++;
            }
        }
    }

 
}
