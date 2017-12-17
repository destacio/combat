﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        public override void drawSpaceShit(ref CombatMap cMap, ref System.Drawing.Bitmap bmap)
        {
            Graphics g = Graphics.FromImage(bmap);

            SolidBrush generalBrush;

            if (player == Player.FirstPlayer)
                generalBrush = new SolidBrush(Color.Blue);
            else if (player == Player.SecondPlayer)
                generalBrush = new SolidBrush(Color.Red);
            else
                generalBrush = new SolidBrush(Color.Gray);

            var hexOffset = new SizeF(cMap.HexToPixel(ObjectCoordinates));
            var myPointArray = PolygonPoints.Select(p => PointF.Add(p, hexOffset)).ToArray();

            g.FillPolygon(generalBrush, myPointArray);
            PointF leftMostHexagonPoint = PointF.Add(cMap.GetHexagonCornerOffset(3), hexOffset);
            g.DrawString(actionsLeft.ToString(), new Font("Arial", 8.0F), Brushes.Blue, PointF.Add(leftMostHexagonPoint, new Size(25, 15)));
            g.DrawString(currentHealth.ToString(), new Font("Arial", 8.0F), Brushes.Red, PointF.Add(leftMostHexagonPoint, new Size(20, -25)));
        }

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
