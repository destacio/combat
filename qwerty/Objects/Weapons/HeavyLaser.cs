using System.Collections.Generic;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace qwerty.Objects.Weapons
{
    class HeavyLaser : Weapon
    {
        public HeavyLaser()
        {
            attackPower = 50;
            attackRange = 5;
            energyСonsumption = 1;
        }

        public override string Description=> "";

        public override Color AttackColorPrimary => Color.Orange;
        public override Color AttackColorSecondary => Color.Orange;

        public override List<Bitmap> GetAttackSprites(PointF sourcePoint, PointF targetPoint)
        {
            System.Threading.Thread.Sleep(150);
            soundPlayer.SoundLocation = @"../../Sounds/laser1.wav";

            Graphics g = Graphics.FromImage(bitmap);
            
            Pen laserPen1 = new Pen(Color.Orange, 3);

            soundPlayer.Play();
            for (int i = -2; i < 2; i++)
            {
                g.DrawLine(laserPen1, sourcePoint, PointF.Add(targetPoint, new Size(i,0)));

                pictureBox.Image = bitmap;
                pictureBox.Refresh();

                System.Threading.Thread.Sleep(35);
            }
        }
    }
}
