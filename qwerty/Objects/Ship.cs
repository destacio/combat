﻿using System;
using System.Windows.Forms;
using qwerty.Objects.Weapons;

namespace qwerty.Objects
{
    public enum ShipType
    {
        Scout, Assaulter
    }

    class Ship : SpaceObject
    {
        public readonly Weapon EquippedWeapon;
        public int weaponPointX;
        public int weaponPointY;

        public Ship(WeaponType wpnType)
        {
            switch (wpnType)
            {
                case WeaponType.HeavyLaser:
                    EquippedWeapon = new HeavyLaser();
                    break;
                case WeaponType.LightIon:
                    EquippedWeapon = new LightIon();
                    break;
                case WeaponType.LightLaser:
                    EquippedWeapon = new LightLaser();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(wpnType), wpnType, null);
            }
        }

        public override string Description => "";

        public override void drawSpaceShit(ref CombatMap cMap, ref System.Drawing.Bitmap bmap)
        {
            
        }

        public void Move(CombatMap cMap, int pointAId, int pointBId)
        {

            if (actionsLeft > 0)
            {

                boxId = pointBId;
                cMap.Cells[pointAId].spaceObject = null;
                cMap.Cells[pointBId].spaceObject = this;
                actionsLeft -= 1;

            }
        }
        public bool Attack(CombatMap cMap, int pointB, ref System.Drawing.Bitmap bmap, System.Media.SoundPlayer player, ref PictureBox pictureMap)
        {
            if (actionsLeft < EquippedWeapon.energyСonsumption)
            {
                return false;
            }

            EquippedWeapon.drawAttack(cMap.Cells[boxId].xcenter + weaponPointX, cMap.Cells[boxId].ycenter + weaponPointY,
                cMap.Cells[pointB].xcenter, cMap.Cells[pointB].ycenter,
                ref bmap, player, ref pictureMap);

            Random rand = new Random();
            int damage = rand.Next(-EquippedWeapon.attackPower / 10, EquippedWeapon.attackPower / 10) + EquippedWeapon.attackPower;
            cMap.Cells[pointB].spaceObject.currentHealth -= damage;
            actionsLeft -= 1;
            if (cMap.Cells[pointB].spaceObject.currentHealth <= 0)
            {
                cMap.Cells[pointB].spaceObject.player = -1;
                cMap.Cells[pointB].spaceObject.boxId = -1;
                cMap.Cells[pointB].spaceObject = null;
                return true;
            }
            return false;
        }
        

        public void Rotate(double angle)
        {
            int xold, yold;
            angle = angle * Math.PI / 180;
            for (int i = 0; i < xpoints.Count; i++)
            {
                xold = xpoints[i];
                yold = ypoints[i];
                xpoints[i] = (int)(Math.Round((double)xold * Math.Cos(angle) - (double)yold * Math.Sin(angle), 0));
                ypoints[i] = (int)(Math.Round((double)xold * Math.Sin(angle) + (double)yold * Math.Cos(angle), 0));
            }
            weaponPointX = (int)(Math.Round((double)weaponPointX * Math.Cos(angle) - (double)weaponPointY * Math.Sin(angle), 0));
            weaponPointY = (int)(Math.Round((double)weaponPointX * Math.Sin(angle) + (double)weaponPointY * Math.Cos(angle), 0));
        }

        public void RefillEnergy()
        {
            actionsLeft = maxActions;
        }
    }
}
