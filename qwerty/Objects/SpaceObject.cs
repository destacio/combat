using System.Collections.Generic;
using System.Drawing;
using Barbar.HexGrid;

namespace qwerty.Objects
{
    public enum ObjectType
    {
        Ship, Meteor
    }

    public abstract class SpaceObject
    {
        // TODO: is this field really needed?
        public readonly ObjectType objectType;

        public readonly Player Owner;
        public OffsetCoordinates ObjectCoordinates;
        public readonly int MaxHealth;
        public int currentHealth;

        public abstract string Description { get; }
        public bool IsMoving { get; set; }

        public List<PointF> PolygonPoints;

        public readonly int MaxActions;
        public int actionsLeft;

        protected SpaceObject(Player owner, int maxHealth, ObjectType objectType, int maxActions)
        {
            this.Owner = owner;
            this.MaxHealth = maxHealth;
            this.objectType = objectType;
            this.MaxActions = maxActions;
        }

        public abstract void Rotate(double angle);
    }
}
