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
        public readonly int FieldWidth;
        public readonly int FieldHeight;
        private const float CellSideLength = 40;

        public Hex.HexLayout<Hex.Point, Hex.PointPolicy> HexGrid = Hex.HexLayoutFactory.CreateFlatHexLayout(
            new Hex.Point(CellSideLength, CellSideLength), new Hex.Point((int)(CellSideLength + 10),  (int)(Math.Sin(Math.PI / 3) * CellSideLength + 10)),
            Hex.Offset.Odd);
        
        public readonly List<Cell> Cells = new List<Cell>();
        public CombatMap(int w, int h) 
        {
            FieldWidth = w;
            FieldHeight = h;
            InitializeMap();
        }

        public int FieldWidthPixels
        {
            get
            {
                var hexagonOffset =
                    HexGrid.HexToPixel(HexGrid.ToCubeCoordinates(new Hex.OffsetCoordinates(FieldWidth - 1, FieldHeight - 1)));
                var cornerOffset = HexGrid.HexCornerOffset(0);
                return (int)(hexagonOffset.X + cornerOffset.X);
            }
        }

        public int FieldHeightPixels
        {
            get
            {
                var hexagonOffset =
                    HexGrid.HexToPixel(HexGrid.ToCubeCoordinates(new Hex.OffsetCoordinates(FieldWidth - 1, FieldHeight - 1)));
                var cornerOffset = HexGrid.HexCornerOffset(5);
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

        public Point HexToPixel(int x, int y)
        {
            var cubeCoordinates = HexGrid.ToCubeCoordinates(new Hex.OffsetCoordinates(x,y));
            return HexGrid.HexToPixel(cubeCoordinates).ConvertToDrawingPoint();
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

        public List<PointF[]> AllHexagonCorners
        {
            get
            {
                var cornerPointList = new List<PointF[]>();
                for (int x = 0; x < FieldWidth; x++)
                {
                    for (int y = 0; y < FieldHeight; y++)
                    {
                        cornerPointList.Add(GetHexagonCorners(x, y));
                    }
                }
                return cornerPointList;
            }
        }

        public double GetAngle(int sourceCellId, int targetCellId)
        {
            var sourceOffsetCoordinates = new Hex.OffsetCoordinates(Cells[sourceCellId].x, Cells[sourceCellId].y);
            double shipx = HexGrid.HexToPixel(HexGrid.ToCubeCoordinates(sourceOffsetCoordinates)).X;
            double shipy = HexGrid.HexToPixel(HexGrid.ToCubeCoordinates(sourceOffsetCoordinates)).Y;
            var targetOffsetCoordinates = new Hex.OffsetCoordinates(Cells[targetCellId].x, Cells[targetCellId].y);
            double targetx = HexGrid.HexToPixel(HexGrid.ToCubeCoordinates(targetOffsetCoordinates)).X;
            double targety = HexGrid.HexToPixel(HexGrid.ToCubeCoordinates(targetOffsetCoordinates)).Y;
            return Math.Atan2(targety - shipy, targetx - shipx) * 180 / Math.PI;
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

        public Hex.OffsetCoordinates PixelToOffsetCoordinates(Point pixelCoordinates)
        {
            var cubeCoordinates = HexGrid.PixelToHex(pixelCoordinates.ConvertToHexPoint()).Round();
            var offsetCoordinates = HexGrid.ToOffsetCoordinates(cubeCoordinates);
            if (offsetCoordinates.Column < 0 || offsetCoordinates.Column > FieldWidth ||
                offsetCoordinates.Row < 0 || offsetCoordinates.Row > FieldHeight)
            {
                throw new ArgumentOutOfRangeException($"Pixel ({pixelCoordinates.X},{pixelCoordinates.Y}) is outside game field.");
            }
            return offsetCoordinates;
        }

        public int GetDistance(Hex.CubeCoordinates firstHexagon, Hex.CubeCoordinates secondHexagon)
        {
            return Hex.CubeCoordinates.Distance(firstHexagon,secondHexagon);
        }
        
        public int GetDistance(Hex.OffsetCoordinates firstHexagon, Hex.OffsetCoordinates secondHexagon)
        {
            return Hex.CubeCoordinates.Distance(HexGrid.ToCubeCoordinates(firstHexagon),HexGrid.ToCubeCoordinates(secondHexagon));
        }

        public Point HexToPixel(Hex.OffsetCoordinates objectCoordinates)
        {
            return HexGrid.HexToPixel(HexGrid.ToCubeCoordinates(objectCoordinates)).ConvertToDrawingPoint();
        }
    } 
}
