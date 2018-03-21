﻿﻿using System;
 using System.Collections.Generic;
 using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using Barbar.HexGrid;
using qwerty.Objects;
using Point = System.Drawing.Point;
using System.Threading;
 using qwerty.Objects.Weapons;

namespace qwerty
{
    class FieldPainter
    {
        public Bitmap CurrentBitmap;

        private ObjectManager objectManager;

        private CombatMap combatMap => this.objectManager.CombatMap;

        public event EventHandler BitmapUpdated;

        public FieldPainter(int fieldWidth, int fieldHeight, ObjectManager objectManager)
        {
            this.objectManager = objectManager;
            this.CurrentBitmap = new Bitmap(fieldWidth, fieldHeight, PixelFormat.Format32bppArgb);
        }

        public void UpdateBitmap(AnimationEventArgs animationToPerform = null)  
        {
            if (animationToPerform == null)
            {
                this.DrawField();
                return;
            }

            switch (animationToPerform.AnimationType)
            {
                case AnimationType.Sprites:
                    this.AnimateAttack(animationToPerform.SpaceObject, animationToPerform.OverlaySprites);
                    break;
                case AnimationType.Rotation:
                    this.AnimateRotation(animationToPerform.SpaceObject, animationToPerform.RotationAngle);
                    break;
                case AnimationType.Movement:
                    this.AnimateMovingObjects(animationToPerform.SpaceObject, animationToPerform.MovementStart,
                        animationToPerform.MovementDestination);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void DrawField()
        {
            Graphics g = Graphics.FromImage(this.CurrentBitmap);
            g.FillRectangle(Brushes.Black, 0, 0, this.CurrentBitmap.Width, this.CurrentBitmap.Height);

            foreach (var hexagonCorners in this.combatMap.AllHexagonCorners)
            {
                g.DrawPolygon(Pens.Purple, hexagonCorners);
            }

            if (this.objectManager.ActiveShip != null)
            {
                // highlight active ship attack range
                Pen redPen = new Pen(Color.Red, 1);
                foreach (var hexagonCorners in this.combatMap.GetAllHexagonCornersInRange(this.objectManager.ActiveShip.ObjectCoordinates, this.objectManager.ActiveShip.EquippedWeapon.AttackRange))
                {
                    g.DrawPolygon(redPen, hexagonCorners);
                }

                // highlight active ship
                Pen activeShipAriaPen = new Pen(Color.Purple, 5);
                g.DrawPolygon(activeShipAriaPen, this.combatMap.GetHexagonCorners(this.objectManager.ActiveShip.ObjectCoordinates.Column,
                                                          this.objectManager.ActiveShip.ObjectCoordinates.Row));
            }
            
            
            foreach (var ship in this.objectManager.Ships)
            {
                if (!ship.IsMoving)
                {
                    this.DrawShip(ship);
                }
            }

            foreach (var meteor in this.objectManager.Meteors)
            {
                if (!meteor.IsMoving)
                {
                    this.DrawMeteor(meteor);
                }
            }
            
#if DEBUG
            // draw hexagon coordinates
            for (int x = 0; x < this.combatMap.FieldWidth; x++)
            {
                for (int y = 0; y < this.combatMap.FieldHeight; y++)
                {
                    g.DrawString($"C{x}R{y}", new Font("Arial", 7), Brushes.DeepSkyBlue, this.combatMap.GetHexagonCorners(x,y)[2]);
                    
                    var cubeCoordinates = this.combatMap.HexGrid.ToCubeCoordinates(new OffsetCoordinates(x, y));
                    g.DrawString($"Q{cubeCoordinates.Q}R{cubeCoordinates.R}S{cubeCoordinates.S}", new Font("Arial", 7),
                        Brushes.DeepSkyBlue, PointF.Add(this.combatMap.GetHexagonCorners(x, y)[4], new Size(0, -12)));
                }
            }
#endif
        }

        private void DrawSpaceObject(SpaceObject spaceObject)
        {
            this.DrawSpaceObject(spaceObject, this.combatMap.HexToPixel(spaceObject.ObjectCoordinates));
        }

        private void DrawSpaceObject(SpaceObject spaceObject, Point spaceObjectCoordinates)
        {
            if (spaceObject is Meteor)
            {
                this.DrawMeteor((Meteor) spaceObject, spaceObjectCoordinates);
                return;
            }
            if (spaceObject is Ship)
            {
                this.DrawShip((Ship) spaceObject, spaceObjectCoordinates);
                return;
            }

            throw new ArgumentException($"Drawing of {spaceObject.GetType()} not supported");
        }

        private void DrawMeteor(Meteor meteor)
        {
            this.DrawMeteor(meteor, this.combatMap.HexToPixel(meteor.ObjectCoordinates));
        }

        private void DrawMeteor(Meteor meteor, Point meteorCoordinates)
        {
            var meteorRadius = 15;
            Graphics g = Graphics.FromImage(this.CurrentBitmap);
            g.FillEllipse(Brushes.Gray,
                new Rectangle(Point.Subtract(meteorCoordinates, new Size(meteorRadius, meteorRadius)), new Size(2 * meteorRadius, 2 * meteorRadius)));
            g.DrawString(meteor.CurrentHealth.ToString(), new Font("Arial", 8.0F), Brushes.Red,
                Point.Add(meteorCoordinates, new Size(5, -25)));
            // TODO: better indicate meteor's way
            var directionAngle = 60 * (int) meteor.MovementDirection - 30;
            var directionAngleRadians = (float)directionAngle / 180 * Math.PI;
            var beamStartAngles = new List<double>
            {
                directionAngleRadians + Math.PI / 2,
                directionAngleRadians + 5 * Math.PI / 6,
                //directionAngleRadians + 7 * Math.PI / 8,
                directionAngleRadians + Math.PI,
                //directionAngleRadians + 9 * Math.PI / 8,
                directionAngleRadians + 7 * Math.PI / 6,
                directionAngleRadians + 3 * Math.PI / 2
            };
            foreach (var beamStartAngle in beamStartAngles)
            {
                var beamStartPoint = new Point((int)(meteorRadius * Math.Cos(beamStartAngle)),
                    (int)(-meteorRadius * Math.Sin(beamStartAngle)));
                var beamEndPoint = Point.Add(beamStartPoint,
                    new Size((int) (-20 * Math.Cos(directionAngleRadians)), (int) (20 * Math.Sin(directionAngleRadians))));
                g.DrawLine(new Pen(Color.Yellow, 2), Point.Add(meteorCoordinates, new Size(beamStartPoint)), Point.Add(meteorCoordinates, new Size(beamEndPoint)));
            }
            
            g.DrawArc(new Pen(Color.Blue, 2), meteorCoordinates.X - 10,
                meteorCoordinates.Y - 10, 20, 20, -directionAngle + 20, -40); // start and sweep angles counted clockwise
        }

        private void DrawShip(Ship ship)
        {
            this.DrawShip(ship, this.combatMap.HexToPixel(ship.ObjectCoordinates));
        }

        private void DrawShip(Ship ship, Point shipCoordinates)
        {
            Graphics g = Graphics.FromImage(this.CurrentBitmap);

            SolidBrush generalBrush;

            if (ship.Owner == Player.FirstPlayer)
                generalBrush = new SolidBrush(Color.Blue);
            else if (ship.Owner == Player.SecondPlayer)
                generalBrush = new SolidBrush(Color.Red);
            else
                generalBrush = new SolidBrush(Color.Gray);

            var myPointArray = ship.PolygonPoints.Select(p => PointF.Add(p, new Size(shipCoordinates))).ToArray();
            g.FillPolygon(generalBrush, myPointArray);
            g.DrawString(ship.ActionsLeft.ToString(), new Font("Arial", 8.0F), Brushes.Blue, Point.Add(shipCoordinates, new Size(0, 15)));
            g.DrawString(ship.CurrentHealth.ToString(), new Font("Arial", 8.0F), Brushes.Red, PointF.Add(shipCoordinates, new Size(0, -25)));
        }

        public void OnAnimationPending(object sender, AnimationEventArgs eventArgs)
        {
            this.UpdateBitmap(eventArgs);
        }

        private void AnimateMovingObjects(SpaceObject spaceObject,PointF movementStartPoint, PointF movementDestinationPoint)
        {
            // disabling ability to move multiple objects at once, because it kinda hurts turn-based principle, where each object moves on it's own turn
            if (spaceObject == null)
            {
                return;
            }
            spaceObject.IsMoving = true;
            var stepDifference = new SizeF((movementDestinationPoint.X - movementStartPoint.X) / 10, (movementDestinationPoint.Y - movementStartPoint.Y) / 10);
            var currentCoordinates = movementStartPoint;
            for (int i = 0; i < 10; i++)
            {
                currentCoordinates = PointF.Add(currentCoordinates, stepDifference);
                this.DrawField();
                this.DrawSpaceObject(spaceObject, Point.Round(currentCoordinates));
                this.BitmapUpdated?.Invoke(this, EventArgs.Empty);
                Thread.Sleep(30);
            }

            spaceObject.IsMoving = false;
            this.DrawField();
        }
        
        private void AnimateAttack(SpaceObject pendingAnimationSpaceObject, List<Bitmap> pendingAnimationOverlaySprites)
        {
            foreach (var overlaySprite in pendingAnimationOverlaySprites)
            {
                this.DrawField();
                Graphics g = Graphics.FromImage(this.CurrentBitmap);
                g.DrawImage(overlaySprite, 0, 0);
                this.BitmapUpdated?.Invoke(this, EventArgs.Empty);
                Thread.Sleep(50);
            }
            // animation fully drawn - redraw initial field
            this.DrawField();
        }

        private void AnimateRotation(SpaceObject spaceObject, double angle)
        {
            spaceObject.IsMoving = true;
            angle = angle * Math.PI / 180;
            var dAngle = angle / 10;
            
            var initialPolygonPoints = spaceObject.PolygonPoints.ToList();
            
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < spaceObject.PolygonPoints.Count; j++)
                {
                    spaceObject.PolygonPoints[j] =
                        new PointF((float) (spaceObject.PolygonPoints[j].X * Math.Cos(dAngle) - spaceObject.PolygonPoints[j].Y * Math.Sin(dAngle)),
                            (float) (spaceObject.PolygonPoints[j].X * Math.Sin(dAngle) + spaceObject.PolygonPoints[j].Y * Math.Cos(dAngle)));
                }
                this.DrawField();
                this.DrawSpaceObject(spaceObject);
                this.BitmapUpdated?.Invoke(this, EventArgs.Empty);
                Thread.Sleep(20);
                
            }

            spaceObject.PolygonPoints = initialPolygonPoints;
            spaceObject.IsMoving = false;
            this.DrawField();
        }
    }
}