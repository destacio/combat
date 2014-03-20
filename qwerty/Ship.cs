using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace qwerty
{
    class Ship : SpaceShit
    {
        public int attackPower;
        public int attackRange;
        public override string description()
        {
            return "";
        }

        public override void drawSpaceShit(ref combatMap cMap, ref System.Drawing.Bitmap bmap)
        {
            
        }

        public void moveShip(combatMap cMap, int pointAId, int pointBId)
        {

            if (actionsLeft > 0)
            {


                boxId = pointBId;
                cMap.boxes[pointAId].spaceObject = null;
                cMap.boxes[pointBId].spaceObject = this;
                actionsLeft -= 1;


            }
        }
        public int attack(combatMap cMap, int pointB)
        {
            int dmg;
            if (actionsLeft > 0)
            {
                Random rand = new Random();
                dmg = rand.Next(-attackPower / 10, attackPower / 10) + attackPower;
                cMap.boxes[pointB].spaceObject.currentHealth -= dmg;
                actionsLeft -= 1;
                if (cMap.boxes[pointB].spaceObject.currentHealth <= 0)
                {
                    cMap.boxes[pointB].spaceObject.player = -1;
                    cMap.boxes[pointB].spaceObject.boxId = -1;
                    cMap.boxes[pointB].spaceObject = null;
                    return 1;
                }
            }
            return 0;
        }
        public void placeShip(ref combatMap cMap)
        {
            if (player == 1)
            {
                while (true)
                {
                    Random rand = new Random();
                    int randomBox = rand.Next(0, cMap.height * 2);

                    if (cMap.boxes[randomBox].spaceObject == null)
                    {
                        cMap.boxes[randomBox].spaceObject = this;
                        boxId = randomBox;
                        break;
                    }
                }
            }
            else if (player == 2)
            {
                while (true)
                {
                    Random rand = new Random();
                    int randomBox = rand.Next(cMap.boxes.Count - cMap.height * 2, cMap.boxes.Count);

                    if (cMap.boxes[randomBox].spaceObject == null)
                    {
                        cMap.boxes[randomBox].spaceObject = this;
                        boxId = randomBox;
                        break;
                    }
                }
            }
        }

        public void shipRotate(double angle)
        {
            int xold, yold;
            angle = angle * Math.PI / 180;
            for (int i = 0; i < xpoints.Count; i++)
            {
                xold = xpoints[i];
                yold = ypoints[i];
                xpoints[i] = (int)(Math.Round((double)xold * Math.Cos(angle) - (double)yold * Math.Sin(angle), 0));
                ypoints[i] = (int)(Math.Round((double)xold * Math.Sin(angle) + (double)yold * Math.Cos(angle), 0));
            }
        }
        public void refill()
        {
            actionsLeft = maxActions;
        }
    }
}
