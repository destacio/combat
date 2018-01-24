using System.Collections.Generic;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace qwerty.Objects.Weapons
{
    class LightIon : Weapon
    {
        public override Color AttackColorPrimary => Color.CadetBlue;
        public override Color AttackColorSecondary => Color.CornflowerBlue;

        public LightIon()
        {
            attackPower = 18;
            attackRange = 4;
            energyСonsumption = 1;
        }

        public override string Description => "";
    
        public override List<Bitmap> GetAttackSprites(PointF sourcePoint, PointF targetPoint)
        {
            System.Threading.Thread.Sleep(150);
            soundPlayer.SoundLocation = @"../../Sounds/laser1.wav";

            Graphics g = Graphics.FromImage(bitmap);
            Rectangle rect;  //  --- размер изображения
            Bitmap oldImage;  //  --- переменная, в которую его засунем
            soundPlayer.Play();

            SolidBrush brush1 = new SolidBrush(Color.CadetBlue);
            SolidBrush brush = new SolidBrush(Color.CornflowerBlue);

            // TODO: review to match points
            var dx = (targetPoint.X - sourcePoint.X) / 10;
            var dy = (targetPoint.Y - sourcePoint.Y) / 10;

            for (int i = 0; i < 10; i++)
            {
                // --- 1) находим размер изображения
                rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height); 
                // --- 2) клонируем наш битмап
                oldImage = bitmap.Clone(rect, bitmap.PixelFormat);

                g.FillEllipse(brush, sourcePoint.X - 5 + dx * i, sourcePoint.Y - 5 + dy * i, 10, 10);
                g.FillEllipse(brush1, sourcePoint.X - 3 + dx * i, sourcePoint.Y - 3 + dy * i, 6, 6);

                pictureBox.Image = bitmap;
                pictureBox.Refresh();

                // --- 3) отрисовываем тот битмам, который сохранили выше
                g.DrawImage(oldImage, 0, 0);

                //System.Threading.Thread.Sleep(15);
            } 
        }
    }
}
