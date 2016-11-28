using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace qwerty
{
    class ShipAssaulter : Ship
    {
        public string staticDescription;
        public override string Description => "" + staticDescription + "\nhp - " + currentHealth + "/" + maxHealth + "\nactions - "
                                              + actionsLeft + "/" + maxActions + "\nAP - " + equippedWeapon.attackPower + "\nRange - " +
                                              equippedWeapon.attackRange;

        public override void drawSpaceShit(ref combatMap cMap, ref System.Drawing.Bitmap bmap)
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
                    new Point(cMap.boxes[boxId].xcenter + xpoints[3], cMap.boxes[boxId].ycenter + ypoints[3]),
                    new Point(cMap.boxes[boxId].xcenter + xpoints[4], cMap.boxes[boxId].ycenter + ypoints[4])
                    };
            g.FillPolygon(generalBrush, myPointArray);
            g.DrawString(actionsLeft.ToString(), new Font("Arial", 8.0F), Brushes.Blue, new PointF(cMap.boxes[boxId].xpoint1 + 25, cMap.boxes[boxId].ypoint1 + 15));
            g.DrawString(currentHealth.ToString(), new Font("Arial", 8.0F), Brushes.Red, new PointF(cMap.boxes[boxId].xpoint1 + 20, cMap.boxes[boxId].ypoint1 - 25));
        }

        public ShipAssaulter(int p, Constants.WeaponType weaponType):base(weaponType)
        {
            objectType = Constants.SHIP;

            player = p;
            maxHealth = 100;
            currentHealth = maxHealth;
            maxActions = 2;
            actionsLeft = maxActions;
            staticDescription = "Лёгкий корабль\nкласса Scout";

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

            if (player == 2)
            {
                shipRotate(180);
            }
            weaponPointX = xpoints[2];
            weaponPointY = ypoints[2];
        }
    }
}
