using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using qwerty.Objects;
using qwerty.Objects.Weapons;

namespace qwerty
{
    class ObjectManager
    {
        public List<Meteor> Meteors = new List<Meteor>();
        public List<Ship> Ships = new List<Ship>();
        private const int MeteorAppearanceChance = 20;

        public CombatMap CombatMap;

        public int FirstPlayerShipCount => Ships.Count(sh => sh.player == 1);
        public int SecondPlayerShipCount => Ships.Count(sh => sh.player == 2);
        public int FieldWidth => CombatMap.FieldWidthPixels;
        public int FieldHeight => CombatMap.FieldHeightPixels;

        public ObjectManager(int mapWidth, int mapHeight)
        {
            CombatMap = new CombatMap(mapWidth, mapHeight);

            Ship penumbra = CreateShip(ShipType.Scout, 1, WeaponType.LightIon);
            Ships.Add(penumbra);
            Ship holycow = CreateShip(ShipType.Scout, 1, WeaponType.LightIon);
            Ships.Add(holycow);
            Ship leroy = CreateShip(ShipType.Assaulter, 1, WeaponType.HeavyLaser);
            Ships.Add(leroy);


            Ship pandorum = CreateShip(ShipType.Scout, 2, WeaponType.LightLaser);
            Ships.Add(pandorum);
            Ship exodar = CreateShip(ShipType.Scout, 2, WeaponType.LightLaser);
            Ships.Add(exodar);
            Ship neveria = CreateShip(ShipType.Assaulter, 2, WeaponType.HeavyLaser);
            Ships.Add(neveria);

            // расставляем корабли по полю, синие - слева, красные - справа

            foreach (var ship in Ships)
            {
                CombatMap.PlaceShip(ship);
            }

            meteorCreate();
        }

        public void moveMeteors()
        {
            foreach (var meteor in Meteors)
            {
                if (meteor.boxId >= 0)
                {
                    meteor.move(CombatMap);
                }
            }

            Random rand = new Random();
            if (rand.Next(0, 100) <= MeteorAppearanceChance)
            {
                meteorCreate();
            }
        }
        public void meteorCreate()
        {
            bool createNewMeteor = true;
            int box4meteor = 0;
            int xWay = Constants.RIGHT;
            int yWay = Constants.MEDIUM_TOP;
            int i;

            Random rand = new Random();
            int randomNum = rand.Next(1, 100) % 4;
            

            // место появления и направление полёта
            switch(randomNum)
            {
                
                case 1:  // left
                    box4meteor = rand.Next(0, CombatMap.FieldHeight);
                    for (i = 0; i < 10; i++ )
                    {
                        if (CombatMap.Cells[box4meteor].spaceObject != null)
                        {
                            box4meteor += 1;
                            if (box4meteor >= CombatMap.FieldHeight - 1)
                                box4meteor = 0;
                        }
                        else break;
                    }
                    if(i == 10) createNewMeteor = false;
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
                    box4meteor = CombatMap.GetCellByCoordinates(rand.Next(0,CombatMap.FieldWidth/2-1) * 2, 0).id;
                    for (i = 0; i < 10; i++ )
                            {
                        if (CombatMap.Cells[box4meteor].spaceObject != null)
                        {
                            box4meteor += CombatMap.FieldHeight * 2;
                            if (box4meteor > CombatMap.FieldWidth + 1)
                                box4meteor = 0;
                        }
                        else break;
                    }
                    if (i == 10) createNewMeteor = false;
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
                    box4meteor = rand.Next(CombatMap.Cells.Count-1 - CombatMap.FieldHeight, CombatMap.Cells.Count-1);
                    for (i = 0; i < 10; i++)
                    {
                        if (CombatMap.Cells[box4meteor].spaceObject != null)
                        {
                            box4meteor += 1;
                            if (box4meteor > CombatMap.Cells.Count)
                                box4meteor = CombatMap.Cells.Count - CombatMap.FieldHeight;
                        }
                        else break;
                    }
                    if (i == 10) createNewMeteor = false;
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
                    box4meteor = CombatMap.GetCellByCoordinates(rand.Next(0, CombatMap.FieldWidth / 2 - 1) * 2+1, CombatMap.FieldHeight * 2 - 1).id;
                    for (i = 0; i < 10; i++ )
                    {
                        if (CombatMap.Cells[box4meteor].spaceObject != null)
                        {
                            box4meteor += CombatMap.FieldHeight * 2;
                            if (box4meteor > CombatMap.Cells.Count-1)
                                box4meteor = CombatMap.FieldHeight - 1;
                        }
                        else break;
                    }
                    if (i == 10) createNewMeteor = false;
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
            
            if (createNewMeteor)
            {
                var meteorHealth = rand.Next(1, 150);
                var meteorDmg = meteorHealth / 4;

                var newMeteor = new Meteor(box4meteor, meteorHealth, meteorDmg, xWay, yWay);
                Meteors.Add(newMeteor);
                CombatMap.Cells[box4meteor].spaceObject = newMeteor;
            }
            
        }

        private static Ship CreateShip(ShipType type, int p, WeaponType wpn)
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
            CombatMap.Cells[i].spaceObject.drawSpaceShit(ref CombatMap, ref combatBitmap);
        }
    }
}
