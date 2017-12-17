using System.Drawing;
using System.Drawing.Imaging;

namespace qwerty
{
    class FieldPainter
    {
        public Bitmap CurrentBitmap;

        private ObjectManager objectManager;
        private CombatMap combatMap => objectManager.CombatMap;

        public FieldPainter(int fieldWidth, int fieldHeight, ObjectManager objectManager)
        {
            this.objectManager = objectManager;
            CurrentBitmap = new Bitmap(fieldWidth, fieldHeight, PixelFormat.Format32bppArgb);
        }
        
        public void DrawField()
        {
            Graphics g = Graphics.FromImage(CurrentBitmap);
            g.FillRectangle(Brushes.Black, 0, 0, CurrentBitmap.Width, CurrentBitmap.Height); //рисуем фон окна

            foreach (var hexagonCorners in combatMap.AllHexagonCorners)
            {
                g.DrawPolygon(Pens.Purple, hexagonCorners);
            }
            
            // highlight active ship
            if (objectManager.ActiveShip != null)
            {
                Pen activeShipAriaPen = new Pen(Color.Purple, 5);
                g.DrawPolygon(activeShipAriaPen,
                    combatMap.GetHexagonCorners(objectManager.ActiveShip.ObjectCoordinates.Column,
                        objectManager.ActiveShip.ObjectCoordinates.Row));
            }
            
            // highlight active ship attack range
            // Pen redPen = new Pen(Color.Red, 3);
            // g.DrawPolygon(redPen, cMap.GetHexagonCorners(cMap.Cells[allShips[count].boxId]));
            
            // draw ships
//                if (cMap.Cells[i].spaceObject != null)
//                {
//                    // this call is as dumb as it could ever be
//                    _objectManager.drawSpaceShit(i, ref CurrentBitmap);
//
//                }
//
                foreach (var ship in objectManager.Ships)
                {
                    objectManager.drawSpaceShit(ship, ref CurrentBitmap);
                }
//#if DEBUG
//                g.DrawString(cMap.Cells[i].id.ToString(), new Font("Arial", 8.0F), Brushes.Yellow, PointF.Add(cMap.Cells[i].CellPoints[3], new Size(10, 10)));
//                g.DrawString($"({cMap.Cells[i].x}, {cMap.Cells[i].y})", new Font("Arial", 8.0F), Brushes.DeepSkyBlue, PointF.Add(cMap.Cells[i].CellPoints[3], new Size(30, 10)));
//#endif
        } 
    }
}