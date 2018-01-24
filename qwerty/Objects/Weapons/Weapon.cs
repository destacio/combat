using System.Collections.Generic;
using System.Drawing;
using System.Media;
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
        public abstract List<Bitmap> GetAttackSprites(PointF sourcePoint, PointF targetPoint);

        public abstract Color AttackColorPrimary { get; }
        public abstract Color AttackColorSecondary { get; }

        public int attackRange;
        public int attackPower;
        public int energyСonsumption;
    }
    
}
