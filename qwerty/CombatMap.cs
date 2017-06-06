using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using qwerty.Objects;

namespace qwerty
{
    class CombatMap
    {
        public int FieldWidth;
        public int FieldHeight;
        private const float CellSideLength = 40;
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
                return (int)(Cells.Last().CellPoints.Max(cell => cell.X) + 10);
            }
        }

        public int FieldHeightPixels
        {
            get
            {
                return (int)(Cells.Last().CellPoints.Max(cell => cell.Y) + 10);
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
            // TODO: better check if point is inside polygon
            return
                Cells.FirstOrDefault(
                    cell => x > cell.CellPoints[2].X && x < cell.CellPoints[1].X && y > cell.CellPoints[5].Y && y < cell.CellPoints[1].Y);
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
