﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using qwerty.Objects;
using qwerty.Objects.Weapons;
using Hex = Barbar.HexGrid;

namespace qwerty
{
    class ObjectManager
    {
        public int MapWidth { get; }
        public int MapHeight { get; }
        public List<Meteor> Meteors = new List<Meteor>();
        public List<Ship> Ships = new List<Ship>();
        private const int MeteorAppearanceChance = 20;

        public CombatMap CombatMap;
        public SpaceObject[] SpaceObjects;
        
        public int FirstPlayerShipCount => Ships.Count(sh => sh.player == Player.FirstPlayer);
        public int SecondPlayerShipCount => Ships.Count(sh => sh.player == Player.SecondPlayer);
        public int FieldWidth => CombatMap.FieldWidthPixels;
        public int FieldHeight => CombatMap.FieldHeightPixels;

        public ObjectManager(int mapWidth, int mapHeight)
        {
            MapWidth = mapWidth;
            MapHeight = mapHeight;
            CombatMap = new CombatMap(mapWidth, mapHeight);
            SpaceObjects = new SpaceObject[mapWidth * mapHeight];

            CreateShip(ShipType.Scout, WeaponType.LightIon, Player.FirstPlayer);
            CreateShip(ShipType.Scout, WeaponType.LightIon, Player.FirstPlayer);
            CreateShip(ShipType.Assaulter, WeaponType.HeavyLaser, Player.FirstPlayer);

            CreateShip(ShipType.Scout, WeaponType.LightLaser, Player.SecondPlayer);
            CreateShip(ShipType.Scout, WeaponType.LightLaser, Player.SecondPlayer);
            CreateShip(ShipType.Assaulter, WeaponType.HeavyLaser, Player.SecondPlayer);

            meteorCreate();
        }

        public SpaceObject PixelToSpaceObject(Point pixelCoordinates)
        {
            var hexOffsetCoordinates = PixelToOffsetCoordinates(pixelCoordinates);
            return GetObjectByOffsetCoordinates(hexOffsetCoordinates.Column, hexOffsetCoordinates.Row);
        }
        
        public Hex.OffsetCoordinates PixelToOffsetCoordinates(Point pixelCoordinates)
        {
            return CombatMap.PixelToOffsetCoordinates(pixelCoordinates);
        }

        public bool CanMoveObjectTo(SpaceObject spaceObject, Hex.CubeCoordinates destination)
        {
            return CombatMap.AreNeighbors(spaceObject.ObjectCoordinates, destination);
        }

        public SpaceObject GetObjectByOffsetCoordinates(int column, int row)
        {
            return SpaceObjects[GetObjectIndexByOffsetCoordinates(column, row)];
        }

        public void SetObjectAtOffsetCoordinates(SpaceObject spaceObject, int column, int row)
        {
            SpaceObjects[GetObjectIndexByOffsetCoordinates(column, row)] = spaceObject;
        }

        private int GetObjectIndexByOffsetCoordinates(int column, int row)
        {
            return row * MapWidth + column;
        }

        public int GetDistance(SpaceObject firstObject, SpaceObject secondObject)
        {
            return CombatMap.GetDistance(firstObject.ObjectCoordinates, secondObject.ObjectCoordinates);
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
                    if(i == 10) return;
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
                    box4meteor = CombatMap.GetCellByCellCoordinates(rand.Next(0,CombatMap.FieldWidth/2-1) * 2, 0).id;
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
                    if (i == 10) return;
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
                    if (i == 10) return;
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
                    box4meteor = CombatMap.GetCellByCellCoordinates(rand.Next(0, CombatMap.FieldWidth / 2 - 1) * 2+1, CombatMap.FieldHeight * 2 - 1).id;
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
                    if (i == 10) return;
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

            var meteorHealth = rand.Next(1, 150);
            var meteorDmg = meteorHealth / 4;

            var newMeteor = new Meteor(box4meteor, meteorHealth, meteorDmg, xWay, yWay);
            Meteors.Add(newMeteor);
            CombatMap.Cells[box4meteor].spaceObject = newMeteor;
        }

        private void CreateShip(ShipType shipType, WeaponType weaponType, Player owner)
        {
            Ship newShip;
            switch (shipType)
            {
                case ShipType.Scout:
                    newShip = new ShipScout((int)owner, weaponType);
                    break;
                case ShipType.Assaulter:
                    newShip = new ShipAssaulter((int)owner, weaponType);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(shipType), shipType, null);
            }
            
            var rand = new Random();
            int randomRow;
            int randomColumn;
            do
            {
                randomRow = rand.Next(0, FieldHeight - 1);
                randomColumn = rand.Next(0, 1);
                if (owner == Player.SecondPlayer)
                {
                    randomColumn = FieldWidth - randomColumn - 1;
                }
            } while (GetObjectByOffsetCoordinates(randomColumn, randomRow) != null);

            SetObjectAtOffsetCoordinates(newShip, randomColumn, randomRow);
            // TODO: assign right coordinates
            newShip.ObjectCoordinates = randomColumn;
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
