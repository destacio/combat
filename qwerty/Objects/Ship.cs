using System;
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
        public Weapon equippedWeapon;
        public int weaponPointX;
        public int weaponPointY;

        public Ship(WeaponType wpnType)
        {
            switch (wpnType)
            {
                case WeaponType.HeavyLaser:
                    equippedWeapon = new HeavyLaser();
                    break;
                case WeaponType.LightIon:
                    equippedWeapon = new LightIon();
                    break;
                case WeaponType.LightLaser:
                    equippedWeapon = new LightLaser();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(wpnType), wpnType, null);
            }
        }

        public override string Description => "";

        public override void drawSpaceShit(ref CombatMap cMap, ref System.Drawing.Bitmap bmap)
        {
            
        }

        public void moveShip(CombatMap cMap, int pointAId, int pointBId)
        {

            if (actionsLeft > 0)
            {

                boxId = pointBId;
                cMap.boxes[pointAId].spaceObject = null;
                cMap.boxes[pointBId].spaceObject = this;
                actionsLeft -= 1;

            }
        }
        public int attack(CombatMap cMap, int pointB, ref System.Drawing.Bitmap bmap, System.Media.SoundPlayer player, ref PictureBox pictureMap)
        {
            int dmg;

            if (actionsLeft >= equippedWeapon.energyСonsumption)
            {
                equippedWeapon.drawAttack(cMap.boxes[boxId].xcenter + weaponPointX, cMap.boxes[boxId].ycenter + weaponPointY,
                    cMap.boxes[pointB].xcenter, cMap.boxes[pointB].ycenter,
                    ref bmap, player, ref pictureMap
                );

                Random rand = new Random();
                dmg = rand.Next(-equippedWeapon.attackPower / 10, equippedWeapon.attackPower / 10) + equippedWeapon.attackPower;
                cMap.boxes[pointB].spaceObject.currentHealth -= dmg;
                actionsLeft -= 1;
                if (cMap.boxes[pointB].spaceObject.currentHealth <= 0)
                {
                    cMap.boxes[pointB].spaceObject.player = -1;
                    cMap.boxes[pointB].spaceObject.boxId = -1;
                    cMap.boxes[pointB].spaceObject = null;
                    return 1;
                }
            }
            return 0;
        }
        

        public void shipRotate(double angle)
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
