using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using qwerty.Objects;
using Hex = Barbar.HexGrid;

namespace qwerty
{
    class CombatMap
    {
        public int FieldWidth;
        public int FieldHeight;
        private const float CellSideLength = 40;

        public Hex.HexLayout<Hex.Point, Hex.PointPolicy> HexGrid = Hex.HexLayoutFactory.CreateFlatHexLayout(
            new Hex.Point(CellSideLength, CellSideLength), new Hex.Point((int)(CellSideLength + 10),  (int)(Math.Sin(Math.PI / 3) * CellSideLength + 10)),
            Hex.Offset.Odd);
        
        public readonly List<Cell> Cells = new List<Cell>();
        public SpaceObject[] SpaceObjects;
        public CombatMap(int w, int h) 
        {
            FieldWidth = w;
            FieldHeight = h;
            SpaceObjects = new SpaceObject[w * h];
            InitializeMap();
        }

        public int FieldWidthPixels
        {
            get
            {
                var hexagonOffset =
                    HexGrid.HexToPixel(HexGrid.ToCubeCoordinates(new Hex.OffsetCoordinates(FieldWidth, FieldHeight)));
                var cornerOffset = HexGrid.HexCornerOffset(0);
                return (int)(hexagonOffset.X + cornerOffset.X);
            }
        }

        public int FieldHeightPixels
        {
            get
            {
                var hexagonOffset =
                    HexGrid.HexToPixel(HexGrid.ToCubeCoordinates(new Hex.OffsetCoordinates(FieldWidth, FieldHeight)));
                var cornerOffset = HexGrid.HexCornerOffset(1);
                return (int)(hexagonOffset.Y + cornerOffset.Y);
            }
        }

        public Cell GetCellByCellCoordinates(int x, int y)
        {
            if (x < 0 || y < 0 || x*FieldHeight + y > FieldWidth*FieldHeight)
            {
                return null;
            }

            return Cells[x*FieldHeight + y];
        }

        public Cell GetCellByPixelCoordinates(int x, int y)
        {
            var hexagon = HexGrid.PixelToHex(new Hex.Point(x, y)).Round();
            var offsetCoordinates = HexGrid.ToOffsetCoordinates(hexagon);
            return Cells.Find(c => c.x == offsetCoordinates.Column && c.y == offsetCoordinates.Row);
        }

        public bool AreNeighbors(Hex.OffsetCoordinates firstHexagon, Hex.OffsetCoordinates secondHexagon)
        {
            return AreNeighbors(HexGrid.ToCubeCoordinates(firstHexagon), HexGrid.ToCubeCoordinates(secondHexagon));
        }
        
        public bool AreNeighbors(Hex.CubeCoordinates firstHexagon, Hex.CubeCoordinates secondHexagon)
        {
            for (int i = 0; i < 6; i++)
            {
                if (Hex.CubeCoordinates.Neighbor(firstHexagon, i).Equals(secondHexagon))
                {
                    return true;
                }
            }
            return false;
        }

        public PointF[] GetHexagonCorners(Cell cell)
        {
            return GetHexagonCorners(cell.x, cell.y);
        }
        
        public PointF[] GetHexagonCorners(int x, int y)
        {
            var coordinates = HexGrid.ToCubeCoordinates(new Hex.OffsetCoordinates(x, y));
            return HexGrid.PolygonCorners(coordinates).Select(c => c.ConvertToDrawingPointF()).ToArray();
        }

        public double GetAngle(int sourceCellId, int targetCellId, int activePlayerId)
        {
            double angle;

            double shipx = Cells[sourceCellId].CellCenter.X;
            double shipy = Cells[sourceCellId].CellCenter.Y;
            double targetx = Cells[targetCellId].CellCenter.X;
            double targety = Cells[targetCellId].CellCenter.Y;

            if (shipx == targetx) // избегаем деления на ноль
            {
                if (shipy > targety)
                {
                    angle = -90;
                }
                else
                {
                    angle = 90;
                }
                if (activePlayerId == 2) angle = -angle;

            }
            else // находим угол, на который нужно повернуть корабль (если он не равен 90 градусов)
            {
                angle = Math.Atan((targety - shipy) / (targetx - shipx)) * 180 / Math.PI;
            }
            // дальше идет коррекция, не пытайся разобраться как это работает, просто оставь как есть
            if (activePlayerId == 1)
            {
                if (shipy == targety && shipx > targetx)
                {
                    angle = 180;
                }
                else if (shipx > targetx && shipy < targety)
                {
                    angle += 180;
                }
                else if (shipx > targetx && shipy > targety)
                {
                    angle = angle - 180;
                }
            }
            else if (activePlayerId == 2)
            {
                if (shipy == targety && shipx < targetx)
                {
                    angle = 180;
                }
                else if (shipx < targetx && shipy < targety)
                {
                    angle -= 180;
                }
                else if (shipx < targetx && shipy > targety)
                {
                    angle += 180;
                }
            }

            if (angle > 150) angle = 150;
            else if (angle < -150) angle = -150;
            return angle;
        }

        public void PlaceShip(Ship ship)
        {
            var rand = new Random();
            int randomBoxId;
            do
            {
                randomBoxId = rand.Next(0, FieldHeight*2);
                if (ship.player == 2)
                {
                    randomBoxId = Cells.Count - randomBoxId - 1;
                }
            } while (Cells[randomBoxId].spaceObject != null);

            Cells[randomBoxId].spaceObject = ship;
            ship.boxId = randomBoxId;
        }

        private void InitializeMap()
        {
            for (int i = 0; i < FieldWidth; i++)
            {
                for (int j = 0; j < FieldHeight; j++)
                {
                    Cells.Add(new Cell(CellSideLength, i, j, i * FieldHeight + j,
                        new Size((int)(CellSideLength + 10), (int)(Math.Sin(Math.PI / 3) * CellSideLength + 10))));
                }
            }
        }
    }

 
}
