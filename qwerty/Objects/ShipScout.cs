using System;
using System.Collections.Generic;
using System.Drawing;
using qwerty.Objects.Weapons;

namespace qwerty.Objects
{
    class ShipScout : Ship
    {
        private static readonly string StaticDescription = $"Лёгкий корабль{Environment.NewLine}класса Scout";

        public override string Description =>
            $"{StaticDescription}{Environment.NewLine}" +
            $"hp - {currentHealth}/{maxHealth}{Environment.NewLine}" +
            $"actions - {actionsLeft}/{maxActions}{Environment.NewLine}" +
            $"AP - {EquippedWeapon.attackPower}{Environment.NewLine}" +
            $"Range - {EquippedWeapon.attackRange}";

        public override void drawSpaceShit(ref CombatMap cMap, ref System.Drawing.Bitmap bmap)
        {
            Graphics g = Graphics.FromImage(bmap);
            
            SolidBrush generalBrush;

            if (player == 1)
                generalBrush = new SolidBrush(Color.Blue);
            else if (player == 2)
                generalBrush = new SolidBrush(Color.Red);
            else
                generalBrush = new SolidBrush(Color.Gray);

            /*Point[] myPointArray = {
                    new Point(cMap.Cells[boxId].xcenter + xpoints[0], cMap.Cells[boxId].ycenter + ypoints[0]),
                    new Point(cMap.Cells[boxId].xcenter + xpoints[1], cMap.Cells[boxId].ycenter + ypoints[1]),
                    new Point(cMap.Cells[boxId].xcenter + xpoints[2], cMap.Cells[boxId].ycenter + ypoints[2]),
                    };
*/
            PointF[] myPointArray = {
                PointF.Add(PolygonPoints[0],  new SizeF(cMap.Cells[boxId].xcenter, cMap.Cells[boxId].ycenter)),
                PointF.Add(PolygonPoints[1], new SizeF(cMap.Cells[boxId].xcenter, cMap.Cells[boxId].ycenter)),
                PointF.Add(PolygonPoints[2],new SizeF(cMap.Cells[boxId].xcenter, cMap.Cells[boxId].ycenter))
            };
            g.FillPolygon(generalBrush, myPointArray);
            g.DrawString(actionsLeft.ToString(), new Font("Arial", 8.0F), Brushes.Blue, new PointF(cMap.Cells[boxId].xpoint1 + 25, cMap.Cells[boxId].ypoint1 + 15));
            g.DrawString(currentHealth.ToString(), new Font("Arial", 8.0F), Brushes.Red, new PointF(cMap.Cells[boxId].xpoint1 + 20, cMap.Cells[boxId].ypoint1 - 25));
        }

        public ShipScout(int playerId, WeaponType weaponType) : base(playerId, weaponType)
        {
            maxHealth = 50;
            currentHealth = maxHealth;
            maxActions = 3;
            actionsLeft = maxActions;

            // координаты точек относительно центра ячейки
            /*xpoints.Add(-15);
            xpoints.Add(-15);
            xpoints.Add(17);
            // лишние точки

            ypoints.Add(-14);
            ypoints.Add(14);
            ypoints.Add(0);*/

            PolygonPoints = new List<PointF>
            {
                new PointF(-15, -14),
                new PointF(-15, 14),
                new PointF(17, 0)
            };
     
            // лишние точки

            /*weaponPointX = xpoints[2];
            weaponPointY = ypoints[2];*/
            WeaponPoint = PolygonPoints[2];

            if (player == 2)
            {
                Rotate(180);
            }
        }
    }
}
