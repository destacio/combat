using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using qwerty.Objects;

namespace qwerty
{
    class Cell
    {
        public SpaceObject spaceObject = null;
        public int id;
        public int x;
        public int y;

        public PointF[] CellPoints;
        public PointF CellCenter;

        public Cell(float sideLength, int cellX, int cellY, int cellId, Size fieldOffset = default(Size))
        {
            x = cellX;
            y = cellY;
            id = cellId;

            float xCoord = (float)Math.Truncate(sideLength*Math.Cos(Math.PI/3));
            float yCoord = (float)Math.Truncate(sideLength*Math.Sin(Math.PI/3));
            CellPoints = new[]
            {
                new PointF(sideLength, 0),
                new PointF(xCoord, yCoord),
                new PointF(-xCoord, yCoord),
                new PointF(-sideLength, 0),
                new PointF(-xCoord, -yCoord),
                new PointF(xCoord, -yCoord)
            };

            Size cellOffset = new Size((int)(cellX * (2 * sideLength - xCoord)), (int)(cellY * 2 * yCoord + (cellX % 2 == 0 ? 0 : yCoord)));
            for (int i = 0; i < CellPoints.Length; i++)
            {
                CellPoints[i] = PointF.Add(CellPoints[i], fieldOffset + cellOffset);
            }
            
            CellCenter = new PointF((CellPoints[1].X + CellPoints[2].X)/2, (CellPoints[1].Y + CellPoints[5].Y)/2);
        }
    }
}
