﻿using System;
 using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using Barbar.HexGrid;
using qwerty.Objects;
using Point = System.Drawing.Point;
using System.Threading;

namespace qwerty
{
    class FieldPainter
    {
        public Bitmap CurrentBitmap;

        private ObjectManager objectManager;
        private readonly System.ComponentModel.BackgroundWorker imageUpdater;

        private CombatMap combatMap => objectManager.CombatMap;

        public FieldPainter(int fieldWidth, int fieldHeight, ObjectManager objectManager, System.ComponentModel.BackgroundWorker imageUpdater)
        {
            this.objectManager = objectManager;
            this.imageUpdater = imageUpdater;
            CurrentBitmap = new Bitmap(fieldWidth, fieldHeight, PixelFormat.Format32bppArgb);
        }
        
        public void DrawField()
        {
            Graphics g = Graphics.FromImage(CurrentBitmap);
            g.FillRectangle(Brushes.Black, 0, 0, CurrentBitmap.Width, CurrentBitmap.Height);

            foreach (var hexagonCorners in combatMap.AllHexagonCorners)
            {
                g.DrawPolygon(Pens.Purple, hexagonCorners);
            }

            if (objectManager.ActiveShip != null)
            {
                // highlight active ship attack range
                Pen redPen = new Pen(Color.Red, 1);
                foreach (var hexagonCorners in combatMap.GetAllHexagonCornersInRange(objectManager.ActiveShip.ObjectCoordinates, objectManager.ActiveShip.EquippedWeapon.attackRange))
                {
                    g.DrawPolygon(redPen, hexagonCorners);
                }

                // highlight active ship
                Pen activeShipAriaPen = new Pen(Color.Purple, 5);
                g.DrawPolygon(activeShipAriaPen,
                              combatMap.GetHexagonCorners(objectManager.ActiveShip.ObjectCoordinates.Column,
                                                          objectManager.ActiveShip.ObjectCoordinates.Row));
            }
            
            
            foreach (var ship in objectManager.Ships)
            {
                DrawShip(ship);
            }

            foreach (var meteor in objectManager.Meteors)
            {
                if (!meteor.IsMoving)
                {
                    DrawMeteor(meteor);
                }
            }
            
#if DEBUG
            // draw hexagon coordinates
            for (int x = 0; x < combatMap.FieldWidth; x++)
            {
                for (int y = 0; y < combatMap.FieldHeight; y++)
                {
                    g.DrawString($"C{x}R{y}", new Font("Arial", 7), Brushes.DeepSkyBlue, combatMap.GetHexagonCorners(x,y)[2]);
                    
                    var cubeCoordinates = combatMap.HexGrid.ToCubeCoordinates(new OffsetCoordinates(x, y));
                    g.DrawString($"Q{cubeCoordinates.Q}R{cubeCoordinates.R}S{cubeCoordinates.S}", new Font("Arial", 7),
                        Brushes.DeepSkyBlue, PointF.Add(combatMap.GetHexagonCorners(x, y)[4], new Size(0, -12)));
                }
            }
#endif
        }

        private void DrawMeteor(Meteor meteor)
        {
            DrawMeteor(meteor, combatMap.HexToPixel(meteor.ObjectCoordinates));
        }

        private void DrawMeteor(Meteor meteor, Point meteorCoordinates)
        {
            Graphics g = Graphics.FromImage(CurrentBitmap);
            g.FillEllipse(Brushes.Gray,
                new Rectangle(Point.Subtract(meteorCoordinates, new Size(15, 15)), new Size(30, 30)));
            g.DrawString(meteor.currentHealth.ToString(), new Font("Arial", 8.0F), Brushes.Red,
                Point.Add(meteorCoordinates, new Size(5, -25)));
            // TODO: better indicate meteor's way
            var directionAngle = 390 - 60 * (int) meteor.MovementDirection;
            g.DrawArc(new Pen(Color.Blue, 2), meteorCoordinates.X - 10,
                meteorCoordinates.Y - 10, 20, 20, directionAngle - 20, 40);
        }

        private void DrawShip(Ship ship)
        {
            Graphics g = Graphics.FromImage(CurrentBitmap);

            SolidBrush generalBrush;

            if (ship.player == Player.FirstPlayer)
                generalBrush = new SolidBrush(Color.Blue);
            else if (ship.player == Player.SecondPlayer)
                generalBrush = new SolidBrush(Color.Red);
            else
                generalBrush = new SolidBrush(Color.Gray);

            var myPointArray = ship.PolygonPoints.Select(p => PointF.Add(p, new Size(combatMap.HexToPixel(ship.ObjectCoordinates)))).ToArray();
            g.FillPolygon(generalBrush, myPointArray);
            g.DrawString(ship.actionsLeft.ToString(), new Font("Arial", 8.0F), Brushes.Blue, Point.Add(combatMap.HexToPixel(ship.ObjectCoordinates), new Size(0, 15)));
            g.DrawString(ship.currentHealth.ToString(), new Font("Arial", 8.0F), Brushes.Red, PointF.Add(combatMap.HexToPixel(ship.ObjectCoordinates), new Size(0, -25)));
        }

        public void PerformAnimation(AnimationEventArgs eventArgs)
        {
            AnimateMovingObject(eventArgs.SpaceObject, eventArgs.MovementDestination);
        }

        private void AnimateMovingObject(SpaceObject spaceObject, Point destination)
        {
            spaceObject.IsMoving = true;
            var stepDifference = new Size((destination.X - combatMap.HexToPixel(spaceObject.ObjectCoordinates).X) / 10,
                (destination.Y - combatMap.HexToPixel(spaceObject.ObjectCoordinates).Y) / 10);
            var currentCoordinates = combatMap.HexToPixel(spaceObject.ObjectCoordinates);
            for (int i = 0; i < 10; i++)
            {
                DrawField();
                DrawMeteor((Meteor)spaceObject, currentCoordinates);
                this.imageUpdater.ReportProgress(0);
                Point.Add(currentCoordinates, stepDifference);
                Thread.Sleep(50);
            }
            spaceObject.IsMoving = false;
        }
    }
}