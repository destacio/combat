using System;
using System.Collections.Generic;
using System.Drawing;
using qwerty.Objects.Weapons;

namespace qwerty.Objects
{
    class ShipScout : Ship
    {
        private static readonly string StaticDescription = $"Лёгкий корабль{Environment.NewLine}класса Scout";

        public override string Description =>
            $"{StaticDescription}{Environment.NewLine}" +
            $"hp - {currentHealth}/{maxHealth}{Environment.NewLine}" +
            $"actions - {actionsLeft}/{maxActions}{Environment.NewLine}" +
            $"AP - {EquippedWeapon.attackPower}{Environment.NewLine}" +
            $"Range - {EquippedWeapon.attackRange}";

        public ShipScout(Player playerId, WeaponType weaponType) : base(playerId, weaponType)
        {
            maxHealth = 50;
            currentHealth = maxHealth;
            maxActions = 3;
            actionsLeft = maxActions;

            PolygonPoints = new List<PointF>
            {
                new PointF(-15, -14),
                new PointF(-15, 14),
                new PointF(17, 0)
            };

            WeaponPoint = PolygonPoints[2];

            if (player == Player.SecondPlayer)
            {
                Rotate(180);
            }
        }
    }
}
