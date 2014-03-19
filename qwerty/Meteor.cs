using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qwerty
{
    class Meteor : SpaceShit
    {
        public int explodeDmg;
        public string staticDescription = "Движущийся метеор";
        public int xdirection;
        public int ydirection;
        public Meteor(int box, int health, int dmg, int x, int y)
        {
            boxId = box;
            objectType = Constants.METEOR;
            player = 0;
            maxHealth = health;
            currentHealth = maxHealth;
            explodeDmg = dmg;
            xdirection = x;
            ydirection = y;
            
        }
        public void move(combatMap cMap)
        {
                int newx;
                int newy;
                int pointB;

                newx = cMap.boxes[boxId].x + xdirection;
                newy = cMap.boxes[boxId].y + ydirection;

                if (newx < 0 || newx > cMap.width - 1
                    || newy < 0 || newy > cMap.height * 2 - 1)
                {
                    cMap.boxes[boxId].spaceObject.player = -1;
                    cMap.boxes[boxId].spaceObject = null;
                    boxId = -1;
                }
                else
                {
                    pointB = cMap.getBoxByCoords(newx, newy).id;

                    if (cMap.boxes[pointB].spaceObject == null)
                    {
                        cMap.boxes[boxId].spaceObject = null;
                        cMap.boxes[pointB].spaceObject = this;
                        boxId = cMap.boxes[pointB].id;
                    }
                    else
                    {
                        cMap.boxes[pointB].spaceObject.currentHealth -= explodeDmg;
                        if (cMap.boxes[pointB].spaceObject.currentHealth <= 0)
                        {
                            cMap.boxes[pointB].spaceObject.player = -1;
                            cMap.boxes[pointB].spaceObject.boxId = -1;
                            cMap.boxes[pointB].spaceObject = null;
                        }
                        cMap.boxes[boxId].spaceObject.player = -1;
                        cMap.boxes[boxId].spaceObject = null;
                        boxId = -1;
                    }
                    
                }
        }
        public override string description()
        {
            string x = "";
            string y = "";

            switch(xdirection)
            {
                case -1:
                    x = "left ";
                    break;
                case 1:
                    x = "right ";
                    break;
            }
            switch(ydirection)
            {
                case -1:
                    y = "top ";
                    break;
                case 1:
                    y = "bottom ";
                    break;
            }

            return staticDescription + "\nУрон при попадании\n в корабль: " + explodeDmg 
                + "\nhp - " + currentHealth 
                + "\nНаправление: \n" + x + y;
        }
    }
}
