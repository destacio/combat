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
        
        
        public string staticDescription;
        public override string description()
        {
            return "" + staticDescription + "\nhp - " + currentHealth + "/" + maxHealth + "\nactions - "
                            + actionsLeft + "/" + maxActions + "\nAP - " + attackPower + "\nRange - " + attackRange;
        }
        public Ship(int type, int p)
        {
            objectType = Constants.SHIP;
            switch(type)
            {
                case Constants.SCOUT:
                    player = p;
                    maxHealth = 50;
                    currentHealth = maxHealth;
                    attackPower = 25;
                    attackRange = 3;
                    maxActions = 3;
                    actionsLeft = maxActions;
                    staticDescription = "Лёгкий корабль\nкласса Scout";

                    xpoints.Add(-15); // координаты точек относительно второй точки ячейки
                    xpoints.Add(0);
                    xpoints.Add(15);
                    xpoints.Add(0);
                    xpoints.Add(-15);
                    // лишние точки
                    
                    ypoints.Add(-14);
                    ypoints.Add(-7);
                    ypoints.Add(0);
                    ypoints.Add(7);
                    ypoints.Add(14);
                    // лишние точки
                    
                    if (player == 2)
                    {
                        shipRotate(180);
                    }
                    break;

                case Constants.ASSAULTER:
                    player = p;
                    maxHealth = 100;
                    currentHealth = maxHealth;
                    attackPower = 50;
                    attackRange = 5;
                    maxActions = 2;
                    actionsLeft = maxActions;
                    staticDescription = "Средний боевой корабль\nкласса Assaulter";

                   
                    xpoints.Add(-16); // координаты точек относительно второй точки ячейки
                    xpoints.Add(6);
                    xpoints.Add(18);
                    xpoints.Add(6);
                    xpoints.Add(-16);

                    // лишние точки
                    
                    ypoints.Add(-15);
                    ypoints.Add(-10);
                    ypoints.Add(0);
                    ypoints.Add(10);
                    ypoints.Add(15);

                    if (player == 2)
                    {
                        shipRotate(180);
                    }   
                    break;
            }
        }
        public void moveShip(ref combatMap cMap, int pointAId, int pointBId)
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
                dmg = rand.Next(-attackPower/10, attackPower/10) + attackPower;
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
            if(player == 1)
            {
                while(true)
                {
                    Random rand = new Random();
                    int randomBox = rand.Next(0, cMap.height*2); 

                    if(cMap.boxes[randomBox].spaceObject == null)
                    {
                        cMap.boxes[randomBox].spaceObject = this;
                        boxId = randomBox;
                        break;
                    }
                }
            }
            else if(player == 2)
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
