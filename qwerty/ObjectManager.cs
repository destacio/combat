using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qwerty
{
    class ObjectManager
    {
        public List<Meteor> meteors = new List<Meteor>();
        public int meteorAppearanceChance = 20;

        public void moveMeteors(combatMap cMap)
        {
            for(int i = 0; i < meteors.Count; i++)
            {
                if (meteors[i].boxId >= 0)
                {
                    meteors[i].move(cMap);
                }
            }
        }
        public void meteorCreate(combatMap cMap)
        {
            int flag = 1;
            int box4meteor = 0;
            int meteorHealth;
            int meteorDmg;
            int xWay = Constants.RIGHT;
            int yWay = Constants.MEDIUM_TOP;
            Meteor newMeteor;
            int i;

            Random rand = new Random();
            int randomNum = rand.Next(1, 100) % 4;
            

            // место появления и направление полёта
            switch(randomNum)
            {
                
                case 1:  // left
                    box4meteor = rand.Next(0, cMap.height);
                    for (i = 0; i < 10; i++ )
                    {
                        if (cMap.boxes[box4meteor].spaceObject != null)
                        {
                            box4meteor += 1;
                            if (box4meteor >= cMap.height - 1)
                                box4meteor = 0;
                        }
                        else break;
                    }
                    if(i == 10) flag = 0;
                        xWay = Constants.RIGHT;
                    if(rand.Next(1, 100) > 50)
                    {
                        yWay = Constants.MEDIUM_TOP;
                    }
                    else 
                    {
                        yWay = Constants.MEDIUM_BOTTOM;
                    }
                    break;
                case 2: // top
                    box4meteor = cMap.getBoxByCoords(rand.Next(0,cMap.width/2-1) * 2, 0).id;
                    for (i = 0; i < 10; i++ )
                            {
                        if (cMap.boxes[box4meteor].spaceObject != null)
                        {
                            box4meteor += cMap.height * 2;
                            if (box4meteor > cMap.width + 1)
                                box4meteor = 0;
                        }
                        else break;
                    }
                    if (i == 10) flag = 0;
                    if (rand.Next(1, 100) > 50)
                    {
                        xWay = Constants.RIGHT;
                    }
                    else
                    {
                        xWay = Constants.LEFT;
                    }
                    yWay = Constants.MEDIUM_BOTTOM;
                    break;
                case 3: // right
                    box4meteor = rand.Next(cMap.boxes.Count-1 - cMap.height, cMap.boxes.Count-1);
                    for (i = 0; i < 10; i++)
                    {
                        if (cMap.boxes[box4meteor].spaceObject != null)
                        {
                            box4meteor += 1;
                            if (box4meteor > cMap.boxes.Count)
                                box4meteor = cMap.boxes.Count - cMap.height;
                        }
                        else break;
                    }
                    if (i == 10) flag = 0;
                    xWay = Constants.LEFT;
                    if (rand.Next(1, 100) > 50)
                    {
                        yWay = Constants.MEDIUM_TOP;
                    }
                    else
                    {
                        yWay = Constants.MEDIUM_BOTTOM;
                    }
                    break;
                case 0: // bottom
                    box4meteor = cMap.getBoxByCoords(rand.Next(0, cMap.width / 2 - 1) * 2+1, cMap.height * 2 - 1).id;
                    for (i = 0; i < 10; i++ )
                    {
                        if (cMap.boxes[box4meteor].spaceObject != null)
                        {
                            box4meteor += cMap.height * 2;
                            if (box4meteor > cMap.boxes.Count-1)
                                box4meteor = cMap.height - 1;
                        }
                        else break;
                    }
                    if (i == 10) flag = 0;
                    if (rand.Next(1, 100) > 50)
                    {
                        xWay = Constants.RIGHT;
                    }
                    else
                    {
                        xWay = Constants.LEFT;
                    }
                    yWay = Constants.MEDIUM_TOP;
                    break;
            }
            
            if (flag == 1)
            {
                meteorHealth = rand.Next(1, 150);
                meteorDmg = meteorHealth / 4;

                newMeteor = new Meteor(box4meteor, meteorHealth, meteorDmg, xWay, yWay);
                meteors.Add(newMeteor);
                cMap.boxes[box4meteor].spaceObject = newMeteor;
            }
            
        }
        public int whether2createMeteor()
        {
            Random rand = new Random();
            if (rand.Next(0, 100) <= meteorAppearanceChance)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
