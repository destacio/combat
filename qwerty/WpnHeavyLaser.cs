using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace qwerty
{
    class WpnHeavyLaser : Weapon
    {
        public WpnHeavyLaser()
        {
            attackPower = 50;
            attackRange = 5;
            energyСonsumption = 1;
        }
        public override string description()
        {
            return "";
        }
        public override void drawAttack(int x, int y, int targetx, int targety, ref System.Drawing.Bitmap bmap, System.Media.SoundPlayer player, ref PictureBox pictureMap)
        {
            System.Threading.Thread.Sleep(150);
            player.SoundLocation = @"../../Sounds/laser1.wav";

            Graphics g = Graphics.FromImage(bmap);
            
            Pen laserPen1 = new Pen(Color.Orange, 3);

            player.Play();
            for (int i = -2; i < 2; i++)
            {
                g.DrawLine(laserPen1, new Point(x, y), new Point(targetx + i, targety));

                pictureMap.Image = bmap;
                pictureMap.Refresh();

                System.Threading.Thread.Sleep(35);
            }
        }
    }
}
