using System.Drawing;
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

        public override void drawAttack(int x, int y, int targetx, int targety, ref System.Drawing.Bitmap bmap, System.Media.SoundPlayer player, ref PictureBox pictureMap)
        {
            System.Threading.Thread.Sleep(150);
            player.SoundLocation = @"../../Sounds/laser1.wav";

            Graphics g = Graphics.FromImage(bmap);
            //Rectangle rect;  //  --- размер изображения
            //Bitmap oldImage;  //  --- переменная, в которую его засунем

            Pen laserPen1 = new Pen(Color.GreenYellow, 2);

            player.Play();
            for (int i = 0; i < 5; i++)
            {
                // --- 1) находим размер изображения
                //rect = new Rectangle(0, 0, combatBitmap.Width, combatBitmap.Height); 
                // --- 2) клонируем наш битмап
                //oldImage = combatBitmap.Clone(rect, combatBitmap.PixelFormat);

                g.DrawLine(laserPen1, new Point(x, y), new Point(targetx + i, targety));

                pictureMap.Image = bmap;
                pictureMap.Refresh();

                // --- 3) отрисовываем тот битмам, который сохранили выше
                //g.DrawImage(oldImage, 0, 0);


                System.Threading.Thread.Sleep(35);
            }
        }
    }
}
