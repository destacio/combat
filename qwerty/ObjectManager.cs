using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qwerty
{
    class ObjectManager
    {
        public List<Meteor> Meteors = new List<Meteor>();
        public List<Ship> Ships = new List<Ship>();
        private const int MeteorAppearanceChance = 20;

        public combatMap combatMap;

        public ObjectManager(int mapWidth, int mapHeight)
        {
            combatMap = new combatMap(mapWidth, mapHeight);

            Ship penumbra = shipCreate(ShipType.Scout, 1, WeaponType.LightIon);
            Ships.Add(penumbra);
            Ship holycow = shipCreate(ShipType.Scout, 1, WeaponType.LightIon);
            Ships.Add(holycow);
            Ship leroy = shipCreate(ShipType.Assaulter, 1, WeaponType.HeavyLaser);
            Ships.Add(leroy);


            Ship pandorum = shipCreate(ShipType.Scout, 2, WeaponType.LightLaser);
            Ships.Add(pandorum);
            Ship exodar = shipCreate(ShipType.Scout, 2, WeaponType.LightLaser);
            Ships.Add(exodar);
            Ship neveria = shipCreate(ShipType.Assaulter, 2, WeaponType.HeavyLaser);
            Ships.Add(neveria);

            // расставляем корабли по полю, синие - слева, красные - справа

            foreach (var ship in Ships)
            {
                placeShip(ship);
            }

            meteorCreate();
        }

        public void moveMeteors()
        {
            for(int i = 0; i < Meteors.Count; i++)
            {
                if (Meteors[i].boxId >= 0)
                {
                    Meteors[i].move(combatMap);
                }
            }

            if (DoCreateMeteor)
            {
                meteorCreate();
            }
        }
        public void meteorCreate()
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
                    box4meteor = rand.Next(0, combatMap.height);
                    for (i = 0; i < 10; i++ )
                    {
                        if (combatMap.boxes[box4meteor].spaceObject != null)
                        {
                            box4meteor += 1;
                            if (box4meteor >= combatMap.height - 1)
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
                    box4meteor = combatMap.getBoxByCoords(rand.Next(0,combatMap.width/2-1) * 2, 0).id;
                    for (i = 0; i < 10; i++ )
                            {
                        if (combatMap.boxes[box4meteor].spaceObject != null)
                        {
                            box4meteor += combatMap.height * 2;
                            if (box4meteor > combatMap.width + 1)
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
                    box4meteor = rand.Next(combatMap.boxes.Count-1 - combatMap.height, combatMap.boxes.Count-1);
                    for (i = 0; i < 10; i++)
                    {
                        if (combatMap.boxes[box4meteor].spaceObject != null)
                        {
                            box4meteor += 1;
                            if (box4meteor > combatMap.boxes.Count)
                                box4meteor = combatMap.boxes.Count - combatMap.height;
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
                    box4meteor = combatMap.getBoxByCoords(rand.Next(0, combatMap.width / 2 - 1) * 2+1, combatMap.height * 2 - 1).id;
                    for (i = 0; i < 10; i++ )
                    {
                        if (combatMap.boxes[box4meteor].spaceObject != null)
                        {
                            box4meteor += combatMap.height * 2;
                            if (box4meteor > combatMap.boxes.Count-1)
                                box4meteor = combatMap.height - 1;
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
                Meteors.Add(newMeteor);
                combatMap.boxes[box4meteor].spaceObject = newMeteor;
            }
            
        }

        private bool DoCreateMeteor
        {
            get
            {
                Random rand = new Random();
                return rand.Next(0, 100) <= MeteorAppearanceChance;
            }
        }

        Ship shipCreate(ShipType type, int p, WeaponType wpn)
        {
            Ship newShip = null;
            switch (type)
            {
                case ShipType.Scout:
                    newShip = new ShipScout(p, wpn);
                    break;
                case ShipType.Assaulter:
                    newShip = new ShipAssaulter(p, wpn);
                    break;
            }
            return newShip;
        }

        public void placeShip(Ship ship)
        {
            if (ship.player == 1)
            {
                while (true)
                {
                    Random rand = new Random();
                    int randomBox = rand.Next(0, combatMap.height * 2);

                    if (combatMap.boxes[randomBox].spaceObject == null)
                    {
                        combatMap.boxes[randomBox].spaceObject = ship;
                        ship.boxId = randomBox;
                        break;
                    }
                }
            }
            else if (ship.player == 2)
            {
                while (true)
                {
                    Random rand = new Random();
                    int randomBox = rand.Next(combatMap.boxes.Count - combatMap.height * 2, combatMap.boxes.Count);

                    if (combatMap.boxes[randomBox].spaceObject == null)
                    {
                        combatMap.boxes[randomBox].spaceObject = ship;
                        ship.boxId = randomBox;
                        break;
                    }
                }
            }
        }

        public void EndTurn()
        {
            foreach (var ship in Ships)
            {
                ship.RefillEnergy();
            }

            moveMeteors();
        }

        public void drawSpaceShit(int i, ref Bitmap combatBitmap)
        {
            combatMap.boxes[i].spaceObject.drawSpaceShit(ref combatMap, ref combatBitmap);
        }
    }
}
