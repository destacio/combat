using System;
using System.Drawing;
using qwerty.Objects.Weapons;

namespace qwerty.Objects
{
    class ShipAssaulter : Ship
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

            Point[] myPointArray = {
                    new Point(cMap.Cells[boxId].xcenter + xpoints[0], cMap.Cells[boxId].ycenter + ypoints[0]),
                    new Point(cMap.Cells[boxId].xcenter + xpoints[1], cMap.Cells[boxId].ycenter + ypoints[1]),
                    new Point(cMap.Cells[boxId].xcenter + xpoints[2], cMap.Cells[boxId].ycenter + ypoints[2]),
                    new Point(cMap.Cells[boxId].xcenter + xpoints[3], cMap.Cells[boxId].ycenter + ypoints[3]),
                    new Point(cMap.Cells[boxId].xcenter + xpoints[4], cMap.Cells[boxId].ycenter + ypoints[4])
                    };
            g.FillPolygon(generalBrush, myPointArray);
            g.DrawString(actionsLeft.ToString(), new Font("Arial", 8.0F), Brushes.Blue, new PointF(cMap.Cells[boxId].xpoint1 + 25, cMap.Cells[boxId].ypoint1 + 15));
            g.DrawString(currentHealth.ToString(), new Font("Arial", 8.0F), Brushes.Red, new PointF(cMap.Cells[boxId].xpoint1 + 20, cMap.Cells[boxId].ypoint1 - 25));
        }

        public ShipAssaulter(int p, WeaponType weaponType):base(weaponType)
        {
            objectType = ObjectType.Ship;

            player = p;
            maxHealth = 100;
            currentHealth = maxHealth;
            maxActions = 2;
            actionsLeft = maxActions;

            xpoints.Add(-16); // координаты точек относительно центра ячейки
            xpoints.Add(6);
            xpoints.Add(18);
            xpoints.Add(6);
            xpoints.Add(-16);

            // лишние точки

            ypoints.Add(-15);
            ypoints.Add(-10);
            ypoints.Add(0);
            ypoints.Add(10);
            ypoints.Add(15);

            PolygonPoints = new[]
            {
                new PointF(-16, -15),
                new PointF(6, -10),
                new PointF(18, 0),
                new PointF(6, 10),
                new PointF(-16, 15),  
            };

            if (player == 2)
            {
                Rotate(180);
            }
            weaponPointX = xpoints[2];
            weaponPointY = ypoints[2];
        }
    }
}
