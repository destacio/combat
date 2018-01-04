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

			enemyObject.currentHealth -= activeShip.AttackDamage;
			if (enemyObject.currentHealth < 0)
			{
				objectManager.DeleteObject(enemyObject);
			}
			activeShip.actionsLeft--;
//
//            var angle = cMap.GetAngle(activeShip.boxId, selectedCell.id, activePlayer);
//
//            // поворачиваем корабль на угол angle
//            RotateShip(angle);
//
//            // отрисовка атаки
//            Thread.Sleep(150);
//
//            if (activeShip.Attack(cMap, selectedCell.id, ref combatBitmap, player, ref pictureMap))
//                UpdateShipCount();
//            UpdateUi();
//
//            // возвращаем корабль в исходное положение
//            RotateShip(-angle);
//
//            // убираем подсветку с корабля, если у него не осталось очков передвижений
//            if (activeShip.actionsLeft == 0)
//            {
//                activeShip = null;
//                UpdateUi();
//            }
        }

        private void MoveActiveShip(OffsetCoordinates clickedHexagon)
        {
            if (activeShip.actionsLeft <= 0 || !objectManager.CanMoveObjectTo(activeShip, clickedHexagon)) return;
			/*
             * rotate
             * move
             * rotate back
             */

			objectManager.MoveObjectTo(activeShip, clickedHexagon);
			activeShip.actionsLeft--;

//            var rotateAngle = cMap.GetAngle(activeShip.boxId, selectedCell.id, activePlayer);
//
//            RotateShip(rotateAngle);
//
//            var x1 = cMap.Cells[activeShip.boxId].CellCenter.X;
//            var y1 = cMap.Cells[activeShip.boxId].CellCenter.Y;
//            var x2 = selectedCell.CellCenter.X;
//            var y2 = selectedCell.CellCenter.Y;
//
//            var oldPoints = new List<PointF>(activeShip.PolygonPoints);
//
//            var range = (int) Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
//            var dx = range / 15;
//            int step = 15;
//
//            var deltax = (x2 - x1) / step;
//            var deltay = (y2 - y1) / step;
//
//            for (int count1 = 0; count1 < range - 10; count1 += dx)
//            {
//                for (var i = 0; i < activeShip.PolygonPoints.Count; i++)
//                {
//                    activeShip.PolygonPoints[i] = new PointF(activeShip.PolygonPoints[i].X + deltax,
//                        activeShip.PolygonPoints[i].Y + deltay);
//                }
//                Thread.Sleep(5);
//                UpdateUi();
//            }
//
//            activeShip.PolygonPoints = oldPoints;
//
//            activeShip.Move(cMap, activeShip.boxId, selectedCell.id);
//
//            RotateShip(-rotateAngle);
//
//            //boxDescription.Text = activeShip.Description;
//
//            if (activeShip.actionsLeft == 0) activeShip = null;
//            //UpdateUi();
        }

        public void EndTurn()
        {
//            if (!UpdateShipCount())
//            {
//                return;
//            }

            activePlayer = activePlayer == Player.FirstPlayer ? Player.SecondPlayer : Player.FirstPlayer;

            //lblTurn.Text = "Ходит " + _activePlayer + "-й игрок";

            activeShip = null;

            objectManager.EndTurn();

            //UpdateUi();
        }
    }
}