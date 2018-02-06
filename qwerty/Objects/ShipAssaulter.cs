﻿using System;
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
            $"hp - {this.currentHealth}/{this.maxHealth}{Environment.NewLine}" +
            $"actions - {this.actionsLeft}/{this.maxActions}{Environment.NewLine}" +
            $"AP - {this.EquippedWeapon.attackPower}{Environment.NewLine}" +
            $"Range - {this.EquippedWeapon.attackRange}";

        public ShipAssaulter(Player playerId, WeaponType weaponType) : base(playerId, weaponType)
        {
            this.maxHealth = 100;
            this.currentHealth = this.maxHealth;
            this.maxActions = 2;
            this.actionsLeft = this.maxActions;

            this.PolygonPoints = new List<PointF>
            {
                new PointF(-16, -15),
                new PointF(6, -10),
                new PointF(18, 0),
                new PointF(6, 10),
                new PointF(-16, 15),  
            };

            this.WeaponPoint = this.PolygonPoints[2];

            if (this.Owner == Player.SecondPlayer)
            {
                this.Rotate(180);
            }
        }
    }
}
