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

        public override void drawAttack(PointF sourcePoint, PointF targetPoint, ref Bitmap bitmap, SoundPlayer soundPlayer, ref PictureBox pictureBox)
        {
            System.Threading.Thread.Sleep(150);
            soundPlayer.SoundLocation = @"../../Sounds/laser1.wav";

            Graphics g = Graphics.FromImage(bitmap);
            Pen laserPen1 = new Pen(Color.GreenYellow, 2);

            soundPlayer.Play();
            for (int i = 0; i < 5; i++)
            {
                g.DrawLine(laserPen1, sourcePoint, PointF.Add(targetPoint, new Size(i,0)));

                pictureBox.Image = bitmap;
                pictureBox.Refresh();

                System.Threading.Thread.Sleep(35);
            }
        }
    }
}
