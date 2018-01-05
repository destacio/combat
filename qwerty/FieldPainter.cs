using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using qwerty.Objects;

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
            g.FillRectangle(Brushes.Black, 0, 0, CurrentBitmap.Width, CurrentBitmap.Height);

            foreach (var hexagonCorners in combatMap.AllHexagonCorners)
            {
                g.DrawPolygon(Pens.Purple, hexagonCorners);
            }
            
            if (objectManager.ActiveShip != null)
            {
				// highlight active ship attack range
				Pen redPen = new Pen(Color.Red, 1);
				foreach (var hexagonCorners in combatMap.GetAllHexagonCornersInRange(objectManager.ActiveShip.ObjectCoordinates, objectManager.ActiveShip.EquippedWeapon.attackRange))
				{
					g.DrawPolygon(redPen, hexagonCorners);
				}

				// highlight active ship
				Pen activeShipAriaPen = new Pen(Color.Purple, 5);
				g.DrawPolygon(activeShipAriaPen,
				              combatMap.GetHexagonCorners(objectManager.ActiveShip.ObjectCoordinates.Column,
				                                          objectManager.ActiveShip.ObjectCoordinates.Row));
            }
            
            
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
				DrawShip(ship);
            }
//#if DEBUG
//                g.DrawString(cMap.Cells[i].id.ToString(), new Font("Arial", 8.0F), Brushes.Yellow, PointF.Add(cMap.Cells[i].CellPoints[3], new Size(10, 10)));
//                g.DrawString($"({cMap.Cells[i].x}, {cMap.Cells[i].y})", new Font("Arial", 8.0F), Brushes.DeepSkyBlue, PointF.Add(cMap.Cells[i].CellPoints[3], new Size(30, 10)));
//#endif
        }

		private void DrawShip(Ship ship)
		{
			Graphics g = Graphics.FromImage(CurrentBitmap);

			SolidBrush generalBrush;

			if (ship.player == Player.FirstPlayer)
                generalBrush = new SolidBrush(Color.Blue);
            else if (ship.player == Player.SecondPlayer)
                generalBrush = new SolidBrush(Color.Red);
            else
                generalBrush = new SolidBrush(Color.Gray);

			var myPointArray = ship.PolygonPoints.Select(p => PointF.Add(p, new Size(combatMap.HexToPixel(ship.ObjectCoordinates)))).ToArray();
			g.FillPolygon(generalBrush, myPointArray);
			g.DrawString(ship.actionsLeft.ToString(), new Font("Arial", 8.0F), Brushes.Blue, Point.Add(combatMap.HexToPixel(ship.ObjectCoordinates), new Size(0, 15)));
            g.DrawString(ship.currentHealth.ToString(), new Font("Arial", 8.0F), Brushes.Red, PointF.Add(combatMap.HexToPixel(ship.ObjectCoordinates), new Size(0, -25)));
		}
    }
}