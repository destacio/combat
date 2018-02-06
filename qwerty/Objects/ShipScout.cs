﻿using System;
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
            $"hp - {this.currentHealth}/{this.maxHealth}{Environment.NewLine}" +
            $"actions - {this.actionsLeft}/{this.maxActions}{Environment.NewLine}" +
            $"AP - {this.EquippedWeapon.attackPower}{Environment.NewLine}" +
            $"Range - {this.EquippedWeapon.attackRange}";

        public ShipScout(Player playerId, WeaponType weaponType) : base(playerId, weaponType)
        {
            this.maxHealth = 50;
            this.currentHealth = this.maxHealth;
            this.maxActions = 3;
            this.actionsLeft = this.maxActions;

            this.PolygonPoints = new List<PointF>
            {
                new PointF(-15, -14),
                new PointF(-15, 14),
                new PointF(17, 0)
            };

            this.WeaponPoint = this.PolygonPoints[2];

            if (this.Owner == Player.SecondPlayer)
            {
                this.Rotate(180);
            }
        }
    }
}
