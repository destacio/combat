using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using Barbar.HexGrid;
using qwerty.Objects;
using Point = System.Drawing.Point;

namespace qwerty
{
    class GameLogic
    {
        private Ship activeShip
        {
            get { return objectManager.ActiveShip; }
            set { objectManager.ActiveShip = value; }
        }

        public string ActiveShipDescription => activeShip?.Description ?? "";
        public string ActivePlayerDescription
        {
            get
            {
                switch (activePlayer)
                {
                    case Player.FirstPlayer:
                        return "First player";
                    case Player.SecondPlayer:
                        return "Second player";
                    default:
                        return activePlayer.ToString();
                }
            }
        }

        public readonly ObjectManager objectManager;
        private Player activePlayer = Player.FirstPlayer;

        public int FirstPlayerShipCount => objectManager.FirstPlayerShipCount;
        public int SecondPlayerShipCount=> objectManager.SecondPlayerShipCount;
        public int BitmapWidth => this.objectManager.BitmapWidth;
        public int BitmapHeight => this.objectManager.BitmapHeight;

        public GameLogic(int fieldWidth, int fieldHeight)
        {
            objectManager = new ObjectManager(fieldWidth, fieldHeight);
        }

        public void HandleFieldClick(Point clickLocation)
        {
            OffsetCoordinates clickedHexagon;
            SpaceObject clickedObject;
            try
            {
                clickedHexagon = objectManager.PixelToOffsetCoordinates(clickLocation);
                clickedObject = objectManager.PixelToSpaceObject(clickLocation);
            }
            catch (ArgumentOutOfRangeException)
            {
                // clicked pixel outside game field
                return;
            }

            if (activeShip == null)
            {
                // Nothing active and nothing to be activated
                if (clickedObject == null) return;

                if (activePlayer == clickedObject.player)
                {
                    activeShip = (Ship) clickedObject;
                }
                return;
            }
            
            // если выбранная клетка пуста - определяем возможность перемещения 
            if (clickedObject == null)
            {
                MoveActiveShip(clickedHexagon);
                return;
            }
            
            if (clickedObject.player == activePlayer)
            {
                activeShip = (Ship) clickedObject;
            }
            else
            {
                ActiveShipAttack(clickedObject);
            }
        }

        private void ActiveShipAttack(SpaceObject enemyObject)
        {
            if (activeShip.EquippedWeapon.attackRange < objectManager.GetDistance(activeShip, enemyObject) ||
                activeShip.actionsLeft < activeShip.EquippedWeapon.energyСonsumption)
            {
                // another object is out of range or requires more energy than is left
                return;
            }
            
            var rotateAngle = this.objectManager.GetTargetHexagonAngle(activeShip.ObjectCoordinates, enemyObject.ObjectCoordinates);
            objectManager.RotateObject(activeShip, rotateAngle);
            
            objectManager.AttackObject(activeShip, enemyObject);

            // TODO: move methods to object manager
            DealDamage(enemyObject, activeShip.AttackDamage);
            activeShip.actionsLeft--;

            objectManager.RotateObject(activeShip, -rotateAngle);

            if (activeShip.actionsLeft == 0)
            {
                activeShip = null;
            }
        }

        private void DealDamage(SpaceObject victim, int damageAmount)
        {
            victim.currentHealth -= damageAmount;
            if (victim.currentHealth <= 0)
            {
                objectManager.DeleteObject(victim);
            }
        }
        
        private void MoveActiveShip(OffsetCoordinates clickedHexagon)
        {
            if (activeShip.actionsLeft <= 0 || !objectManager.CanMoveObjectTo(activeShip, clickedHexagon)) return;

            var rotateAngle = this.objectManager.GetTargetHexagonAngle(activeShip.ObjectCoordinates, clickedHexagon);
            objectManager.RotateObject(activeShip, rotateAngle);
            
            objectManager.MoveObjectTo(activeShip, clickedHexagon);
            activeShip.actionsLeft--;

            objectManager.RotateObject(this.activeShip, -rotateAngle);

            if (activeShip.actionsLeft == 0)
            {
                activeShip = null;
            }
        }
        
        private void MoveMeteors()
        {
            foreach (var meteor in objectManager.Meteors)
            {
                var meteorNextStepCoordinates = objectManager.GetMeteorNextStepCoordinates(meteor);
                var objectOnTheWay = objectManager.GetObjectByOffsetCoordinates(meteorNextStepCoordinates.Column, meteorNextStepCoordinates.Row);

                if (objectOnTheWay == null)
                {
                    objectManager.MoveObjectTo(meteor, meteorNextStepCoordinates);
                    continue;
                }
                
                DealDamage(objectOnTheWay, meteor.explodeDmg);
                objectManager.DeleteObject(meteor);
            }

            if (new Random().Next(0, 100) <= ObjectManager.MeteorAppearanceChance)
            {
                objectManager.CreateMeteor();
            }
        }

        public void EndTurn()
        {
            activePlayer = activePlayer == Player.FirstPlayer ? Player.SecondPlayer : Player.FirstPlayer;
            activeShip = null;
            MoveMeteors();
            objectManager.EndTurn();
        }
    }
}