using System;
using System.Collections.Generic;
using System.Drawing;
using qwerty.Objects;

namespace qwerty
{
    class CombatMap
    {
        public int FieldWidth;
        public int FieldHeight;
        private const float CellSideLength = 40;
        public List<Cell> Cells = new List<Cell>(); 
        public CombatMap(int w, int h) 
        {
            FieldWidth = w;
            FieldHeight = h;

            InitializeMap();
        }

        public Cell GetCellByCoordinates(int x, int y)
        {
            if (x < 0 || y < 0 || x*FieldHeight + y > FieldWidth*FieldHeight)
            {
                return null;
            }

            return Cells[x*FieldHeight + y];
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
                    randomBoxId = Cells.Count - randomBoxId;
                }
            } while (Cells[randomBoxId].spaceObject != null);

            Cells[randomBoxId].spaceObject = ship;
            ship.boxId = randomBoxId;
        }

        private void InitializeMap()
        {
            for(int i = 0; i < FieldWidth; i++)
            {
                for (int j = 0; j < FieldHeight; j++)
                {
                    Cells.Add(new Cell(CellSideLength, i, j, i*FieldHeight + j,
                        new Size((int)(CellSideLength + 10), (int) (Math.Sin(Math.PI/3)*CellSideLength + 10))));
                }
            }
        }
    }

 
}
