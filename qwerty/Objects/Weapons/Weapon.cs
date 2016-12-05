using System.Drawing;
using System.Windows.Forms;

namespace qwerty.Objects.Weapons
{
    public enum WeaponType
    {
        LightLaser, HeavyLaser, LightIon
    }

    abstract class Weapon
    {
        public abstract string Description { get; }
        public abstract void drawAttack(int x, int y, int targetx, int targety, ref System.Drawing.Bitmap bmap, System.Media.SoundPlayer player, ref PictureBox pictureMap);

        public abstract Color AttackColorPrimary { get; }
        public abstract Color AttackColorSecondary { get; }

        public int attackRange;
        public int attackPower;
        public int energyСonsumption;
    }
    
}
