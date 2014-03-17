using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    attackRange = 1;
                    maxActions = 3;
                    actionsLeft = maxActions;
                    staticDescription = "Лёгкий корабль\nкласса Scout";

                    if (player == 1) 
                    {     
                        xpoints.Add(0); // координаты точек относительно первой точки ячейки
                        xpoints.Add(15);
                        xpoints.Add(30);
                        xpoints.Add(15);
                        xpoints.Add(0);

                        ypoints.Add(15);
                        ypoints.Add(23);
                        ypoints.Add(30);
                        ypoints.Add(38);
                        ypoints.Add(45);
                    }
                    else if (player == 2)
                    {
                        xpoints.Add(30); // координаты точек относительно первой точки ячейки
                        xpoints.Add(0);
                        xpoints.Add(30);
                        // пока так.
                        xpoints.Add(30);
                        xpoints.Add(30);

                        ypoints.Add(15);
                        ypoints.Add(30);
                        ypoints.Add(45);
                        // и снова пока так
                        ypoints.Add(15);
                        ypoints.Add(15);
                    }
                    break;

                case Constants.ASSAULTER:
                    player = p;
                    maxHealth = 100;
                    currentHealth = maxHealth;
                    attackPower = 50;
                    attackRange = 1;
                    maxActions = 2;
                    actionsLeft = maxActions;
                    staticDescription = "Средний боевой корабль\nкласса Assaulter";

                    if (player == 1)
                    {
                        xpoints.Add(0); // координаты точек относительно второй точки ячейки
                        xpoints.Add(25);
                        xpoints.Add(35);
                        xpoints.Add(25);
                        xpoints.Add(0);

                        ypoints.Add(15);
                        ypoints.Add(20);
                        ypoints.Add(30);
                        ypoints.Add(40);
                        ypoints.Add(45);
                    }
                    else if (player == 2)
                    {
                        xpoints.Add(30); // координаты точек относительно первой точки ячейки
                        xpoints.Add(5);
                        xpoints.Add(-5);
                        xpoints.Add(5);
                        xpoints.Add(30);

                        ypoints.Add(15);
                        ypoints.Add(20);
                        ypoints.Add(30);
                        ypoints.Add(40);
                        ypoints.Add(45);
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
        public void refill()
        {
            actionsLeft = maxActions;
        }
    }
}
