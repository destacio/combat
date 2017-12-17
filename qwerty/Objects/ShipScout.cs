﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

            var offset = new SizeF(cMap.HexToPixel(ObjectCoordinates));
            var myPointArray = PolygonPoints.Select(p => PointF.Add(p, offset)).ToArray();
            
            g.FillPolygon(generalBrush, myPointArray);
            g.DrawString(actionsLeft.ToString(), new Font("Arial", 8.0F), Brushes.Blue, PointF.Add(cMap.Cells[boxId].CellPoints[3], new Size(25, 15)));
            g.DrawString(currentHealth.ToString(), new Font("Arial", 8.0F), Brushes.Red, PointF.Add(cMap.Cells[boxId].CellPoints[3], new Size(20, -25)));
        }

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
