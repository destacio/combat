﻿using System;
 using System.Collections.Generic;
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

        private CombatMap combatMap => this.objectManager.CombatMap;

        private List<AnimationEventArgs> pendingAnimations = new List<AnimationEventArgs>();

        private bool IsAnimationPending => this.pendingAnimations?.Count > 0;

        public FieldPainter(int fieldWidth, int fieldHeight, ObjectManager objectManager, System.ComponentModel.BackgroundWorker imageUpdater)
        {
            this.objectManager = objectManager;
            this.imageUpdater = imageUpdater;
            CurrentBitmap = new Bitmap(fieldWidth, fieldHeight, PixelFormat.Format32bppArgb);
        }

        public void UpdateBitmap()  
        {
            if (!this.IsAnimationPending)
            {
                this.DrawField();
                return;
            }

            // collect movement animations to be performed simultaneously
            var movementAnimations = new List<AnimationEventArgs>();
            for (var i = 0; i < this.pendingAnimations.Count; i++)
            {
                if (this.pendingAnimations[i].AnimationType == AnimationType.Movement)
                {
                    movementAnimations.Add(this.pendingAnimations[i]);
                    if (i == this.pendingAnimations.Count - 1)
                    {
                        this.AnimateMovingObjects(movementAnimations.Select(a => a.SpaceObject).ToList(),
                            movementAnimations.Select(a => (PointF) a.MovementStart).ToList(),
                            movementAnimations.Select(a => (PointF) a.MovementDestination).ToList());
                    }
                }
                else
                {
                    this.AnimateMovingObjects(movementAnimations.Select(a => a.SpaceObject).ToList(),
                        movementAnimations.Select(a => (PointF) a.MovementStart).ToList(),
                        movementAnimations.Select(a => (PointF) a.MovementDestination).ToList());
                    movementAnimations.Clear();
                    if (this.pendingAnimations[i].AnimationType == AnimationType.Sprites)
                    {
                        this.AnimateAttack(this.pendingAnimations[i].SpaceObject, this.pendingAnimations[i].OverlaySprites);
                    }

                    if (this.pendingAnimations[i].AnimationType == AnimationType.Rotation)
                    {
                        this.AnimateRotation(this.pendingAnimations[i].SpaceObject, this.pendingAnimations[i].RotationAngle);
                    }
                }
            }

            this.pendingAnimations.Clear();
        }

        public void DrawField()
        {
            Graphics g = Graphics.FromImage(CurrentBitmap);
            g.FillRectangle(Brushes.Black, 0, 0, CurrentBitmap.Width, CurrentBitmap.Height);

            foreach (var hexagonCorners in combatMap.AllHexagonCorners)
            {
                g.DrawPolygon(Pens.Purple, hexagonCorners);
            }

            if (this.objectManager.ActiveShip != null)
            {
                // highlight active ship attack range
                Pen redPen = new Pen(Color.Red, 1);
                foreach (var hexagonCorners in combatMap.GetAllHexagonCornersInRange(this.objectManager.ActiveShip.ObjectCoordinates, this.objectManager.ActiveShip.EquippedWeapon.attackRange))
                {
                    g.DrawPolygon(redPen, hexagonCorners);
                }

                // highlight active ship
                Pen activeShipAriaPen = new Pen(Color.Purple, 5);
                g.DrawPolygon(activeShipAriaPen,
                              combatMap.GetHexagonCorners(this.objectManager.ActiveShip.ObjectCoordinates.Column,
                                                          this.objectManager.ActiveShip.ObjectCoordinates.Row));
            }
            
            
            foreach (var ship in this.objectManager.Ships)
            {
                if (!ship.IsMoving)
                {
                    DrawShip(ship);
                }
            }

            foreach (var meteor in this.objectManager.Meteors)
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

        private void DrawSpaceObject(SpaceObject spaceObject)
        {
            this.DrawSpaceObject(spaceObject, combatMap.HexToPixel(spaceObject.ObjectCoordinates));
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
            DrawShip(ship, combatMap.HexToPixel(ship.ObjectCoordinates));
        }

        private void DrawShip(Ship ship, Point shipCoordinates)
        {
            Graphics g = Graphics.FromImage(CurrentBitmap);

            SolidBrush generalBrush;

            if (ship.player == Player.FirstPlayer)
                generalBrush = new SolidBrush(Color.Blue);
            else if (ship.player == Player.SecondPlayer)
                generalBrush = new SolidBrush(Color.Red);
            else
                generalBrush = new SolidBrush(Color.Gray);

            var myPointArray = ship.PolygonPoints.Select(p => PointF.Add(p, new Size(shipCoordinates))).ToArray();
            g.FillPolygon(generalBrush, myPointArray);
            g.DrawString(ship.actionsLeft.ToString(), new Font("Arial", 8.0F), Brushes.Blue, Point.Add(shipCoordinates, new Size(0, 15)));
            g.DrawString(ship.currentHealth.ToString(), new Font("Arial", 8.0F), Brushes.Red, PointF.Add(shipCoordinates, new Size(0, -25)));
        }

        public void OnAnimationPending(object sender, AnimationEventArgs eventArgs)
        {
            this.pendingAnimations.Add(eventArgs);
            this.UpdateBitmap();
        }

        private void AnimateMovingObjects(List<SpaceObject> spaceObjects, List<PointF> movementStartPoints, List<PointF> movementDestinationPoints)
        {
            if (spaceObjects?.Count == 0)
            {
                return;
            }
            // linq magic
            spaceObjects.ForEach(o => o.IsMoving = true);
            var stepDifferences = movementStartPoints.Zip(movementDestinationPoints,
                (startPoint, destinationPoint) => new SizeF((destinationPoint.X - startPoint.X) / 10,
                    (destinationPoint.Y - startPoint.Y) / 10)).ToList();
            var currentCoordinates = new List<PointF>(movementStartPoints);
            for (int i = 0; i < 10; i++)
            {
                currentCoordinates = currentCoordinates.Zip(stepDifferences, PointF.Add).ToList();
                this.DrawField();
                spaceObjects.Zip(currentCoordinates, Tuple.Create).ToList()
                    .ForEach(tuple => this.DrawSpaceObject(tuple.Item1, Point.Round(tuple.Item2)));
                this.imageUpdater.ReportProgress(0);
                Thread.Sleep(30);
            }
            spaceObjects.ForEach(o => o.IsMoving = false);
            // redraw field with current standings
            // could be dangerous if multiple animations get stacked up, but that's later
            this.DrawField();
        }
        
        private void AnimateAttack(SpaceObject pendingAnimationSpaceObject, List<Bitmap> pendingAnimationOverlaySprites)
        {
            foreach (var overlaySprite in pendingAnimationOverlaySprites)
            {
                this.DrawField();
                Graphics g = Graphics.FromImage(this.CurrentBitmap);
                g.DrawImage(overlaySprite, 0, 0);
                this.imageUpdater.ReportProgress(0);
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
                this.imageUpdater.ReportProgress(0);
                Thread.Sleep(20);
                
            }

            spaceObject.PolygonPoints = initialPolygonPoints;
            spaceObject.IsMoving = false;
            this.DrawField();
        }
    }
}