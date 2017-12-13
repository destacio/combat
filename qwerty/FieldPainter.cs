using System.Drawing;
using System.Drawing.Imaging;

namespace qwerty
{
    public class FieldPainter
    {
        public Bitmap CurrentBitmap;

        public FieldPainter(int fieldWidth, int fieldHeight)
        {
            CurrentBitmap = new Bitmap(fieldWidth, fieldHeight, PixelFormat.Format32bppArgb);
        }
    }
}