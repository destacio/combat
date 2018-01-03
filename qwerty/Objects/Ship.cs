using System;
using System.Drawing;
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
        public PointF WeaponPoint;

        public Ship(Player playerId, WeaponType wpnType)
        {
            objectType = ObjectType.Ship;

            player = playerId;

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

        public bool Attack(CombatMap cMap, int pointB, ref System.Drawing.Bitmap bmap, System.Media.SoundPlayer player, ref PictureBox pictureMap)
        {
            if (actionsLeft < EquippedWeapon.energyСonsumption)
            {
                return false;
            }

            EquippedWeapon.drawAttack(PointF.Add(cMap.HexToPixel(ObjectCoordinates), new SizeF(WeaponPoint)),
                cMap.HexToPixel(ObjectCoordinates), ref bmap, player, ref pictureMap);

            Random rand = new Random();
            int damage = rand.Next(-EquippedWeapon.attackPower / 10, EquippedWeapon.attackPower / 10) + EquippedWeapon.attackPower;
            cMap.Cells[pointB].spaceObject.currentHealth -= damage;
            actionsLeft -= 1;
            if (cMap.Cells[pointB].spaceObject.currentHealth <= 0)
            {
                cMap.Cells[pointB].spaceObject.player = Player.None;
                //cMap.Cells[pointB].spaceObject.boxId = -1;
                cMap.Cells[pointB].spaceObject = null;
                return true;
            }
            return false;
        }
        

        public void Rotate(double angle)
        {
            angle = angle * Math.PI / 180;
            for (int i = 0; i < PolygonPoints.Count; i++)
            {
                PolygonPoints[i] =
                    new PointF((float) (PolygonPoints[i].X * Math.Cos(angle) - PolygonPoints[i].Y * Math.Sin(angle)),
                        (float) (PolygonPoints[i].X * Math.Sin(angle) + PolygonPoints[i].Y * Math.Cos(angle)));
            }
            WeaponPoint = new PointF((float) (WeaponPoint.X * Math.Cos(angle) - WeaponPoint.Y * Math.Sin(angle)),
                (float) (WeaponPoint.X * Math.Sin(angle) + WeaponPoint.Y * Math.Cos(angle)));
        }

        public void RefillEnergy()
        {
            actionsLeft = maxActions;
        }
    }
}
