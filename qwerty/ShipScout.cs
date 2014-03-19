using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qwerty
{
    class ShipScout : SpaceShit
    {
        public int attackPower;
        public int attackRange;

        public string staticDescription;
        public override string description()
        {
            return "" + staticDescription + "\nhp - " + currentHealth + "/" + maxHealth + "\nactions - "
                            + actionsLeft + "/" + maxActions + "\nAP - " + attackPower + "\nRange - " + attackRange;
        }
        public ShipScout(int p)
        {
            objectType = Constants.SHIP;
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
                 xpoints.Add(0); // координаты точек относительно второй точки ячейки
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
                 xpoints.Add(30); // координаты точек относительно второй точки ячейки
                 xpoints.Add(0);
                 xpoints.Add(30);
                 xpoints.Add(30);
                 xpoints.Add(30);

                 ypoints.Add(15);
                 ypoints.Add(30);
                 ypoints.Add(45);
                 ypoints.Add(15);
                 ypoints.Add(15);
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
        public int attack(ref combatMap cMap, int pointB)
        {
            int dmg;
            if (actionsLeft > 0)
            {
                Random rand = new Random();
                dmg = rand.Next(-attackPower / 20, attackPower / 20) + attackPower;
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
        public void refill()
        {
            actionsLeft = maxActions;
        }
    }
}
