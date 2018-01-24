using System;
using System.Collections.Generic;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace qwerty.Objects.Weapons
{
    class LightLaser : Weapon
    {
        public override Color AttackColorPrimary => Color.GreenYellow;
        public override Color AttackColorSecondary => Color.GreenYellow;

        public LightLaser()
        {
            attackPower = 25;
            attackRange = 3;
            energyСonsumption = 1;
        }
        public override string Description => ""; 

        public override List<Bitmap> GetAttackSprites(PointF sourcePoint, PointF targetPoint)
        {
            Pen laserPen1 = new Pen(Color.GreenYellow, 2);
            List<Bitmap> sprites = new List<Bitmap>();
            for (int i = 0; i < 5; i++)
            {
                var sprite = new Bitmap((int) Math.Max(sourcePoint.X, targetPoint.X), (int) Math.Max(sourcePoint.Y, targetPoint.Y));
                var g = Graphics.FromImage(sprite);

                g.DrawLine(laserPen1, sourcePoint, PointF.Add(targetPoint, new Size(i,0)));

                sprites.Add(sprite);
            }
            return sprites;
        }
    }
}
