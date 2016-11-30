using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qwerty
{
    public enum ObjectType
    {
        Ship, Meteor
    }

    public enum ShipType
    {
        Scout, Assaulter
    }

    public enum WeaponType
    {
        LightLaser, HeavyLaser, LightIon
    }

    static class Constants
    {
        // направления
        public const int NORMAL = 0;
        public const int LEFT = -1;
        public const int RIGHT = 1;
        public const int TOP = -2;
        public const int MEDIUM_TOP = -1;
        public const int BOTTOM = 2;
        public const int MEDIUM_BOTTOM = 1;
    }
}
