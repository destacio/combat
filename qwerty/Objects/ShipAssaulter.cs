using System;
using System.Collections.Generic;
using System.Drawing;
using qwerty.Objects.Weapons;

namespace qwerty.Objects
{
    class ShipAssaulter : Ship
    {
        private static readonly string StaticDescription = $"Лёгкий корабль{Environment.NewLine}класса Scout";

        public override string Description =>
            $"{StaticDescription}{Environment.NewLine}" +
            $"hp - {currentHealth}/{maxHealth}{Environment.NewLine}" +
            $"actions - {actionsLeft}/{maxActions}{Environment.NewLine}" +
            $"AP - {EquippedWeapon.attackPower}{Environment.NewLine}" +
            $"Range - {EquippedWeapon.attackRange}";

        public ShipAssaulter(Player playerId, WeaponType weaponType) : base(playerId, weaponType)
        {
            maxHealth = 100;
            currentHealth = maxHealth;
            maxActions = 2;
            actionsLeft = maxActions;

            PolygonPoints = new List<PointF>
            {
                new PointF(-16, -15),
                new PointF(6, -10),
                new PointF(18, 0),
                new PointF(6, 10),
                new PointF(-16, 15),  
            };

            WeaponPoint = PolygonPoints[2];

            if (player == Player.SecondPlayer)
            {
                Rotate(180);
            }
        }
    }
}
