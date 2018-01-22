using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using qwerty.Objects;

namespace qwerty
{
    class AnimationEventArgs: EventArgs
    {
        public SpaceObject SpaceObject { get; }
        public Point MovementStart { get; }
        public List<Bitmap> AttackSprites { get; }
        public double RotationAngle { get; }
        public Point MovementDestination { get; }

        public AnimationEventArgs(SpaceObject spaceObject, Point movementStart, Point movementDestination)
        {
            this.SpaceObject = spaceObject;
            this.MovementStart = movementStart;
            this.MovementDestination = movementDestination;
        }

        public AnimationEventArgs(SpaceObject spaceObject, double rotationAngle)
        {
            this.SpaceObject = spaceObject;
            this.RotationAngle = rotationAngle;
        }

        public AnimationEventArgs(SpaceObject spaceObject, List<Bitmap> attackSprites)
        {
            this.SpaceObject = spaceObject;
            this.AttackSprites = attackSprites;
        }
    }
}
