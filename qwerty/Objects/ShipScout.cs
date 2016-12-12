using System;
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
            $"AP - {equippedWeapon.attackPower}{Environment.NewLine}" +
            $"Range - {equippedWeapon.attackRange}";

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
                    new Point(cMap.boxes[boxId].xcenter + xpoints[0], cMap.boxes[boxId].ycenter + ypoints[0]),
                    new Point(cMap.boxes[boxId].xcenter + xpoints[1], cMap.boxes[boxId].ycenter + ypoints[1]),
                    new Point(cMap.boxes[boxId].xcenter + xpoints[2], cMap.boxes[boxId].ycenter + ypoints[2]),
                    };
            g.FillPolygon(generalBrush, myPointArray);
            g.DrawString(actionsLeft.ToString(), new Font("Arial", 8.0F), Brushes.Blue, new PointF(cMap.boxes[boxId].xpoint1 + 25, cMap.boxes[boxId].ypoint1 + 15));
            g.DrawString(currentHealth.ToString(), new Font("Arial", 8.0F), Brushes.Red, new PointF(cMap.boxes[boxId].xpoint1 + 20, cMap.boxes[boxId].ypoint1 - 25));
        }

        public ShipScout(int p, WeaponType weaponType) : base(weaponType)
        {
            objectType = ObjectType.Ship;

            player = p;
            maxHealth = 50;
            currentHealth = maxHealth;
            maxActions = 3;
            actionsLeft = maxActions;

            // координаты точек относительно центра ячейки
            xpoints.Add(-15);
            xpoints.Add(-15);
            xpoints.Add(17);
            // лишние точки

            ypoints.Add(-14);
            ypoints.Add(14);
            ypoints.Add(0);

            PolygonPoints = new[]
            {
                new PointF(-15, -14),
                new PointF(-15, 14),
                new PointF(17, 0)
            };
     
            // лишние точки

            if (player == 2)
            {
                shipRotate(180);
            }

            weaponPointX = xpoints[2];
            weaponPointY = ypoints[2];

        }
    }
}
