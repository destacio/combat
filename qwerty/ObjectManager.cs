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
        public List<Meteor> Meteors => SpaceObjects.OfType<Meteor>().ToList();
        public List<Ship> Ships => SpaceObjects.OfType<Ship>().ToList();
        public const int MeteorAppearanceChance = 20;

        public CombatMap CombatMap;
        public SpaceObject[] SpaceObjects;

        public static event EventHandler<AnimationEventArgs> ObjectAnimated; 
        
        public int FirstPlayerShipCount => Ships.Count(sh => sh.player == Player.FirstPlayer);
        public int SecondPlayerShipCount => Ships.Count(sh => sh.player == Player.SecondPlayer);
        public int BitmapWidth => CombatMap.BitmapWidth;
        public int BitmapHeight => CombatMap.BitmapHeight;
        public Ship ActiveShip { get; set; }

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

            CreateMeteor();
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

        public bool CanMoveObjectTo(SpaceObject spaceObject, Hex.OffsetCoordinates destination)
        {
            return CombatMap.AreNeighbors(spaceObject.ObjectCoordinates, destination);
        }

        public void DeleteObject(SpaceObject spaceObject)
        {
            SpaceObjects[Array.IndexOf(SpaceObjects, spaceObject)] = null;
        }

        public SpaceObject GetObjectByOffsetCoordinates(int column, int row)
        {
            if (column < 0 || column >= MapWidth || row < 0 || row >= MapHeight)
            {
                return null;
            }
            return SpaceObjects[GetObjectIndexByOffsetCoordinates(column, row)];
        }

        public void SetObjectAtOffsetCoordinates(SpaceObject spaceObject, int column, int row)
        {
            // TODO: if object coordinates already set
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

        public Hex.OffsetCoordinates GetMeteorNextStepCoordinates(Meteor meteor)
        {
            return CombatMap.GetNeighborCoordinates(meteor.ObjectCoordinates, (int)meteor.MovementDirection);
        }
        
        public void CreateMeteor()
        {
            var meteorCoordinates = new Hex.OffsetCoordinates();
            HexagonNeighborDirection movementDirection = 0;
            
            var rand = new Random();
            var randomMapSide = rand.Next(4);
            switch(randomMapSide)
            {
                case 0:  // left
                    meteorCoordinates = GetRandomVacantHexagon(0, 0, 0, MapHeight - 1);
                    movementDirection = rand.Next(2) == 0
                        ? HexagonNeighborDirection.NorthEast
                        : HexagonNeighborDirection.SouthEast;
                    break;
                case 1: // top
                    meteorCoordinates = GetRandomVacantHexagon(0, MapWidth - 1, 0, 0);
                    movementDirection = rand.Next(2) == 0
                        ? HexagonNeighborDirection.SouthEast
                        : HexagonNeighborDirection.SouthWest;
                    break;
                case 2: // right
                    meteorCoordinates = GetRandomVacantHexagon(MapWidth - 1, MapWidth - 1, 0, MapHeight - 1);
                    movementDirection = rand.Next(2) == 0
                        ? HexagonNeighborDirection.NorthWest
                        : HexagonNeighborDirection.SouthWest;
                    break;
                case 3: // bottom
                    meteorCoordinates = GetRandomVacantHexagon(0, MapWidth - 1, MapHeight - 1, MapHeight - 1);
                    movementDirection = rand.Next(2) == 0
                        ? HexagonNeighborDirection.NorthEast
                        : HexagonNeighborDirection.NorthWest;
                    break;
            }

            var meteorHealth = rand.Next(1, 150);
            var meteorDmg = meteorHealth / 4;

            var newMeteor = new Meteor(meteorCoordinates, meteorHealth, meteorDmg, movementDirection);
            SetObjectAtOffsetCoordinates(newMeteor, meteorCoordinates.Column, meteorCoordinates.Row);
        }

        public void MoveObjectTo(SpaceObject spaceObject, Hex.OffsetCoordinates destination)
        {
            ObjectAnimated?.Invoke(this, new AnimationEventArgs(spaceObject, this.CombatMap.HexToPixel(destination)));
            if (destination.Column < 0 || destination.Column >= MapWidth ||
                destination.Row < 0 || destination.Row >= MapHeight)
            {
                // moving object outside bounds = deleting object
                DeleteObject(spaceObject);
                return;
            }
            SpaceObjects[GetObjectIndexByOffsetCoordinates(spaceObject.ObjectCoordinates.Column, spaceObject.ObjectCoordinates.Row)] = null;
            SpaceObjects[GetObjectIndexByOffsetCoordinates(destination.Column, destination.Row)] = spaceObject;
            spaceObject.ObjectCoordinates = destination;
        }

        private void CreateShip(ShipType shipType, WeaponType weaponType, Player owner)
        {
            Ship newShip;
            switch (shipType)
            {
                case ShipType.Scout:
                    newShip = new ShipScout(owner, weaponType);
                    break;
                case ShipType.Assaulter:
                    newShip = new ShipAssaulter(owner, weaponType);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(shipType), shipType, null);
            }

            int minColumnIndex = owner == Player.FirstPlayer ? 0 : MapWidth - 2;
            int maxColumnIndex = owner == Player.FirstPlayer ? 1 : MapWidth - 1;
            newShip.ObjectCoordinates = GetRandomVacantHexagon(minColumnIndex, maxColumnIndex, 0, MapHeight - 1);
            SetObjectAtOffsetCoordinates(newShip, newShip.ObjectCoordinates.Column, newShip.ObjectCoordinates.Row);
        }

        private Hex.OffsetCoordinates GetRandomVacantHexagon(int minColumnIndex, int maxColumnIndex, int minRowIndex, int maxRowIndex)
        {
            var rand = new Random();
            int randomColumn;
            int randomRow;
            do
            {
                randomColumn = rand.Next(minColumnIndex, maxColumnIndex+ 1);
                randomRow = rand.Next(minRowIndex, maxRowIndex + 1);
            } while (GetObjectByOffsetCoordinates(randomColumn, randomRow) != null);
            return new Hex.OffsetCoordinates(randomColumn, randomRow);
        }

        public void EndTurn()
        {
            foreach (var ship in Ships)
            {
                ship.RefillEnergy();
            }

            //moveMeteors();
        }
    }
}
