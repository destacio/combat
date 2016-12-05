using System.Windows.Forms;

namespace qwerty.Objects.Weapons
{
    public enum WeaponType
    {
        LightLaser, HeavyLaser, LightIon
    }

    abstract class Weapon
    {
        abstract public string description();
        abstract public void drawAttack(int x, int y, int targetx, int targety, ref System.Drawing.Bitmap bmap, System.Media.SoundPlayer player, ref PictureBox pictureMap);
        public int attackRange;
        public int attackPower;
        public int energyСonsumption;
    }
    
}
